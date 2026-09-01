namespace SchoolAccount.Web.Mvc.Features.Journey;

public sealed class JourneyViewModel
{
    public string User { get; init; }
    public IReadOnlyList<string> CensusStatuses { get; init; }
    public string CallToActionUrl { get; init; }
    public string CallToActionButtonText { get; init; }
};
