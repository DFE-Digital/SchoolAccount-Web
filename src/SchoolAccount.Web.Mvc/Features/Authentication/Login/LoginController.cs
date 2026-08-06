using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static SchoolAccount.Web.Mvc.RouteConstants;

namespace SchoolAccount.Web.Mvc.Features.Authentication.Login;

[Route(Account.Index)]
public class LoginController : Controller
{
    [HttpGet(Account.Login), AllowAnonymous]
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
}
