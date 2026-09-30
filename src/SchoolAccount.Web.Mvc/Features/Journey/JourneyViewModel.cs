using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public string? User { get; init; } = "Unknown";

    public string Title { get; init; }

    public string Caption { get; init; }

    public string? Overview { get; init; }

    public string Status { get; init; }

    public MultipleSchoolsStatusTableViewModel? MatSchoolsStatuses { get; init; }

    public IReadOnlyList<ImportantDate> ImportantDates { get; init; } = [];

    public IReadOnlyList<UnderstandStatus> UnderstandStatuses { get; init; } = [];

    public StepByStepViewModelCollection? Steps { get; init; }

    public SupportService SupportService { get; init; }

    public CallToAction CallToAction { get; init; }

    public bool DisplayImportantDates => ImportantDates.Any();

    public bool DisplayOverview => !string.IsNullOrWhiteSpace(Overview);

    public bool DisplayUnderstandStatuses =>
        UnderstandStatuses.Any() && (Status != "Unavailable" || IsMatOrLocalAuthority);

    public bool TryGetSteps(out StepByStepViewModelCollection steps)
    {
        steps = Steps!;
        return Steps?.HasItems() == true;
    }

    public bool IsMatOrLocalAuthority =>
        MatSchoolsStatuses is not null && MatSchoolsStatuses.SchoolStatuses.Any();
}

public sealed class ImportantDate
{
    public string Label { get; init; }
    public string FormattedDate { get; init; }
}

public sealed class UnderstandStatus
{
    public string Name { get; init; }

    public string Description { get; init; }
}

public sealed class SupportService
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Uri Url { get; init; }
}

public sealed class CallToAction
{
    public Uri Url { get; init; }

    public string Label { get; init; }
}
