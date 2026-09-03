namespace SchoolAccount.Application.Features.GetCensusJourney;

public record GetCensusJourneyResponse
{
    public string Title { get; init; }
    public string Caption { get; init; }
    public string? Overview { get; init; }
    public GetCensusJourneyResponseStatus Status { get; init; }
    public IReadOnlyList<GetCensusJourneyResponseImportantDates> ImportantDates { get; init; } = [];
    public GetCensusJourneyResponseCallToAction CallToAction { get; init; }
}

public class GetCensusJourneyResponseStatus
{
    public string Name { get; init; }
    public string Label { get; init; }
}

public class GetCensusJourneyResponseImportantDates
{
    public string Label { get; init; }
    public DateOnly Date { get; init; }
}

public record GetCensusJourneyResponseCallToAction
{
    public Uri Url { get; init; }
    public string Label { get; init; }
}
