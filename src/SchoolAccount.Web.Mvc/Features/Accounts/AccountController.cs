using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Accounts;

[AllowAnonymous]
public class AccountController(IUserContext userContext) : Controller
{
    [HttpGet]
    public IActionResult Login(Uri? returnUrl = null)
    {
        var defaultRedirect = Url.Action("Journey", "Journey");

        if (defaultRedirect is null)
        {
            var message = "A return url cannot be automatically determined";
            throw new ArgumentException(message, nameof(returnUrl));
        }

        returnUrl ??= new Uri(defaultRedirect, UriKind.Relative);

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
        if (!userContext.IsAuthenticated)
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
