using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.Application.Services.GetServiceActionUrl;
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
            GetServiceActionsQuery,
            GetServiceActionsResponse
        > getServiceActionsQueryHandler,
        CancellationToken cancellationToken
    )
    {
        var result = await getTimeSpecifyHellosQueryHandler.Handle(
            new GetTimeSpecificHelloQuery(),
            cancellationToken
        );

        var getServiceActionsResult = await getServiceActionsQueryHandler.Handle(
            new GetServiceActionsQuery(),
            cancellationToken
        );

        if (result.IsFailure)
        {
            return Problem(result.Error.Description);
        }

        var model = new DashboardViewModel(userContext.Name ?? "Unknown", result.Value.Message);
        return View(model);
    }
}
