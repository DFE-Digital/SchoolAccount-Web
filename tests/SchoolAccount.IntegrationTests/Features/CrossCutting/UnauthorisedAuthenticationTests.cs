using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class UnauthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthenticatedClient();
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Signed_out_users_cannot_access_the_journey_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldBeEquivalentTo(JourneyReturnUrl);
    }

    [Fact]
    public async Task Signed_out_users_can_see_the_start_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync(StartUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(Redirect);
        response.StatusCode.ShouldBe(OK);
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_the_sign_out_link()
    {
        // Arrange & Act
        var response = await _client.GetAsync(StartUrl, _cancellationToken);
        var startPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        startPage.GetElementContainingText("button", "Sign out").ShouldBeNull();
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_an_organisation_name()
    {
        // Arrange & Act
        var response = await _client.GetAsync(StartUrl, _cancellationToken);
        var startPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        var organisationName = startPage.GetOrganisationName();
        organisationName.ShouldBeNull();
    }
}
