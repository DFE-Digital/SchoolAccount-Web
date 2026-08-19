using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class UnauthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthorisedClient();

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_redirects_for_unauthorised_users()
    {
        // Arrange
        var dashboardUri = factory.GeneratePath("Dashboard", "Dashboard");

        // Act
        var response = await _client.GetAsync(dashboardUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldBeEquivalentTo(
            factory.GeneratePath("Home", "Home", new { ReturnUrl = dashboardUri })
        );
    }

    [Fact]
    public async Task Ensure_that_the_home_controller_allows_unauthorised_users()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unauthenticated_pages_do_not_display_the_organisation_name_for_authorised_users()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var pageSignOutLink = page.GetSignOutLink();
        pageSignOutLink.ShouldBeNull();
    }

    [Fact]
    public async Task Unauthenticated_pages_do_not_display_the_sign_out_link_for_authorised_users()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var pageOrganisationName = page.GetOrganisationName();
        pageOrganisationName.ShouldBeNull();
    }
}
