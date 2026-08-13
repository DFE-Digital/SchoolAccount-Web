using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Home;

[Route("/")]
public class HomeController : Controller
{
    [HttpGet(""), AllowAnonymous]
    public async Task<IActionResult> Home()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Dashboard", "Dashboard");
        }

        return View(new HomeViewModel());
    }
}
