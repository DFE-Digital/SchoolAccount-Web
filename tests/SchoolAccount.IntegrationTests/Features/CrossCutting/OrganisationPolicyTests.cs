using System.Net;
using System.Security.Claims;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class OrganisationPolicyTests(AuthorisedWebApplicationFactory factory)
    : IClassFixture<AuthorisedWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClientWithDefaults();

    [Theory]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("")]
    [InlineData(null)]
    public async Task User_is_Forbidden_when_Organisation_Claim_is_empty(string? value)
    {
        factory.SignedInUser.Claims = ClaimsWithOrganisation(value);

        var response = await _client.GetAsync(
            factory.GeneratePath("Dashboard", "Dashboard"),
            TestContext.Current.CancellationToken
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task User_is_accepted_when_organisation_is_not_empty()
    {
        var response = await _client.GetAsync(
            factory.GeneratePath("Dashboard", "Dashboard"),
            TestContext.Current.CancellationToken
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    private static List<Claim> ClaimsWithOrganisation(string? organisationValue)
    {
        var claims = new List<Claim>
        {
            new(GivenName, "Test user"),
            new(FamilyName, "Test surname"),
        };
        if (organisationValue is not null)
        {
            claims.Add(new Claim(Organisation, organisationValue));
        }

        return claims;
    }
}
