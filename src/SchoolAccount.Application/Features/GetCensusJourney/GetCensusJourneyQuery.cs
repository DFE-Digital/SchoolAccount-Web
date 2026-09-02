using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Features.GetCensusJourney;

public record GetCensusJourneyQuery : IQuery<GetCensusJourneyResponse>
{
    public string Id { get; init; }

    public string EmailAddress { get; init; }

    public IReadOnlyList<Organisation> Organisations { get; init; } = [];
}
