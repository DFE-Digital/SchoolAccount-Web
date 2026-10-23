using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class UnauthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthenticatedClient();

    [Fact]
    public async Task Signed_out_users_are_sent_from_the_dashboard_to_the_start_page()
    {
        // Arrange
        var dashboardUri = factory.GeneratePath("Dashboard", "Dashboard");

        // Act
        var response = await _client.GetAsync(dashboardUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldBeEquivalentTo(
            factory.GeneratePath("Start", "Start", new { ReturnUrl = dashboardUri })
        );
    }

    [Fact]
    public async Task Signed_out_users_can_see_the_start_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_the_sign_out_link()
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
    public async Task Signed_out_users_do_not_see_an_organisation_name()
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
