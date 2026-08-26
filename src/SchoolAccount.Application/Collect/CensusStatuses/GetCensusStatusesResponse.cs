namespace SchoolAccount.Application.Collect.CensusStatuses;

public record GetCensusStatusesResponse
{
    public string Id { get; init; }
    public bool Interesting { get; init; }
    public List<Action> Actions { get; init; } = [];
}

public record Action
{
    public string Name { get; init; }
    public Status Status { get; init; }
}

public class Status
{
    public string Name { get; init; }
}
