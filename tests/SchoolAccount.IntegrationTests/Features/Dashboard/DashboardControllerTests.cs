using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

public class DashboardControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateAuthorisedClient();

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_returns_correct_user_name()
    {
        // Act
        var response = await _client.GetAsync(
            RouteConstants.Dashboard,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var page = new AngleSharpPage(html);

        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Dashboard");

        var headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo("Welcome Test user Test surname");
    }
}
