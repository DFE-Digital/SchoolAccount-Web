using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.SharedKernel;
using static SchoolAccount.Web.Mvc.RouteConstants;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

[Route(Root), Authorize]
public class DashboardController(IUserContext userContext) : Controller
{
    [HttpGet(RouteConstants.Dashboard)]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        return View(new DashboardViewModel(userContext.Name ?? "Unknown"));
    }
}
