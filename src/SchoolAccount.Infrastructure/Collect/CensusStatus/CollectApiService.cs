using System.Net.Http.Json;
using SchoolAccount.Application.Collect.CensusStatus;

namespace SchoolAccount.Infrastructure.Collect.CensusStatus;

public class CollectApiService(HttpClient httpClient) : ICollectApiService
{
    public async Task<GetCensusStatusResponse> GetCensusStatus(GetCensusStatusQuery query)
    {
        var response = await httpClient.PostAsJsonAsync("/status", query.request);
        var results =
            await response.Content.ReadFromJsonAsync<GetCensusStatusDto>()
            ?? throw new Exception("Failed to get census status");

        var statusResponse = new GetCensusStatusResponse
        {
            Name = results.Details[0].Actions[0].Name,
            Status = results.Details[0].Actions[0].Status,
        };

        return statusResponse;
    }
}
