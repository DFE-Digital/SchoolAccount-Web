using System.Globalization;
using SchoolAccount.Application.Features.GetCensusJourney;

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
            Title = getCensusJourneyResponse.Title,
            Caption = getCensusJourneyResponse.Caption,
            Overview = getCensusJourneyResponse.Overview,
            Status = getCensusJourneyResponse.Status,
            ImportantDates = getCensusJourneyResponse
                .ImportantDates.Select(date => new ImportantDate
                {
                    Label = date.Label,
                    FormattedDate = date.Date.ToString("d MMMM yyyy", CultureInfo.InvariantCulture),
                })
                .ToList(),
            CallToAction = getCensusJourneyResponse.CallToAction,
        };
    }
}
