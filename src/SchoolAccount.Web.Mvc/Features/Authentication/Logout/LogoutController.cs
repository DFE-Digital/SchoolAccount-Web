using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using static SchoolAccount.Web.Mvc.RouteConstants;

namespace SchoolAccount.Web.Mvc.Features.Authentication.Logout;

[Route(Account.Index)]
public class LogoutController : Controller
{
    [HttpGet(Account.Logout)]
    public IActionResult Logout(Uri? returnUrl = null)
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToAction("Start", "Start");
        }

        HttpContext.Session.Clear();

        return base.SignOut(OpenIdConnectDefaults.AuthenticationScheme);
    }
}
