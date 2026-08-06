using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Error;

[Route(RouteConstants.Error.Index)]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    [Route(RouteConstants.Error.StatusCode)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCode(HttpStatusCode statusCode)
    {
        logger.LogWarning(
            "HTTP {StatusCode} error occurred at {Path}",
            statusCode,
            HttpContext.Request.Path
        );

        var errorViewModel = new ErrorViewModel(statusCode);

        return View(errorViewModel);
    }
}
