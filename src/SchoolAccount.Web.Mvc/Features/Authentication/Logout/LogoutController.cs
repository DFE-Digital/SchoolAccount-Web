using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Authentication.Logout;

[Route("/")]
public class LogoutController : Controller
{
    [HttpGet("logout"), AllowAnonymous]
    public IActionResult Logout(Uri? returnUrl = null)
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
