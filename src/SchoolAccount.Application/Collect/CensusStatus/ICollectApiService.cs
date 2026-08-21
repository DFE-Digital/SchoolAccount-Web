namespace SchoolAccount.Application.Collect.CensusStatus;

public interface ICollectApiService
{
    Task<GetCensusStatusResponse> GetCensusStatus(GetCensusStatusQuery query);
}
