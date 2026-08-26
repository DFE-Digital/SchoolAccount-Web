namespace SchoolAccount.Application.Collect.CensusStatuses;

public interface ICollectApiService
{
    Task<List<GetCensusStatusesResponse>> GetCensusStatuses(
        GetCensusStatusesQuery query,
        CancellationToken cancellationToken
    );
}
