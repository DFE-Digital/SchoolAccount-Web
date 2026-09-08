using System.Net;
using System.Text.Json;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Features.Academies;
using SchoolAccount.Application.Features.Academies.GetAcademies;
using SchoolAccount.Infrastructure.Clients.Academies.GetAcademies;
using static System.Net.Mime.MediaTypeNames.Application;

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

    public async Task<GetAcademyTrustResponse> GetTrustDetails(
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
            return GetAcademiesMapper.ToTrustResponse(trustResponse, trustEstablishmentsResponse);
        }
        catch (AcademiesApiException exception) when (exception.StatusCode == 404)
        {
            return null;
        }
        catch (AcademiesApiException exception)
        {
            LogProblem(exception, $"trust/{ukprn}");
            throw;
        }
    }

    public async Task<GetAcademyEstablishmentResponse> GetEstablishmentDetails(
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
            return GetAcademiesMapper.ToEstablishmentResponse(response);
        }
        catch (AcademiesApiException exception) when (exception.StatusCode == 404)
        {
            return null;
        }
        catch (AcademiesApiException exception)
        {
            LogProblem(exception, $"trust/{ukprn}");
            throw;
        }
    }

    private void LogProblem(AcademiesApiException exception, string requestUri)
    {
        var statusCode = exception.StatusCode;
        var isBadRequest = statusCode == (int)HttpStatusCode.BadRequest;

        var isJson =
            exception.Headers.TryGetValue("Content-Type", out var contentTypes)
            && contentTypes.Any(contentType =>
                contentType.Contains(ProblemJson, StringComparison.OrdinalIgnoreCase)
                || contentType.Contains(Json, StringComparison.OrdinalIgnoreCase)
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
