using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.Application.Services.GetServiceActionUrl;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

[Route("/{action}"), Authorize]
public class DashboardController(
    IUserContext userContext,
    IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>> getCensusStatusesHandler,
    IQueryHandler<GetServiceActionsQuery, GetServiceActionsResponse> getServiceActionsHandler,
    ILogger<DashboardController> logger
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        if (
            string.IsNullOrEmpty(userContext.Id)
            && string.IsNullOrEmpty(userContext.EmailAddress)
            && userContext.Organisation is null
        )
        {
            throw new ArgumentException("Invalid user context");
        }

        var query = new GetCensusStatusesQuery
        {
            Id = userContext.Id!,
            EmailAddress = userContext.EmailAddress!,
            Organisations = [userContext.Organisation!],
        };

        var censusStatusesResult = await getCensusStatusesHandler.Handle(query, cancellationToken);

        if (censusStatusesResult.IsFailure)
        {
            logger.LogError(
                "Census statuses could not be fetched: {ErrorCode} {ErrorDescription}",
                censusStatusesResult.Error.Code,
                censusStatusesResult.Error.Description
            );

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var serviceActionsQuery = new GetServiceActionsQuery();

        var serviceActionsResult = await getServiceActionsHandler.Handle(
            serviceActionsQuery,
            cancellationToken
        );

        var censusStatuses = censusStatusesResult
            .Value.SelectMany(a => a.Actions.Select(x => $"{x.Name}, {x.Status.Name}"))
            .ToList();

        var dashboardViewModel = DashboardViewModelBuilder.Build(
            userContext.Name ?? "Unknown",
            censusStatuses,
            serviceActionsResult.Value
        );

        return View(dashboardViewModel);
    }
}
