using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Controllers;

[Route("Error")]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    [Route("{statusCode:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ErrorStatusCode(int statusCode)
    {
        logger.LogWarning("HTTP {StatusCode} error occurred at {Path}",
            statusCode, HttpContext.Request.Path);

        return View("StatusCode", statusCode);
    }
}
