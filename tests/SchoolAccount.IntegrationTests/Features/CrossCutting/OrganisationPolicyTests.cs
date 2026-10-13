using System.Net;
using System.Security.Claims;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class OrganisationPolicyTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IntegrationTest(factory)
{
    private HttpClient CreateClientOrganisationClaims(string? organisationValue)
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

        TestDoubles.Claims = claims;

        return Factory.CreateAuthorisedClient(new ClientOptions { AllowAutoRedirect = false });
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("")]
    [InlineData(null)]
    public async Task User_is_Forbidden_when_Organisation_Claim_is_empty(string? value)
    {
        using var client = CreateClientOrganisationClaims(value);

        var response = await client.GetAsync(
            Factory.GeneratePath("Dashboard", "Dashboard"),
            TestContext.Current.CancellationToken
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task User_is_accepted_when_organisation_is_not_empty()
    {
        using var client = Factory.CreateAuthorisedClient();

        var response = await client.GetAsync(
            Factory.GeneratePath("Dashboard", "Dashboard"),
            TestContext.Current.CancellationToken
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
