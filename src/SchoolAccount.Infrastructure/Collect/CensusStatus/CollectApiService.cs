using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Collect.CensusStatus;
using Action = SchoolAccount.Application.Collect.CensusStatus.Action;

namespace SchoolAccount.Infrastructure.Collect.CensusStatus;

public class CollectApiService(HttpClient httpClient, ILogger<CollectApiService> logger)
    : ICollectApiService
{
    public async Task<List<GetCensusStatusResponse>> GetCensusStatus(GetCensusStatusQuery query)
    {
        var response = await httpClient.PostAsJsonAsync("/status", query.request);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var validationProblemDetails =
                await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(
                    CancellationToken.None
                );

            if (validationProblemDetails is not null)
            {
                foreach (var error in validationProblemDetails.Errors)
                {
                    foreach (var message in error.Value)
                    {
                        logger.LogError("{Message}", message);
                    }
                }
            }

            return [];
        }

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(
                CancellationToken.None
            );

            if (problemDetails is not null)
            {
                logger.LogError("{Message}", problemDetails.Detail);
            }

            return [];
        }

        var results =
            await response.Content.ReadFromJsonAsync<GetCensusStatusApiResponse>()
            ?? throw new Exception("Failed to get census status");

        var statusResponses = results
            .Details.Select(x => new GetCensusStatusResponse
            {
                Id = x.Id,
                Interesting = x.Interesting,
                Actions = x
                    .Actions.Select(y => new Action
                    {
                        Name = y.Name,
                        Status = new Status { Name = y.Status.Name },
                    })
                    .ToList(),
            })
            .ToList();

        return statusResponses;
    }
}
