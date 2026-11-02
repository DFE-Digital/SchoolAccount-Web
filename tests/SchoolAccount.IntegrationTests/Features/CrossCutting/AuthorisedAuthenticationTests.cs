using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.IntegrationTests.Common.MockAuthHandler;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AuthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    private readonly HttpClient _client = factory.CreateAuthorisedClient(services =>
        services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
    );

    [Theory]
    [InlineData(StartUrl)]
    [InlineData(JourneyUrl)]
    public async Task Signed_in_users_can_access_a_page(string path)
    {
        // Arrange & Act
        var response = await _client.GetAsync(path, _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(Redirect);
        response.StatusCode.ShouldBe(OK);
    }

    [Fact]
    public async Task Signed_in_users_see_the_sign_out_link()
    {
        // Arrange & Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        journeyPage.GetElementContainingText("button", "Sign out").ShouldNotBeNull();
    }

    [Fact]
    public async Task Signed_in_users_see_their_organisation_name()
    {
        // Arrange & Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var organisationName = journeyPage.GetOrganisationName();
        organisationName.ShouldNotBeNull();
        organisationName.ShouldContainWithoutWhitespace(FakeOrganisationName);
    }
}
