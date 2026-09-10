using System.Net;
using System.Text.Json;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Features.Academies.GetAcademies;
using SchoolAccount.SharedKernel;
using static System.Net.Mime.MediaTypeNames.Application;
using static System.StringComparison;
using static SchoolAccount.Infrastructure.Clients.Academies.GetAcademies.GetAcademiesMapper;

namespace SchoolAccount.Infrastructure.Clients.Academies;

public class AcademiesApiClient(
    IEstablishmentsV4Client establishments,
    ITrustsV4Client trusts,
    ILogger<AcademiesApiClient> logger
) : IAcademiesApiClient
{
    private static readonly JsonSerializerOptions _problemJsonOptions = new(
        JsonSerializerDefaults.Web
    );

    public async Task<Result<GetAcademyEstablishmentResponse>> GetEstablishmentDetails(
        string ukprn,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var response = await establishments.GetEstablishmentByUkprnAsync(
                ukprn,
                cancellationToken
            );
            return Result.Success(ToEstablishmentResponse(response));
        }
        catch (AcademiesApiException exception)
        {
            LogProblem(exception, $"establishment/{ukprn}");
            return Result.Failure<GetAcademyEstablishmentResponse>(
                Error.Failure("Academies API", $"Failed to retrieve establishment {ukprn}")
            );
        }
    }

    public async Task<Result<GetAcademyTrustResponse>> GetTrustDetails(
        string ukprn,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var trustResponse = await trusts.GetTrustByUkprn2Async(ukprn, cancellationToken);
            var trustEstablishmentsResponse = await establishments.GetByTrustAsync(
                ukprn,
                cancellationToken
            );

            var trustEstablishments = new List<GetAcademyEstablishmentResponse>();

            foreach (var dto in trustEstablishmentsResponse)
            {
                try
                {
                    trustEstablishments.Add(ToEstablishmentResponse(dto));
                }
                catch (ArgumentException exception)
                {
                    logger.LogWarning(
                        exception,
                        "Skipping malformed establishment within trust directory for {Ukprn}",
                        ukprn
                    );
                }
            }

            return Result.Success(ToTrustResponse(trustResponse, trustEstablishments));
        }
        catch (AcademiesApiException exception)
        {
            LogProblem(exception, $"trust/{ukprn}");
            return Result.Failure<GetAcademyTrustResponse>(
                Error.Failure("Academies API", $"Failed to retrieve trust {ukprn}")
            );
        }
    }

    private void LogProblem(AcademiesApiException exception, string requestUri)
    {
        var statusCode = exception.StatusCode;
        var isBadRequest = statusCode == (int)HttpStatusCode.BadRequest;

        var isJson =
            exception.Headers.TryGetValue("Content-Type", out var contentTypes)
            && contentTypes.Any(contentType =>
                contentType.Contains(ProblemJson, OrdinalIgnoreCase)
                || contentType.Contains(Json, OrdinalIgnoreCase)
            );

        if (!isBadRequest || !isJson)
        {
            logger.LogError(
                "Request to {RequestUri} failed with status {StatusCode}. Response: {Response}",
                requestUri,
                exception.StatusCode,
                exception.Response
            );
            return;
        }

        var problemDetails = ReadProblemDetails(exception.Response);

        if (problemDetails is null)
        {
            return;
        }

        if (problemDetails.Errors.Count > 0)
        {
            logger.LogError(
                "Request to {RequestUri} failed validation with {ValidationErrorCount} errors {@ValidationErrors}",
                requestUri,
                problemDetails.Errors.Count,
                problemDetails.Errors
            );
        }
    }

    private static HttpValidationProblemDetails? ReadProblemDetails(string? response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<HttpValidationProblemDetails>(
                response,
                _problemJsonOptions
            );
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
