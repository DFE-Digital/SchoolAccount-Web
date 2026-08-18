using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Common;

public class MockAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : SignOutAuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string FakeGivenName = "Test user";
    public const string FakeFamilyName = "Test surname";
    public const string FakeOrganisationName = "Test School";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(GivenName, FakeGivenName),
            new Claim(FamilyName, FakeFamilyName),
            new Claim(Organisation, $$"""{"name":"{{FakeOrganisationName}}"}"""),
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
