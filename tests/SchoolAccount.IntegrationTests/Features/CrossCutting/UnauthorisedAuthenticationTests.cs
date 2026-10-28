using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
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
        var journeyUri = factory.GeneratePath("Journey", "Journey");

        // Act
        var response = await _client.GetAsync(journeyUri, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldBeEquivalentTo(
                factory.GeneratePath("Start", "Start", new { ReturnUrl = journeyUri })
            );
    }

    [Fact]
    public async Task Signed_out_users_can_see_the_start_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unauthenticated_pages_do_not_display_the_sign_out_link()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var pageSignOutLink = page.GetSignOutLink();
        pageSignOutLink.ShouldBeNull();
    }

    [Fact]
    public async Task Unauthenticated_pages_do_not_display_the_organisation_name()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var pageOrganisationName = page.GetOrganisationName();
        pageOrganisationName.ShouldBeNull();
    }
}
