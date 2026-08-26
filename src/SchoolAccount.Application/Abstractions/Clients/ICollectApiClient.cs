using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Abstractions.Clients;

public interface ICollectApiClient
{
    Task<List<GetCensusStatusesResponse>> GetCensusStatuses(
        string id,
        string emailAddress,
        List<Organisation> organisations,
        CancellationToken cancellationToken
    );
}
