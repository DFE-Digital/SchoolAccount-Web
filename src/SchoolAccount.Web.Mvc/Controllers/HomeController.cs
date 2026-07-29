using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Trueman.GetTimeSpecifyHellos;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Models;

namespace SchoolAccount.Web.Mvc.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> IndexAsync(
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromServices]
            IQueryHandler<
            GetTimeSpecifyHellosQuery,
            GetTimeSpecifyHellosResponse
        > getTimeSpecifyHellosQueryHandler,
        CancellationToken cancellationToken
    )
    {
        Result<GetTimeSpecifyHellosResponse> model = await getTimeSpecifyHellosQueryHandler.Handle(
            new GetTimeSpecifyHellosQuery(),
            cancellationToken
        );
        return model.IsSuccess
            ? View(new HomeViewModel { GreetingMessage = model.Value.Message })
            : Problem(model.Error.Description);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
