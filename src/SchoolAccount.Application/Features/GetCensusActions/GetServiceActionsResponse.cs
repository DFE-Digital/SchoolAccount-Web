namespace SchoolAccount.Application.Features.GetCensusActions;

public record GetServiceActionsResponse
{
    public CallToAction CallToAction { get; init; }
}

public record CallToAction
{
    public string Url { get; init; }
    public string ButtonText { get; init; }
}
