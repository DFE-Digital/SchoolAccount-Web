using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SchoolAccount.TestCommon.Constants;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Common;

public class MockAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    MockAuthClaimsOptions claimsOptions
) : SignOutAuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string FakeGivenName = "Test user";
    public const string FakeFamilyName = "Test surname";
    private const string FakeSub = "1159ee82-d515-4d34-b28d-ac138cb1506b";
    private const string FakeEmail = "test@example.com";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var organisationJson = OrganisationConstants.ValidAcademyJson;

        var claims =
            claimsOptions.Claims?.ToArray()
            ??
            [
                new Claim(GivenName, FakeGivenName),
                new Claim(FamilyName, FakeFamilyName),
                new Claim(Sub, FakeSub),
                new Claim(Email, FakeEmail),
                new Claim(Organisation, organisationJson),
            ];

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
