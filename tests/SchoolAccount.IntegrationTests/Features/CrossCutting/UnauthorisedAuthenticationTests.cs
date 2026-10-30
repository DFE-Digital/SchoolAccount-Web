using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Constants;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class UnauthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthenticatedClient();
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Signed_out_users_cannot_access_the_journey_page()
    {
        // Arrange
        var journeyUri = UrlConstants.JourneyUrl;

        // Act
        var response = await _client.GetAsync(journeyUri, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldBeEquivalentTo(UrlConstants.JourneyReturnUrl);
    }

    [Fact]
    public async Task Signed_out_users_can_see_the_start_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync(UrlConstants.BaseUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_the_sign_out_link()
    {
        // Arrange & Act
        var response = await _client.GetAsync(UrlConstants.BaseUrl, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.GetElementContainingText("button", "Sign out").ShouldBeNull();
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_an_organisation_name()
    {
        // Arrange & Act
        var response = await _client.GetAsync(UrlConstants.BaseUrl, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var pageOrganisationName = page.GetOrganisationName();
        pageOrganisationName.ShouldBeNull();
    }
}
