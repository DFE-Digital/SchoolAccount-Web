namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public string User { get; init; }
    public Uri CallToActionUrl { get; init; }
    public string CallToActionButtonText { get; init; }
}
