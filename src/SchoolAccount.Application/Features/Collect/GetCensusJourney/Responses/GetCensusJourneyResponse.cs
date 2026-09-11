using SchoolAccount.Application.Features.Collect.CensusStatuses;

namespace SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;

public record GetCensusJourneyResponse
{
    public GetCensusJourneyContentResponse Content { get; init; }

    public IReadOnlyList<GetCensusStatusesResponse> SchoolStatuses { get; init; }
}
