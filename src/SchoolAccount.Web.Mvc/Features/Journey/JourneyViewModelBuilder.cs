using System.Globalization;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.Web.Mvc.Features.Shared.MatSchoolsStatusTable;
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
            Title = getCensusJourneyResponse.Content.Title,
            Caption = getCensusJourneyResponse.Content.Caption,
            Overview = getCensusJourneyResponse.Content.Overview,
            Status = getCensusJourneyResponse.Content.Status,
            ImportantDates = getCensusJourneyResponse
                .Content.ImportantDates.OrderBy(date => date.Date)
                .Select(date => new ImportantDate
                {
                    Label = date.Label,
                    FormattedDate = date.Date.ToString("d MMMM yyyy", CultureInfo.InvariantCulture),
                })
                .ToList(),
            CallToAction = getCensusJourneyResponse.Content.CallToAction,
            Steps = StepByStepViewModelCollection
                .Create("Journey:StepByStep")
                .AddSteps(getCensusJourneyResponse.Content.StepByStep),
            MatSchoolsStatuses = new MatSchoolsStatusTableViewModel
            {
                SchoolStatuses = getCensusJourneyResponse
                    .SchoolStatuses.SelectMany(a =>
                        a.Actions.Select(x => new SchoolStatus
                        {
                            Name = x.Name,
                            Status = x.Status.Name,
                        })
                    )
                    .ToList(),
            },
            //.RememberSteps(),
        };
    }
}
