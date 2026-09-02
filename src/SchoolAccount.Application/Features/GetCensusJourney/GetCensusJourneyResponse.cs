namespace SchoolAccount.Application.Features.GetCensusJourney;

public record GetCensusJourneyResponse
{
    public StepByStepCollection StepByStep { get; init; } = [];
    public CallToAction CallToAction { get; init; }
}

public class StepByStepCollection : List<StepByStep>;

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
