using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Infrastructure.Clients.Collect.CensusJourney;

public class GetCensusJourneyApiRequest
{
    public string Id { get; init; }

    public string Email { get; init; }

    public IReadOnlyList<Organisation> Organisations { get; init; }
}
