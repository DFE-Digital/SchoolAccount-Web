namespace SchoolAccount.Application.Collect.CensusStatus;

public interface ICollectApiService
{
    Task<List<GetCensusStatusResponse>> GetCensusStatus(
        GetCensusStatusQuery query,
        CancellationToken cancellationToken
    );
}
