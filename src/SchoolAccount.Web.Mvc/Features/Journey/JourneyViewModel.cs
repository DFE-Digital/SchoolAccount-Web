using System.Diagnostics.CodeAnalysis;
using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public string? User { get; init; } = "Unknown";

    public string Title { get; init; }

    public string Caption { get; init; }

    public string? Overview { get; init; }

    public string Status { get; init; }

    public IReadOnlyList<ImportantDate> ImportantDates { get; init; } = [];

    public StepByStepViewModelCollection? Steps { get; init; }

    [MemberNotNullWhen(true, nameof(Steps))]
    public bool HasSteps => Steps is not null;

    public CallToAction CallToAction { get; init; }

    public bool DisplayImportantDates => ImportantDates.Any();

    public bool DisplayOverview => !string.IsNullOrWhiteSpace(Overview);
}

public sealed class ImportantDate
{
    public string Label { get; init; }
    public string FormattedDate { get; init; }
}
