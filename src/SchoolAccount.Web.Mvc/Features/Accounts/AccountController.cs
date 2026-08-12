using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Accounts;

[AllowAnonymous]
public class AccountController : Controller
{
    [HttpPost]
    public IActionResult Login(Uri? returnUrl = null)
    {
        returnUrl ??= new Uri(
            Url.Action("Dashboard", "Dashboard")
                ?? throw new AggregateException("Return Url cannot be automatically determined"),
            UriKind.Relative
        );

        if (!Url.IsLocalUrl(returnUrl.ToString()))
        {
            return ValidationProblem();
        }

        return Challenge(
            new AuthenticationProperties { RedirectUri = returnUrl.ToString() },
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    [HttpPost]
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
    public IActionResult LoggedOut()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Start", "Start");
    }
}
