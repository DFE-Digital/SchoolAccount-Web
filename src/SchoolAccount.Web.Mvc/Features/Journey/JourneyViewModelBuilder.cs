using SchoolAccount.Application.Extensions;
using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public static class JourneyViewModelBuilder
{
    public static JourneyViewModel Build(
        string? user,
        GetCensusJourneyResponse getCensusJourneyResponse
    )
    {
        return new JourneyViewModel
        {
            User = user,
            CallToActionUrl = getCensusJourneyResponse.CallToAction.Url,
            CallToActionButtonText = getCensusJourneyResponse.CallToAction.ButtonText,
            Steps = StepByStepViewModelCollection
                .Create("Journey:StepByStep")
                .AddSteps(getCensusJourneyResponse.StepByStep),
            //.RememberSteps(),
        };
    }
}
