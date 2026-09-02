using SchoolAccount.Application.Features.GetCensusJourney;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public static class JourneyViewModelBuilder
{
    public static JourneyViewModel Build(
        string user,
        GetCensusJourneyResponse getCensusJourneyResponse
    )
    {
        return new JourneyViewModel
        {
            User = user,
            CallToActionUrl = getCensusJourneyResponse.CallToAction.Url,
            CallToActionButtonText = getCensusJourneyResponse.CallToAction.ButtonText,
        };
    }
}
