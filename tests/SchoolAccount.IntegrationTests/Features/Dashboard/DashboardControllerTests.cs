using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.IntegrationTests.Common.Scenarios;
using SchoolAccount.TestCommon.Builders;
using Shouldly;
using static SchoolAccount.IntegrationTests.Common.MockAuthHandler;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

public class DashboardControllerTests(SchoolUserHost host) : IClassFixture<SchoolUserHost>
{
    private readonly HttpClient _client = host.CreateTestClient();

    [Fact]
    public async Task Greets_the_signed_in_user_by_name()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = host.GeneratePath("Dashboard", "Dashboard");

        host.CensusStatuses.Returns(
            CensusStatusesResponseBuilder.ACensusStatusResponse().AsSuccess()
        );

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo($"Hello {FakeGivenName} {FakeFamilyName}");
    }

    [Fact]
    public async Task Title_is_dashboard()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = host.GeneratePath("Dashboard", "Dashboard");

        host.CensusStatuses.Returns(
            CensusStatusesResponseBuilder.ACensusStatusResponse().AsSuccess()
        );

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Dashboard");
    }

    [Fact]
    public async Task Census_actions_are_listed_with_their_status()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = host.GeneratePath("Dashboard", "Dashboard");
        var response = CensusStatusesResponseBuilder
            .ACensusStatusResponse()
            .WithAction("Test Action", "Test Status");

        host.CensusStatuses.Returns(response.AsSuccess());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContain("Test Action, Test Status");
    }

    [Fact]
    public async Task A_failure_to_fetch_census_statuses_shows_the_service_error_page()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = host.GeneratePath("Dashboard", "Dashboard");
        host.CensusStatuses.Returns(CensusStatusesResponseBuilder.AsFailure());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(message, token);
        var body = await message.Content.ReadAsStringAsync(token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        page.IsServerErrorPageTitle().ShouldBeTrue();
        page.GetFirstHeading().ShouldBe("Sorry, there is a problem with the service");
        body.ShouldNotContain(CensusStatusesResponseBuilder.FetchFailed.Description);
    }
}
