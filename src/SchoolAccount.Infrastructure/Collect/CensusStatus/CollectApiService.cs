using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Collect.CensusStatus;
using static System.Net.Mime.MediaTypeNames.Application;
using Action = SchoolAccount.Application.Collect.CensusStatus.Action;

namespace SchoolAccount.Infrastructure.Collect.CensusStatus;

public sealed class CollectApiService(HttpClient httpClient, ILogger<CollectApiService> logger)
    : ICollectApiService
{
    private const string _statusEndpoint = "/status";

    public async Task<List<GetCensusStatusResponse>> GetCensusStatus(
        GetCensusStatusQuery query,
        CancellationToken cancellationToken
    )
    {
        using var response = await httpClient.PostAsJsonAsync(
            _statusEndpoint,
            query.Request,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            await LogProblem(response, _statusEndpoint);
            response.EnsureSuccessStatusCode();
        }

        var content = await response.Content.ReadFromJsonAsync<GetCensusStatusApiResponse>(
            cancellationToken
        );

        if (content is null)
        {
            logger.LogError("Response from {RequestUri} was empty", _statusEndpoint);

            throw new Exception("Response from Collect API was empty");
        }

        return content.Details.ConvertAll(MapToCensusStatus);
    }

    private async Task LogProblem(HttpResponseMessage response, string requestUri)
    {
        var statusCode = response.StatusCode;
        var isBadRequest = statusCode == HttpStatusCode.BadRequest;
        var isJson = response.Content.Headers.ContentType?.MediaType is ProblemJson or Json;

        if (!isBadRequest || !isJson)
        {
            return;
        }

        var problemDetails = await ReadProblemDetails(response);

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
