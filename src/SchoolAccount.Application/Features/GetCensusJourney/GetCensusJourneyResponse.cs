namespace SchoolAccount.Application.Features.GetCensusJourney;

public record GetCensusJourneyResponse
{
    public string Title { get; init; }
    public string Caption { get; init; }
    public string? Overview { get; init; }
    public string Status { get; init; }
    public IReadOnlyList<GetCensusJourneyResponseImportantDate> ImportantDates { get; init; } = [];
    public GetCensusJourneyResponseCallToAction CallToAction { get; init; }
}

public class GetCensusJourneyResponseImportantDate
{
    public string Label { get; init; }
    public DateOnly Date { get; init; }
}

public record GetCensusJourneyResponseCallToAction
{
    public Uri Url { get; init; }
    public string Label { get; init; }
}
