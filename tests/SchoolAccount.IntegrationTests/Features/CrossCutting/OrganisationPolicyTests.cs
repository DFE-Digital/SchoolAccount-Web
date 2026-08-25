using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAccount.IntegrationTests.Common;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class OrganisationPolicyTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private HttpClient ClientWithOrganisation(string? organisationValue)
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

        return factory.CreateAuthorisedClient(
            services =>
            {
                services.RemoveAll<MockAuthClaimsOptions>();
                services.AddSingleton(new MockAuthClaimsOptions { Claims = claims });
            },
            new ClientOptions { AllowAutoRedirect = false }
        );
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData(" {} ")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Forbidden_When_Organisation_Empty(string value)
    {
        var client = ClientWithOrganisation(value);

        var response = await client.GetAsync(
            factory.GeneratePath("Dashboard", "Dashboard"),
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Forbidden_When_Organisation_Claim_Missing()
    {
        var client = ClientWithOrganisation(null);

        var response = await client.GetAsync(
            factory.GeneratePath("Dashboard", "Dashboard"),
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("{\"name\":\"Acme\"}")]
    [InlineData("{\"id\":\"abc-123\"}")]
    public async Task Ok_When_Organisation_Populated(string value)
    {
        var client = ClientWithOrganisation(value);

        var response = await client.GetAsync(
            factory.GeneratePath("Dashboard", "Dashboard"),
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
