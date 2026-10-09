using System.Globalization;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;
using SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;
using SchoolAccount.Web.Mvc.Helpers;
using static SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable.MultipleSchoolsStatusTableViewModel;

namespace SchoolAccount.Web.Mvc.Features.Journey;

public class JourneyViewModelBuilder(IDateTimeProvider dateTimeProvider)
{
    private readonly DateFormatter dateFormatter = new(dateTimeProvider);

    public JourneyViewModel Build(
        string? user,
        GetCensusJourneyResponse getCensusJourneyResponse,
        Organisation organisation
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
                    FormattedDate = dateFormatter.ToDayMonthNameAndYear(date.Date),
                })
                .ToList(),
            UnderstandStatuses = getCensusJourneyResponse
                .Content.UnderstandStatuses.Select(understandStatus => new UnderstandStatus
                {
                    Name = understandStatus.Name,
                    Description = understandStatus.Description,
                })
                .ToList(),
            SupportService = new SupportService
            {
                Title = getCensusJourneyResponse.Content.SupportService.Title,
                Description = getCensusJourneyResponse.Content.SupportService.Description,
                Url = getCensusJourneyResponse.Content.SupportService.Url,
            },
            CallToAction = new CallToAction
            {
                Label = getCensusJourneyResponse.Content.CallToAction.Label,
                Url = getCensusJourneyResponse.Content.CallToAction.Url,
            },
            Steps = StepByStepViewModelCollection
                .Create("Journey:StepByStep")
                .AddSteps(getCensusJourneyResponse.Content.StepByStep),
            MatSchoolsStatuses = !organisation.IsEstablishment
                ? new MultipleSchoolsStatusTableViewModel
                {
                    SchoolStatuses = getCensusJourneyResponse
                        .SchoolStatuses.SelectMany(a =>
                            a.Actions.Select(x => new SchoolStatus
                            {
                                Name = a.SchoolName,
                                Status = x.Status.Name,
                                Errors = x.Errors,
                                Queries = x.Queries,
                                UpdatedAt = x.UpdatedAt is null
                                    ? string.Empty
                                    : dateFormatter.ToDaysAgo(x.UpdatedAt.Value),
                            })
                        )
                        .ToList(),
                }
                : null,
        };
    }
}
