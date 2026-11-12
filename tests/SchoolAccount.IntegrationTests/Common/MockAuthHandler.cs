using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SchoolAccount.TestCommon.Constants;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Signs every request in as a test user. A client can choose the user with the
/// <see cref="ClaimsHeader"/>; otherwise the claims come from <see cref="MockAuthClaimsOptions"/>,
/// or the default single school user.
/// </summary>
public class MockAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    MockAuthClaimsOptions claimsOptions
) : SignOutAuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string ClaimsHeader = "X-Test-Claims";
    public const string FakeGivenName = "Test user";
    public const string FakeFamilyName = "Test surname";
    private const string FakeSub = "1159ee82-d515-4d34-b28d-ac138cb1506b";
    private const string FakeEmail = "test@example.com";

    /// <summary>
    /// The default test user, optionally belonging to a different organisation.
    /// </summary>
    public static IReadOnlyList<Claim> DefaultClaims(
        string organisationJson = OrganisationConstants.SingleSchoolJson
    )
    {
        return
        [
            new Claim(GivenName, FakeGivenName),
            new Claim(FamilyName, FakeFamilyName),
            new Claim(Sub, FakeSub),
            new Claim(Email, FakeEmail),
            new Claim(Organisation, organisationJson),
        ];
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claimsJson = Request.Headers[ClaimsHeader].FirstOrDefault();

        var claims = claimsJson is not null
            ? JsonSerializer
                .Deserialize<Dictionary<string, string>>(claimsJson)!
                .Select(claim => new Claim(claim.Key, claim.Value))
                .ToList()
            : claimsOptions.Claims ?? DefaultClaims();

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
