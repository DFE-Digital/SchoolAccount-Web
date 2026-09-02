namespace SchoolAccount.Application.Collect.CensusStatus;

public class GetCensusStatusResponse(IEnumerable<GetCensusStatus> items)
    : List<GetCensusStatus>(items)
{
    public static GetCensusStatusResponse Create(params GetCensusStatus[] statuses)
    {
        return new GetCensusStatusResponse(statuses.ToList());
    }
};

public record GetCensusStatus
{
    public string Id { get; init; }
    public bool Interesting { get; init; }
    public List<Action> Actions { get; init; }
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
