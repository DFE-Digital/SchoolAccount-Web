using SchoolAccount.Application.Features.GetCensusJourney;
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

    public static GetCensusJourneyResponse ToResponse(GetCensusJourneyApiResponse content)
    {
        return new GetCensusJourneyResponse
        {
            CallToAction = new CallToAction
            {
                Url = content.CallToAction.Url,
                ButtonText = content.CallToAction.Label,
            },
        };
    }
}
