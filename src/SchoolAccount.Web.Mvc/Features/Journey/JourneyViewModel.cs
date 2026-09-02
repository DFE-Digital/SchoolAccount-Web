namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public Uri CallToActionUrl { get; init; }
    public string CallToActionButtonText { get; init; }

    public string? User
    {
        get;
        init => field = value ?? "Unknown";
    } = "Unknown";
}
