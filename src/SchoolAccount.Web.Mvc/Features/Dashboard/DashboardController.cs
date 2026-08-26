using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Collect.CensusStatus;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

[Route("/{action}"), Authorize]
public class DashboardController(IUserContext userContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Dashboard(
        [FromServices]
            IQueryHandler<
            GetCensusStatusQuery,
            List<GetCensusStatusResponse>
        > getCensusStatusQueryHandler,
        CancellationToken cancellationToken
    )
    {
        List<string> censusGreetings;

        if (
            !string.IsNullOrEmpty(userContext.Id)
            && !string.IsNullOrEmpty(userContext.EmailAddress)
            && userContext.Organisation is not null
        )
        {
            var censusStatusResult = await getCensusStatusQueryHandler.Handle(
                new GetCensusStatusQuery(
                    new GetCensusStatusRequestModel
                    {
                        Id = userContext.Id,
                        Email = userContext.EmailAddress,
                        Organisations = [userContext.Organisation],
                    }
                ),
                cancellationToken
            );

            if (censusStatusResult.IsFailure)
            {
                return Problem(censusStatusResult.Error.Description);
            }

            censusGreetings = censusStatusResult
                .Value.SelectMany(a => a.Actions.Select(x => $"{x.Name}, {x.Status.Name}"))
                .ToList();
        }
        else
        {
            return Problem("User property is missing");
        }

        var model = new DashboardViewModel(userContext.Name ?? "Unknown", censusGreetings);
        return View(model);
    }
}
