using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.Web.Mvc.Features.Shared.MatSchoolsStatusTable;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public string? User { get; init; } = "Unknown";

    public string Title { get; init; }

    public string Caption { get; init; }

    public string? Overview { get; init; }

    public string Status { get; init; }

    public MatSchoolsStatusTableViewModel? MatSchoolsStatuses { get; init; }

    public IReadOnlyList<ImportantDate> ImportantDates { get; init; } = [];

    public StepByStepViewModelCollection? Steps { get; init; }

    public CallToAction CallToAction { get; init; }

    public bool DisplayImportantDates => ImportantDates.Any();

    public bool DisplayOverview => !string.IsNullOrWhiteSpace(Overview);

    public bool TryGetSteps(out StepByStepViewModelCollection steps)
    {
        steps = Steps!;
        return Steps?.HasItems() == true;
    }

    public bool IsMatOrLocalAuthority => MatSchoolsStatuses is not null;
}

public sealed class ImportantDate
{
    public string Label { get; init; }
    public string FormattedDate { get; init; }
}
