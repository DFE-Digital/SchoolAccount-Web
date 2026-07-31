using System.Net;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Web.Mvc.Models;

namespace SchoolAccount.Web.Mvc.Controllers;

[Route("Error")]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    [Route("{statusCode}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ErrorStatusCode(HttpStatusCode statusCode)
    {
        logger.LogWarning(
            "HTTP {StatusCode} error occurred at {Path}",
            statusCode,
            HttpContext.Request.Path
        );

        var errorViewModel = new ErrorViewModel(statusCode);

        return View("StatusCode", errorViewModel);
    }
}
