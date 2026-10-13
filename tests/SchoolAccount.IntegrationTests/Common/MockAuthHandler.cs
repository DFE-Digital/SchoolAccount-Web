using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Signs every request in as a test user. The user's claims come from the request's
/// <see cref="ClaimsHeader"/> when a client sets one, so each client carries its own user.
/// </summary>
public class MockAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : SignOutAuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string ClaimsHeader = "X-Test-Claims";
    public const string FakeGivenName = "Test user";
    public const string FakeFamilyName = "Test surname";
    private const string FakeSub = "1159ee82-d515-4d34-b28d-ac138cb1506b";
    private const string FakeEmail = "test@example.com";
    public const string FakeOrganisationName = "Test School";

    private const string DefaultOrganisationJson = $$"""
        {
         "id": "2E774B32-E4DB-445B-B915-736C777FF5A4",
         "name": "{{FakeOrganisationName}}",
         "category": {
             "id": "001",
             "name": "Establishment"
         },
         "ukprn": "10037611",
         "establishmentNumber": "2091",
         "localAuthority": {
             "id": "502EF2E9-2CA6-4905-9BF7-E80695BD5717",
             "name": "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL",
             "code": "394"
         }
        }
        """;

    /// <summary>
    /// The default test user, optionally belonging to a different organisation.
    /// </summary>
    public static IReadOnlyList<Claim> DefaultClaims(
        string organisationJson = DefaultOrganisationJson
    ) =>
        [
            new Claim(GivenName, FakeGivenName),
            new Claim(FamilyName, FakeFamilyName),
            new Claim(Sub, FakeSub),
            new Claim(Email, FakeEmail),
            new Claim(Organisation, organisationJson),
        ];

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claimsJson = Request.Headers[ClaimsHeader].FirstOrDefault();
        var claims = claimsJson is null
            ? DefaultClaims()
            : JsonSerializer
                .Deserialize<Dictionary<string, string>>(claimsJson)!
                .Select(claim => new Claim(claim.Key, claim.Value))
                .ToList();

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
