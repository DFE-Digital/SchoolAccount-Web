using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace SchoolAccount.Infrastructure.Collect.CensusStatuses;

public class GetCensusStatusesApiResponse
{
    public IReadOnlyList<OrganisationResponse> Details { get; init; } = [];
}

[DynamicallyAccessedMembers(AllProperties | AllConstructors)]
public class OrganisationResponse
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string CategoryId { get; init; }
    public string Ukprn { get; init; }
    public string Laestab { get; init; }
    public bool Interesting { get; init; }
    public IReadOnlyList<ActionApiResponse> Actions { get; init; } = [];
}

[DynamicallyAccessedMembers(AllProperties | AllConstructors)]
public class ActionApiResponse
{
    public string Name { get; init; }
    public StatusApiResponse Status { get; init; }
}

[DynamicallyAccessedMembers(AllProperties | AllConstructors)]
public class StatusApiResponse
{
    public string Name { get; init; }
}
