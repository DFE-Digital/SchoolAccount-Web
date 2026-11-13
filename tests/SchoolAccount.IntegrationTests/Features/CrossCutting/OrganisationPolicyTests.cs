using NSubstitute;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Stubs;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.IntegrationTests.Common.Builders.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyResponseBuilder;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;
using static Xunit.TestContext;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class OrganisationPolicyTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CensusJourneyHandler _getCensusJourneyHandler = Stub.CensusJourneyHandler();

    public OrganisationPolicyTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(_getCensusJourneyHandler)
        );
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("")]
    public async Task User_is_forbidden_when_organisation_claim_is_empty(string value)
    {
        // Arrange
        _client.WithOrganisation(value);

        // Act
        var response = await _client.GetAsync(JourneyUrl, Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Forbidden);
    }

    [Fact]
    public async Task User_is_forbidden_when_organisation_claim_is_missing()
    {
        // Arrange
        _client.WithClaims(
            MockAuthHandler.DefaultClaims().Where(claim => claim.Type != Organisation)
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Forbidden);
    }

    [Fact]
    public async Task User_is_accepted_when_organisation_is_not_empty()
    {
        // Arrange
        _getCensusJourneyHandler
            .Handle(Arg.Any<GetCensusJourneyQuery>(), Arg.Any<CancellationToken>())
            .Returns(AGetCensusJourneyResponse().AsSuccess());

        // Act
        var response = await _client.GetAsync(JourneyUrl, Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);
    }
}
