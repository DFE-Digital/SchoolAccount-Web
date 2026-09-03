namespace SchoolAccount.Infrastructure.Collect.CensusJourney;

public class GetCensusJourneyApiResponse
{
    public string Title { get; init; }
    public string Caption { get; init; }
    public string? Overview { get; init; }

    public GetCensusJourneyApiResponseStatus Status { get; init; }

    public IReadOnlyList<GetCensusJourneyApiResponseImportantDate> ImportantDates { get; init; } =
    [];

    public required GetCensusJourneyApiResponseCallToAction CallToAction { get; init; }
}

public class GetCensusJourneyApiResponseStatus
{
    public string Label { get; init; }
}

public class GetCensusJourneyApiResponseImportantDate
{
    public string Label { get; init; }
    public DateOnly Date { get; init; }
}

public class GetCensusJourneyApiResponseCallToAction
{
    public string Label { get; init; }
    public Uri Url { get; init; }
}
