using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;
using static SchoolAccount.IntegrationTests.Common.MockAuthHandler;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

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

        _getCensusStatusesHandler.Returns(CensusStatusesResponseBuilder.Create().AsSuccess());

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
    public async Task Ensure_that_the_dashboard_controller_returns_correct_page_title()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");

        _getCensusStatusesHandler.Returns(CensusStatusesResponseBuilder.Create().AsSuccess());

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
        bodyElement.ShouldContain("Test Action, Test Status");
    }

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_reports_a_problem_when_the_query_fails()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        _getCensusStatusesHandler.Returns(CensusStatusesResponseBuilder.AsFailure());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var body = await message.Content.ReadAsStringAsync(token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        body.ShouldContain(CensusStatusesResponseBuilder.FetchFailed.Description);
    }

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_fails_when_the_user_context_is_empty()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        var client = CreateClientWithoutUserDetails();

        _getCensusStatusesHandler.Returns(CensusStatusesResponseBuilder.Create().AsSuccess());

        // Act
        var message = await client.GetAsync(pageUri, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    private HttpClient CreateClientWithoutUserDetails()
    {
        List<Claim> claims = [new(Organisation, """{"id": 12345}""")];

        return _factory.CreateAuthorisedClient(services =>
        {
            services.RemoveAll<MockAuthClaimsOptions>();
            services.AddSingleton(new MockAuthClaimsOptions { Claims = claims });
            services.StubQueryHandler(_getCensusStatusesHandler);
        });
    }

    private sealed class StubCensusStatusesHandler
        : StubQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>;
}
