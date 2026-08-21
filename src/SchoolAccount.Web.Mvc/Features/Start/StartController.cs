using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Start;

[Route("/")]
public class StartController : Controller
{
    [HttpGet(""), AllowAnonymous]
    public async Task<IActionResult> Start()
    {
        return View(new StartViewModel());
    }
}
