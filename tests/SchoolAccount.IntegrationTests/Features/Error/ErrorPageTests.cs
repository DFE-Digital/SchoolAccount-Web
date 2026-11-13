using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.IntegrationTests.Common.Stubs;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static SchoolAccount.Web.Mvc.Features.Error.ErrorViewModel;

namespace SchoolAccount.IntegrationTests.Features.Error;

public class ErrorPageTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _authorisedClient = factory.CreateAuthorisedClient(services =>
        services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
    );

    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_forbidden_error_shows_the_forbidden_page()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(ErrorForbiddenUrl, _cancellationToken);
        var forbiddenErrorPage = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            _cancellationToken
        );

        // Assert
        forbiddenErrorPage.IsForbiddenPageTitle().ShouldBeTrue();
        forbiddenErrorPage.IsForbiddenPageHeading().ShouldBeTrue();
        response.StatusCode.ShouldBe(Forbidden);
    }

    [Fact]
    public async Task A_not_found_error_shows_the_not_found_page()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(ErrorNotFoundUrl, _cancellationToken);
        var notFoundErrorPage = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            _cancellationToken
        );

        // Assert
        notFoundErrorPage.IsNotFoundPageTitle().ShouldBeTrue();
        notFoundErrorPage.IsNotFoundPageHeading().ShouldBeTrue();
        response.StatusCode.ShouldBe(NotFound);
    }

    [Fact]
    public async Task A_service_error_shows_the_service_error_page()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(Error500Url, _cancellationToken);
        var errorPage = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            _cancellationToken
        );

        // Assert
        errorPage.IsServerErrorPageTitle().ShouldBeTrue();
        response.StatusCode.ShouldBe(InternalServerError);
    }

    [Fact]
    public async Task An_unknown_page_shows_the_not_found_page()
    {
        // Arrange
        var requestUri = "/this-errorPage-does-not-exist";

        // Act
        var response = await _authorisedClient.GetAsync(requestUri, _cancellationToken);
        var notFoundErrorPage = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            _cancellationToken
        );

        // Assert
        notFoundErrorPage.IsNotFoundPageTitle().ShouldBeTrue();
        notFoundErrorPage.IsNotFoundPageHeading().ShouldBeTrue();
        response.StatusCode.ShouldBe(NotFound);
    }

    [Fact]
    public async Task A_failure_while_loading_a_page_shows_the_service_error_page()
    {
        // Arrange
        var failingClient = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(StubCensusJourneyHandler.Failing())
        );

        // Act
        var response = await failingClient.GetAsync(JourneyUrl, _cancellationToken);
        var errorPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        errorPage.GetPageTitle().ShouldBe(ErrorTitle);
        response.StatusCode.ShouldBe(InternalServerError);
    }

    [Fact]
    public async Task A_service_error_shows_the_service_error_page_after_accepting_a_post()
    {
        // Arrange & Act
        var response = await _authorisedClient.PostAsync(Error500Url, null, _cancellationToken);
        var errorPage = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            _cancellationToken
        );

        // Assert
        errorPage.IsServerErrorPageTitle().ShouldBeTrue();
        response.StatusCode.ShouldBe(InternalServerError);
    }

    [Fact]
    public async Task A_post_to_an_unknown_page_renders_an_error_page()
    {
        // Arrange
        var requestUri = "/this-errorPage-does-not-exist";

        // Act
        var response = await _authorisedClient.PostAsync(requestUri, null, _cancellationToken);
        var errorPage = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            _cancellationToken
        );

        // Assert
        errorPage.IsServerErrorPageTitle().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
