using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.SharedKernel;
using static SchoolAccount.Web.Mvc.RouteConstants;

namespace SchoolAccount.Web.Mvc.Features.Start;

[Route(Root)]
public class StartController : Controller
{
    [HttpGet(""), AllowAnonymous]
    public async Task<IActionResult> Start(
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromServices]
            IQueryHandler<
            GetTimeSpecificHelloQuery,
            GetTimeSpecificHelloResponse
        > getTimeSpecifyHellosQueryHandler,
        CancellationToken cancellationToken
    )
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Dashboard", "Dashboard");
        }

        var model = await getTimeSpecifyHellosQueryHandler.Handle(
            new GetTimeSpecificHelloQuery(),
            cancellationToken
        );

        return model.IsSuccess
            ? View(new StartViewModel(model.Value.Message))
            : Problem(model.Error.Description);
    }
}
