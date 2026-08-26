namespace SchoolAccount.Application.Collect.CensusStatuses;

public interface ICollectApiClient
{
    Task<List<GetCensusStatusesResponse>> GetCensusStatuses(
        GetCensusStatusesQuery query,
        CancellationToken cancellationToken
    );
}
