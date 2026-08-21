using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Collect.CensusStatus;
using Action = SchoolAccount.Application.Collect.CensusStatus.Action;

namespace SchoolAccount.Infrastructure.Collect.CensusStatus;

public sealed class CollectApiService(HttpClient httpClient, ILogger<CollectApiService> logger)
    : ICollectApiService
{
    private const string _statusEndpoint = "/status";

    public async Task<List<GetCensusStatusResponse>> GetCensusStatus(GetCensusStatusQuery query)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync(_statusEndpoint, query.request);

            if (!response.IsSuccessStatusCode)
            {
                await LogProblem(response, _statusEndpoint);

                return [];
            }

            var content = await response.Content.ReadFromJsonAsync<GetCensusStatusApiResponse>();

            if (content is null)
            {
                logger.LogError("Response from {RequestUri} was empty", _statusEndpoint);

                return [];
            }

            return content.Details.ConvertAll(MapToCensusStatus);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "Request to {RequestUri} could not be sent",
                _statusEndpoint
            );
        }
        catch (TaskCanceledException exception)
        {
            logger.LogError(exception, "Request to {RequestUri} timed out", _statusEndpoint);
        }
        catch (JsonException exception)
        {
            logger.LogError(
                exception,
                "Response from {RequestUri} was not valid JSON",
                _statusEndpoint
            );
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Request to {RequestUri} failed unexpectedly",
                _statusEndpoint
            );
        }

        return [];
    }

    private async Task LogProblem(HttpResponseMessage response, string requestUri)
    {
        var statusCode = (int)response.StatusCode;
        var problemDetails = await ReadProblemDetails(response);

        if (problemDetails is null)
        {
            logger.LogError(
                "Request to {RequestUri} failed with status {StatusCode}",
                requestUri,
                statusCode
            );

            return;
        }

        if (problemDetails.Errors.Count is 0)
        {
            logger.LogError(
                "Request to {RequestUri} failed with status {StatusCode}: {Title} {Detail}",
                requestUri,
                statusCode,
                problemDetails.Title,
                problemDetails.Detail
            );

            return;
        }

        foreach (var (field, messages) in problemDetails.Errors)
        {
            foreach (var message in messages)
            {
                logger.LogError(
                    "Request to {RequestUri} failed validation on {Field}: {ValidationMessage}",
                    requestUri,
                    field,
                    message
                );
            }
        }
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

    private static GetCensusStatusResponse MapToCensusStatus(OrganisationResponse organisation) =>
        new()
        {
            Id = organisation.Id,
            Interesting = organisation.Interesting,
            Actions = organisation.Actions.ConvertAll(action => new Action
            {
                Name = action.Name,
                Status = new Status { Name = action.Status.Name },
            }),
        };
}
