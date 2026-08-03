using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

[Route("/"), Authorize]
public class DashboardController(IUserContext userContext) : Controller
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        return View(new DashboardViewModel(userContext.Name ?? "Unknown"));
    }
}
