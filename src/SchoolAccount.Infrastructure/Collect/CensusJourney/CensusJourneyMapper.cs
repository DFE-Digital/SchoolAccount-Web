using SchoolAccount.Application.Features.GetCensusActions;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Infrastructure.Collect.CensusJourney;

public static class CensusJourneyMapper
{
    public static GetCensusJourneyApiRequest ToApiRequest(
        string id,
        string emailAddress,
        IReadOnlyList<Organisation> organisations
    )
    {
        return new GetCensusJourneyApiRequest
        {
            Id = id,
            Email = emailAddress,
            Organisations = organisations,
        };
    }

    public static GetServiceActionsResponse ToResponse(CallToActionApiResponse content)
    {
        return new GetServiceActionsResponse
        {
            CallToAction = new CallToAction
            {
                Url = content.CallToActionUrl,
                ButtonText = content.CallToActionButtonText,
            },
        };
    }
}
