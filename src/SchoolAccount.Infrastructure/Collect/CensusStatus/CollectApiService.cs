using System.Net.Http.Json;
using SchoolAccount.Application.Collect.CensusStatus;
using Action = SchoolAccount.Application.Collect.CensusStatus.Action;

namespace SchoolAccount.Infrastructure.Collect.CensusStatus;

public class CollectApiService(HttpClient httpClient) : ICollectApiService
{
    public async Task<List<GetCensusStatusResponse>> GetCensusStatus(GetCensusStatusQuery query)
    {
        var response = await httpClient.PostAsJsonAsync("/status", query.request);
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
