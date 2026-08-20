using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Collect.CensusStatus;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

[Route("/{action}"), Authorize]
public class DashboardController(IUserContext userContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Dashboard(
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromServices]
            IQueryHandler<
            GetTimeSpecificHelloQuery,
            GetTimeSpecificHelloResponse
        > getTimeSpecifyHellosQueryHandler,
        [FromServices]
            IQueryHandler<
            GetCensusStatusQuery,
            GetCensusStatusResponse
        > getCensusStatusQueryHandler,
        CancellationToken cancellationToken
    )
    {
        var censusGreeting = string.Empty;

        var result = await getTimeSpecifyHellosQueryHandler.Handle(
            new GetTimeSpecificHelloQuery(),
            cancellationToken
        );

        if (result.IsFailure)
        {
            return Problem(result.Error.Description);
        }

        if (
            userContext.Id is not null
            && userContext.EmailAddress is not null
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

            censusGreeting =
                $"{censusStatusResult.Value.Name}: {censusStatusResult.Value.Status.Name}.";
        }
        else
        {
            return Problem("User property is missing");
        }

        var model = new DashboardViewModel(
            userContext.Name ?? "Unknown",
            result.Value.Message,
            censusGreeting
        );
        return View(model);
    }
}
