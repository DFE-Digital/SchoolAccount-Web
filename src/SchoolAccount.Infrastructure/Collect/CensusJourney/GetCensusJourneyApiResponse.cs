namespace SchoolAccount.Infrastructure.Collect.CensusJourney;

public class GetCensusJourneyApiResponse
{
    public required CallToActionApiResponse CallToAction { get; init; }
}

public class CallToActionApiResponse
{
    public string CallToActionButtonText { get; init; }
    public string CallToActionUrl { get; init; }
}
