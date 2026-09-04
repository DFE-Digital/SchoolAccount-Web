namespace SchoolAccount.Application.Features.GetCensusJourney;

public record GetCensusJourneyResponse
{
    public string Title { get; init; }
    public string Caption { get; init; }
    public string Overview { get; init; } = string.Empty;
    public string Status { get; init; }
    public IReadOnlyList<ImportantDate> ImportantDates { get; init; } = [];
    public List<StepByStep> StepByStep { get; init; } = [];
    public CallToAction CallToAction { get; init; }
}

public class ImportantDate
{
    public string Label { get; init; }
    public DateOnly Date { get; init; }
}

public record StepByStep
{
    public string Title { get; init; }
    public string Body { get; init; }
    public bool IsOpen { get; init; }
    public StepByStepStatus? Status { get; init; }
}

public record StepByStepStatus
{
    public string Label { get; init; }
    public string? Colour { get; init; }
}

public record CallToAction
{
    public Uri Url { get; init; }
    public string Label { get; init; }
}
