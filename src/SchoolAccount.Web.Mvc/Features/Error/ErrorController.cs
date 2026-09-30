using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Error;

[Route("/{action}"), AllowAnonymous]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    [Route("{statusCode}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [SuppressMessage(
        "Security",
        "CA5395:Miss HttpVerb attribute for action methods",
        Justification = "Status code re-execution preserves the original request method; this action is read-only."
    )]
    public IActionResult Error(HttpStatusCode statusCode)
    {
        logger.LogWarning(
            "HTTP {StatusCode} error occurred at {Path}",
            statusCode,
            HttpContext.Request.Path
        );

        var errorViewModel = new ErrorViewModel(statusCode);
        Response.StatusCode = (int)errorViewModel.StatusCode;

        return View(errorViewModel);
    }
}
