using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Start;

[Route("/")]
public class StartController : Controller
{
    [HttpGet(""), AllowAnonymous]
    public async Task<IActionResult> Start()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Dashboard", "Dashboard");
        }

        return View(new StartViewModel());
    }
}
