namespace SchoolAccount.Application.Features.Collect.CensusStatuses;

public record GetCensusStatusesResponse
{
    public string Id { get; init; }
    public string SchoolName { get; init; }
    public bool Interesting { get; init; }
    public IReadOnlyList<CensusAction> Actions { get; init; } = [];
}

public record CensusAction
{
    public string Name { get; init; }
    public CensusStatus Status { get; init; }
    public int? Errors { get; init; }
    public int? Queries { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CensusStatus
{
    public string Name { get; init; }
}
