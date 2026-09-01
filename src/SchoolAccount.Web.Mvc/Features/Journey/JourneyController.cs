using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.Application.Features.GetCensusActions;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Features.Dashboard;

namespace SchoolAccount.Web.Mvc.Features.Journey;

[Route("/{action}"), Authorize]
public class JourneyController(
    IUserContext userContext,
    IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>> getCensusStatusesHandler,
    IQueryHandler<GetServiceActionsQuery, GetServiceActionsResponse> getServiceActionsHandler,
    ILogger<JourneyController> logger
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Journey(CancellationToken cancellationToken)
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

        var journeyViewModel = JourneyViewModelBuilder.Build(
            userContext.Name ?? "Unknown",
            censusStatuses,
            serviceActionsResult.Value
        );

        return View(journeyViewModel);
    }
}
