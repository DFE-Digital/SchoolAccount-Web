namespace SchoolAccount.Application.Collect.CensusStatus;

public record GetCensusStatusResponse
{
    public string Name { get; init; }
    public Status Status { get; init; }
}

public class Status
{
    public string Name { get; init; }
}
