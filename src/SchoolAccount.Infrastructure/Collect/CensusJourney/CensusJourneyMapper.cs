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
            Title = content.Title,
            Caption = content.Caption,
            Overview = content.Overview,
            Status = new GetCensusJourneyResponseStatus
            {
                Name = content.Status.Name,
                Label = content.Status.Label,
            },
            ImportantDates = content
                .ImportantDates.Select(importantDate => new GetCensusJourneyResponseImportantDates
                {
                    Label = importantDate.Label,
                    Date = importantDate.Date,
                })
                .ToList(),
            CallToAction = new GetCensusJourneyResponseCallToAction
            {
                Url = content.CallToAction.Url,
                Label = content.CallToAction.Label,
            },
        };
    }
}
