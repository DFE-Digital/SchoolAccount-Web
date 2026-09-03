using SchoolAccount.Application.Features.GetCensusJourney;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public string? User { get; init; } = "Unknown";
    public string Title { get; init; }
    public string Caption { get; init; }
    public string? Overview { get; init; }
    public GetCensusJourneyResponseStatus Status { get; init; }
    public IReadOnlyList<ImportantDate> ImportantDates { get; init; } = [];
    public GetCensusJourneyResponseCallToAction CallToAction { get; init; }
    public bool DisplayImportantDates => ImportantDates.Any();
    public bool DisplayOverview => !string.IsNullOrWhiteSpace(Caption);
}

public sealed class ImportantDate
{
    public string Label { get; init; }
    public string FormattedDate { get; init; }
}
