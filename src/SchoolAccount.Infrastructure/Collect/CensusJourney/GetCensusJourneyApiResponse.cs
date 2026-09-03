namespace SchoolAccount.Infrastructure.Collect.CensusJourney;

public class GetCensusJourneyApiResponse
{
    public string Title { get; init; }
    public string Caption { get; init; }
    public string? Overview { get; init; }

    public GetCensusJourneyApiResponseStatus Status { get; init; }

    public IReadOnlyList<GetCensusJourneyApiResponseImportantDates> ImportantDates { get; init; } =
    [];

    public required GetCensusJourneyApiResponseCallToAction CallToAction { get; init; }
}

public class GetCensusJourneyApiResponseStatus
{
    public string Name { get; init; }
    public string Label { get; init; }
}

public class GetCensusJourneyApiResponseImportantDates
{
    public string Label { get; init; }
    public DateOnly Date { get; init; }
}

public class GetCensusJourneyApiResponseCallToAction
{
    public string Label { get; init; }
    public Uri Url { get; init; }
}
