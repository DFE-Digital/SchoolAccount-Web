using System.Net;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

public class DashboardControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly StubCensusStatusesHandler _getCensusStatusesHandler = new();

    public DashboardControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(_getCensusStatusesHandler)
        );
    }

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_returns_correct_user_name()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        var response = CensusStatusesResponseBuilder.Create().NotInteresting();

        _getCensusStatusesHandler.Returns(response.AsSuccess());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Dashboard");

        var headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo(
            $"Hello {MockAuthHandler.FakeGivenName} {MockAuthHandler.FakeFamilyName}"
        );

        var bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContainWithoutWhitespace("Test School");
    }

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_returns_actions_when_there_are_actions()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        var response = CensusStatusesResponseBuilder
            .Create()
            .WithAction("Test Action", "Test Status");

        _getCensusStatusesHandler.Returns(response.AsSuccess());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContainWithoutWhitespace("Test Action");
        bodyElement.ShouldContainWithoutWhitespace("Test Status");
    }

    private sealed class StubCensusStatusesHandler
        : StubQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>;
}
