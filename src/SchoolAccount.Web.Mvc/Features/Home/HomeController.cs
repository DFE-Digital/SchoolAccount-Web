using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Home;

[Route("/")]
public class HomeController : Controller
{
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> Home(
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromServices]
            IQueryHandler<
            GetTimeSpecificHelloQuery,
            GetTimeSpecificHelloResponse
        > getTimeSpecifyHellosQueryHandler,
        CancellationToken cancellationToken
    )
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Dashboard", "Dashboard");
        }

        var model = await getTimeSpecifyHellosQueryHandler.Handle(
            new GetTimeSpecificHelloQuery(),
            cancellationToken
        );

        return model.IsSuccess
            ? View(new HomeViewModel(model.Value.Message))
            : Problem(model.Error.Description);
    }
}
