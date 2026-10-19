using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Builders;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;
using static SchoolAccount.IntegrationTests.Common.MockAuthHandler;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

public class DashboardControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly StubCensusStatusesHandler _getCensusStatusesHandler = new();

    public DashboardControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Greets_the_signed_in_user_by_name()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        var client = CreateClient();

        _getCensusStatusesHandler.Returns(
            CensusStatusesResponseBuilder.ACensusStatusResponse().AsSuccess()
        );

        // Act
        var message = await client.GetAsync(pageUri, token);
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
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        var client = CreateClient();

        _getCensusStatusesHandler.Returns(
            CensusStatusesResponseBuilder.ACensusStatusResponse().AsSuccess()
        );

        // Act
        var message = await client.GetAsync(pageUri, token);
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
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        var client = CreateClient();
        var response = CensusStatusesResponseBuilder
            .ACensusStatusResponse()
            .WithAction("Test Action", "Test Status");

        _getCensusStatusesHandler.Returns(response.AsSuccess());

        // Act
        var message = await client.GetAsync(pageUri, token);
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
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        var client = CreateClient();
        _getCensusStatusesHandler.Returns(CensusStatusesResponseBuilder.AsFailure());

        // Act
        var message = await client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(message, token);
        var body = await message.Content.ReadAsStringAsync(token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        page.IsServerErrorPageTitle().ShouldBeTrue();
        page.GetFirstHeading().ShouldBe("Sorry, there is a problem with the service");
        body.ShouldNotContain(CensusStatusesResponseBuilder.FetchFailed.Description);
    }

    [Fact]
    public async Task A_user_with_no_id_email_or_organisation_is_rejected()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");
        List<Claim> claims = [new(Organisation, """{"id": 12345}""")];
        var client = CreateClient(services =>
            services.Replace(
                ServiceDescriptor.Singleton(new MockAuthClaimsOptions { Claims = claims })
            )
        );

        _getCensusStatusesHandler.Returns(
            CensusStatusesResponseBuilder.ACensusStatusResponse().AsSuccess()
        );

        // Act
        var message = await client.GetAsync(pageUri, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    /// <summary>
    /// Creates the one client a test uses. Any overrides go in <paramref name="configureServices"/>
    /// and use <c>Replace</c>, so every registration for the test is in one place.
    /// </summary>
    private HttpClient CreateClient(Action<IServiceCollection>? configureServices = null) =>
        _factory.CreateAuthorisedClient(services =>
        {
            services.StubQueryHandler(_getCensusStatusesHandler);
            configureServices?.Invoke(services);
        });
}
