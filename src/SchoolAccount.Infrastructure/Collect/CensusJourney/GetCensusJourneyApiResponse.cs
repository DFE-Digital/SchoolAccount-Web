namespace SchoolAccount.Infrastructure.Collect.CensusJourney;

public class GetCensusJourneyApiResponse
{
    public required CallToActionApiResponse CallToAction { get; init; }
}

public class CallToActionApiResponse
{
    public string Label { get; init; }
    public Uri Url { get; init; }
}
