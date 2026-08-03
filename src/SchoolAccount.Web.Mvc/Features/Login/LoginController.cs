using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Login;

[Route("/")]
public class LoginController : Controller
{
    [HttpGet("login"), AllowAnonymous]
    public IActionResult Login(Uri? returnUrl = null)
    {
        returnUrl ??= new Uri(Url.Action("Dashboard", "Dashboard")!, UriKind.Relative);

        if (!Url.IsLocalUrl(returnUrl?.ToString()))
        {
            return Problem();
        }

        return Challenge(
            new AuthenticationProperties { RedirectUri = returnUrl.ToString() },
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }
}
