using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Infrastructure.Http;

public static class HttpResultExtensions
{
    public static async Task<HttpOutcome<HttpResponseMessage>> Validate(
        this Task<HttpResponseMessage> responseTask,
        ILogger logger,
        string requestUri
    )
    {
        return await Validate(
            responseTask,
            logger,
            new Uri(requestUri, UriKind.RelativeOrAbsolute)
        );
    }

    /// <summary>
    /// Starts the chain: awaits the HttpClient call and wraps the response as a
    /// successful outcome. Any exception thrown while sending the request (DNS
    /// failure, connection refused, timeout, ...) simply propagates up the task
    /// chain to be caught by the terminal <see cref="Catch{T}"/> call. This is
    /// also where a Polly resilience handler on the HttpClient registration would
    /// transparently retry, before an exception ever reaches this chain.
    /// </summary>
    public static async Task<HttpOutcome<HttpResponseMessage>> Validate(
        this Task<HttpResponseMessage> responseTask,
        ILogger logger,
        Uri requestUri
    )
    {
        var context = new HttpCallContext(logger, requestUri);
        var response = await responseTask;

        return new HttpOutcome<HttpResponseMessage>(Result.Success(response), context);
    }

    /// <summary>
    /// Turns a non-success status code into a Failure result, logging problem
    /// details when the API returned them. No-ops if the outcome is already a
    /// failure from an earlier stage.
    /// </summary>
    public static async Task<HttpOutcome<HttpResponseMessage>> ValidateProblems(
        this Task<HttpOutcome<HttpResponseMessage>> outcomeTask
    )
    {
        var outcome = await outcomeTask;

        if (!outcome.Result.TryGetValue(out var response))
        {
            return outcome;
        }

        if (response.IsSuccessStatusCode)
        {
            return outcome;
        }

        var error = await BuildApiProblemError(response, outcome.Context);
        response.Dispose();

        return new HttpOutcome<HttpResponseMessage>(
            Result.Failure<HttpResponseMessage>(error),
            outcome.Context
        );
    }

    /// <summary>
    /// Deserializes the response body as <typeparamref name="TResponse"/> and maps
    /// it to <typeparamref name="TResult"/>. Disposes the response either way.
    /// </summary>
    public static async Task<HttpOutcome<TResult>> Query<TResponse, TResult>(
        this Task<HttpOutcome<HttpResponseMessage>> outcomeTask,
        Func<TResponse, TResult> map
    )
    {
        var outcome = await outcomeTask;

        if (!outcome.Result.TryGetValue(out var response))
        {
            return new HttpOutcome<TResult>(
                Result.Failure<TResult>(outcome.Result.Error),
                outcome.Context
            );
        }

        using (response)
        {
            var content = await response.Content.ReadFromJsonAsync<TResponse>();

            if (content is null)
            {
                outcome.Context.Logger.LogError(
                    "Response from {RequestUri} was empty",
                    outcome.Context.RequestUri
                );

                var error = new ValidationError([
                    $"Response from {outcome.Context.RequestUri} was empty",
                ]);

                return new HttpOutcome<TResult>(Result.Failure<TResult>(error), outcome.Context);
            }

            return new HttpOutcome<TResult>(Result.Success(map(content)), outcome.Context);
        }
    }

    /// <summary>
    /// Terminal stage of the chain. Catches every exception that escaped the
    /// earlier stages (transport failures, timeouts, bad JSON, anything else),
    /// logs it, and converts it into a Failure Result rather than letting it
    /// bubble up to the caller. This single method replaces the four chained
    /// `.Catch&lt;TException, TResult&gt;()` calls from before.
    /// </summary>
    public static async Task<Result<T>> Catch<T>(
        this Task<HttpOutcome<T>> outcomeTask,
        ILogger logger,
        Uri requestUri
    )
    {
        try
        {
            var outcome = await outcomeTask;
            return outcome.Result;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Request to {RequestUri} could not be sent", requestUri);
            return Result.Failure<T>(Error.Critical("Request could not be sent", ex));
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Request to {RequestUri} timed out", requestUri);
            return Result.Failure<T>(Error.Critical("Request timed out", ex));
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Response from {RequestUri} was not valid JSON", requestUri);
            return Result.Failure<T>(Error.Critical("Response was not valid JSON", ex));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request to {RequestUri} failed unexpectedly", requestUri);
            return Result.Failure<T>(Error.Critical("Request failed unexpectedly", ex));
        }
    }

    private static async Task<Error> BuildApiProblemError(
        HttpResponseMessage response,
        HttpCallContext context
    )
    {
        var statusCode = (int)response.StatusCode;
        var problem = await ReadProblemDetails(response);

        if (problem is null)
        {
            context.Logger.LogError(
                "Request to {RequestUri} failed with status {StatusCode}",
                context.RequestUri,
                statusCode
            );

            return Error.Problem($"{statusCode}", $"Request failed with status {statusCode}");
        }

        if (problem.Errors.Count > 0)
        {
            context.Logger.LogError(
                "Request to {RequestUri} failed validation with {ValidationErrorCount} errors {@ValidationErrors}",
                context.RequestUri,
                problem.Errors.Count,
                problem.Errors
            );
        }
        else
        {
            context.Logger.LogError(
                "Request to {RequestUri} failed with status {StatusCode}: {Title} {Detail}",
                context.RequestUri,
                statusCode,
                problem.Title,
                problem.Detail
            );
        }

        return new ValidationError([
            Error.Problem($"{statusCode}", $"Request failed with status {statusCode}"),
            .. problem.Errors.Select(e => Error.Problem(e.Key, string.Join(", ", e.Value))),
        ]);
    }

    private static async Task<HttpValidationProblemDetails?> ReadProblemDetails(
        HttpResponseMessage response
    )
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
