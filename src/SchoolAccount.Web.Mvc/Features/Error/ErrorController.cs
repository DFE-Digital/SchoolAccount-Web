using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Error;

[Route("/{action}"), AllowAnonymous]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    [HttpGet("{statusCode}"), AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(HttpStatusCode statusCode)
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
