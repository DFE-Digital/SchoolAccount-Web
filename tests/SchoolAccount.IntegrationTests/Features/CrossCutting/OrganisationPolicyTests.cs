using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.TestCommon.Constants;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class OrganisationPolicyTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
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

        return factory.CreateAuthorisedClient(
            services =>
            {
                services.RemoveAll<MockAuthClaimsOptions>();
                services.AddSingleton(new MockAuthClaimsOptions { Claims = claims });
                services.StubQueryHandler(StubCensusJourneyHandler.Succeeding());
            },
            new ClientOptions { AllowAutoRedirect = false }
        );
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("")]
    [InlineData(null)]
    public async Task User_is_Forbidden_when_Organisation_Claim_is_empty(string? value)
    {
        // Arrange
        var client = CreateClientOrganisationClaims(value);

        // Act
        var response = await client.GetAsync(
            UrlConstants.JourneyUrl,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task User_is_accepted_when_organisation_is_not_empty()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
        );

        // Act
        var response = await client.GetAsync(
            UrlConstants.JourneyUrl,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
