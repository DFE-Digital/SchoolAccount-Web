using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Stubs;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;
using static Xunit.TestContext;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class OrganisationPolicyTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
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
        var response = await client.GetAsync(JourneyUrl, Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Forbidden);
    }

    [Fact]
    public async Task User_is_accepted_when_organisation_is_not_empty()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
        );

        // Act
        var response = await client.GetAsync(JourneyUrl, Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);
    }

    private HttpClient CreateClientOrganisationClaims(string? organisationValue)
    {
        var client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
        );

        if (organisationValue is null)
        {
            return client.WithClaims(
                MockAuthHandler.DefaultClaims().Where(claim => claim.Type != Organisation)
            );
        }

        return client.WithOrganisation(organisationValue);
    }
}
