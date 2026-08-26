using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Collect.CensusStatuses;

public interface ICollectApiClient
{
    Task<List<GetCensusStatusesResponse>> GetCensusStatuses(
        string id,
        string emailAddress,
        List<Organisation> organisations,
        CancellationToken cancellationToken
    );
}
