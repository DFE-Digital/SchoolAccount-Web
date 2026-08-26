namespace SchoolAccount.Application.Collect.CensusStatuses;

public record GetCensusStatusesResponse
{
    public string Id { get; init; }
    public bool Interesting { get; init; }
    public IReadOnlyList<CensusAction> Actions { get; init; } = [];
}

public record CensusAction
{
    public string Name { get; init; }
    public CensusStatus Status { get; init; }
}

public record CensusStatus
{
    public string Name { get; init; }
}
