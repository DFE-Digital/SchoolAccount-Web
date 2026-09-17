namespace SchoolAccount.Infrastructure.Clients.Collect.CensusJourney;

public class GetCensusJourneyApiResponse
{
    public string Title { get; init; }
    public string Caption { get; init; }
    public string Overview { get; init; } = string.Empty;

    public GetCensusJourneyApiResponseStatus Status { get; init; }

    public IReadOnlyList<GetCensusJourneyApiResponseImportantDate> ImportantDates { get; init; } =
    [];

    public IReadOnlyList<GetCensusJourneyApiUnderstandStatus> UnderstandStatuses { get; init; } =
    [];

    public required GetCensusJourneyApiResponseCallToAction CallToAction { get; init; }
}

public class GetCensusJourneyApiUnderstandStatus
{
    public string Name { get; init; }
    public string Description { get; init; }
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
