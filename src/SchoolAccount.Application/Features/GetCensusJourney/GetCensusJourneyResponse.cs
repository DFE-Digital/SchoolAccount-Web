namespace SchoolAccount.Application.Features.GetCensusJourney;

public record GetCensusJourneyResponse
{
    public CallToAction CallToAction { get; init; }
}

public record CallToAction
{
    public string Url { get; init; }
    public string ButtonText { get; init; }
}
