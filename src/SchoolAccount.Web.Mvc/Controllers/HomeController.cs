using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Models;

namespace SchoolAccount.Web.Mvc.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> IndexAsync(
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromServices]
            IQueryHandler<
            GetTimeSpecificHelloQuery,
            GetTimeSpecificHelloResponse
        > getTimeSpecifyHellosQueryHandler,
        CancellationToken cancellationToken
    )
    {
        Result<GetTimeSpecificHelloResponse> model = await getTimeSpecifyHellosQueryHandler.Handle(
            new GetTimeSpecificHelloQuery(),
            cancellationToken
        );
        return model.IsSuccess
            ? View(new HomeViewModel(model.Value.Message))
            : Problem(model.Error.Description);
    }
}
