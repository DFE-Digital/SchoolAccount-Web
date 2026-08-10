using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Accounts;

public class AccountController : Controller
{
    [HttpGet]
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

    [HttpGet]
    public IActionResult Logout()
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToAction("Start", "Start");
        }

        HttpContext.Session.Clear();

        return base.SignOut(
            OpenIdConnectDefaults.AuthenticationScheme,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }

    [HttpGet]
    public IActionResult SignedOut()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Start", "Start");
    }
}
