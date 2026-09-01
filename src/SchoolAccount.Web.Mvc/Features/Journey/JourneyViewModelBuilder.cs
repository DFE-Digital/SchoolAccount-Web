using SchoolAccount.Application.Features.GetCensusActions;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public static class JourneyViewModelBuilder
{
    public static JourneyViewModel Build(
        string user,
        IReadOnlyList<string> censusStatuses,
        GetServiceActionsResponse getServiceActionsResponse
    )
    {
        return new JourneyViewModel
        {
            User = user,
            CensusStatuses = censusStatuses,
            CallToActionUrl = getServiceActionsResponse.CallToAction.Url,
            CallToActionButtonText = getServiceActionsResponse.CallToAction.ButtonText,
        };
    }
}
