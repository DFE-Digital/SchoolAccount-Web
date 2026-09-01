using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.Application.Features.GetCensusActions;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Abstractions.Clients;

public interface ICollectApiClient
{
    Task<List<GetCensusStatusesResponse>> GetCensusStatuses(
        string id,
        string emailAddress,
        IReadOnlyList<Organisation> organisations,
        CancellationToken cancellationToken
    );

    Task<Result<GetServiceActionsResponse>> GetCensusJourneyContent(
        string id,
        string emailAddress,
        IReadOnlyList<Organisation> organisations,
        CancellationToken cancellationToken
    );
}
