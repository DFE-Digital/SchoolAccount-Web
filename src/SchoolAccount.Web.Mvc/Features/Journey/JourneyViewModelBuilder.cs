using SchoolAccount.Application.Features.GetCensusJourney;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public static class JourneyViewModelBuilder
{
    public static JourneyViewModel Build(
        string user,
        IReadOnlyList<string> censusStatuses,
        GetCensusJourneyResponse getCensusJourneyResponse
    )
    {
        return new JourneyViewModel
        {
            User = user,
            CensusStatuses = censusStatuses,
            CallToActionUrl = getCensusJourneyResponse.CallToAction.Url,
            CallToActionButtonText = getCensusJourneyResponse.CallToAction.ButtonText,
        };
    }
}
