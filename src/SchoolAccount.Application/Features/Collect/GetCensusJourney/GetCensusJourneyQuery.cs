using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Features.Collect.GetCensusJourney;

public record GetCensusJourneyQuery : IQuery<GetCensusJourneyResponse>
{
    public string Id { get; init; }

    public string EmailAddress { get; init; }

    public Organisation Organisation { get; init; }
}
