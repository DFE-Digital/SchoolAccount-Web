namespace SchoolAccount.Application.Features.GetCensusJourney;

public record GetCensusJourneyResponse
{
    public List<StepByStep> StepByStep { get; init; } = [];
    public CallToAction CallToAction { get; init; }
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
    public string ButtonText { get; init; }
}
