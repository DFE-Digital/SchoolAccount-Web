using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SchoolAccount.Web.Mvc.Authentication.Handlers;

namespace SchoolAccount.IntegrationTests.Authentication.Handlers;

public class OpenIdConnectEventHandlersTests
{
    [Fact]
    public async Task OnTicketReceived_NoOrganisation_Redirects403()
    {
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity()), "oidc");

        var context = new TicketReceivedContext(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(IAuthenticationHandler)),
            new OpenIdConnectOptions(),
            ticket
        );

        await OpenIdConnectEventHandlers.OnTicketReceived(context);

        Assert.Equal(302, context.HttpContext.Response.StatusCode);
        Assert.Equal("/error/403", context.HttpContext.Response.Headers.Location);
        Assert.True(context.Result?.Handled);
    }

    [Fact]
    public async Task OnTicketReceived_WithOrganisation_DoesNotRedirect()
    {
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim("organisation", """{"NAME":"Test School"}"""));

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), "oidc");

        var context = new TicketReceivedContext(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(IAuthenticationHandler)),
            new OpenIdConnectOptions(),
            ticket
        );

        await OpenIdConnectEventHandlers.OnTicketReceived(context);

        Assert.NotEqual(302, context.HttpContext.Response.StatusCode);
        Assert.Null(context.Result);
    }
}
