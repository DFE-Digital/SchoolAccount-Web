using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SchoolAccount.IntegrationTests.Common;

public class MockAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : SignOutAuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string FakeGivenName = "Test user";
    public const string FakeFamilyName = "Test surname";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("given_name", FakeGivenName),
            new Claim("family_name", FakeFamilyName),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties? properties)
    {
        Response.Redirect(properties?.RedirectUri ?? "/");
        return Task.CompletedTask;
    }

    protected override Task HandleSignOutAsync(AuthenticationProperties? properties)
    {
        return Task.CompletedTask;
    }
}
