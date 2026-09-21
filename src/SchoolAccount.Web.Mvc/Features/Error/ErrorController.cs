using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Error;

[Route("/{action}"), AllowAnonymous]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    /// <summary>
    /// Responds to every verb, not just GET. Both <c>UseExceptionHandler</c> and
    /// <c>UseStatusCodePagesWithReExecute</c> re-execute this action with the original request's
    /// method, so a GET-only error page turns any failed POST — such as the OIDC form_post
    /// callback — into a 405 that masks the real status code.
    /// </summary>
    [Route("{statusCode}"), AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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
