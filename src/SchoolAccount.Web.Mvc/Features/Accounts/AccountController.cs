using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static SchoolAccount.Web.Mvc.RouteConstants;

namespace SchoolAccount.Web.Mvc.Features.Accounts;

[Route(Account.Index), AllowAnonymous]
public class AccountController : Controller
{
    [HttpGet(Account.Login)]
    public IActionResult Login(Uri? returnUrl = null)
    {
        returnUrl ??= new Uri(Url.Action("Dashboard", "Dashboard")!, UriKind.Relative);

        if (!Url.IsLocalUrl(returnUrl?.ToString()))
        {
            return ValidationProblem();
        }

        return Challenge(
            new AuthenticationProperties { RedirectUri = returnUrl.ToString() },
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    [HttpPost(Account.Logout)]
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
