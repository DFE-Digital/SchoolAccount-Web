using SchoolAccount.Application.Collect.CensusStatus;

namespace SchoolAccount.Infrastructure.Collect.CensusStatus;

public class GetCensusStatusDto
{
    public List<OrganisationResponse> Details { get; init; } = new();
}

public class OrganisationResponse
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string CategoryId { get; init; }
    public string Ukprn { get; init; }
    public string Laestab { get; init; }
    public bool Interesting { get; init; }
    public List<GetCensusStatusResponse> Actions { get; init; }
}
