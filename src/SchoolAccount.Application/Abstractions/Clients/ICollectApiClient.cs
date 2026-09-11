using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
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

    Task<Result<GetCensusJourneyContentResponse>> GetCensusJourneyContent(
        string id,
        string emailAddress,
        IReadOnlyList<Organisation> organisations,
        CancellationToken cancellationToken
    );
}
