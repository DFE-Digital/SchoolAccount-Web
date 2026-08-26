using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

[Route("/{action}"), Authorize]
public class DashboardController(
    IUserContext userContext,
    IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>> getCensusStatusesHandler
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

        var censusStatusesResult = await getCensusStatusesHandler.Handle(
            new GetCensusStatusesQuery(
                new GetCensusStatusesRequest
                {
                    Id = userContext.Id!,
                    Email = userContext.EmailAddress!,
                    Organisations = [userContext.Organisation!],
                }
            ),
            cancellationToken
        );

        if (censusStatusesResult.IsFailure)
        {
            return Problem(censusStatusesResult.Error.Description);
        }

        var censusStatuses = censusStatusesResult
            .Value.SelectMany(a => a.Actions.Select(x => $"{x.Name}, {x.Status.Name}"))
            .ToList();

        var model = new DashboardViewModel(userContext.Name ?? "Unknown", censusStatuses);

        return View(model);
    }
}
