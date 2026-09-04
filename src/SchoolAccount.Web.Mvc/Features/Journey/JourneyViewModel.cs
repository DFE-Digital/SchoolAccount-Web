using System.Diagnostics.CodeAnalysis;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public string? User { get; init; } = "Unknown";

    public Uri CallToActionUrl { get; init; }

    public string CallToActionButtonText { get; init; }

    public StepByStepViewModelCollection? Steps { get; init; }

    [MemberNotNullWhen(true, nameof(Steps))]
    public bool HasSteps => Steps is not null;
}
