using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.IntegrationTests.Common.Stubs;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static SchoolAccount.Web.Mvc.Features.Error.ErrorViewModel;

namespace SchoolAccount.IntegrationTests.Features.Error;

public class ErrorPageTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly HttpClient _authenticatedClient;
    private readonly CensusJourneyHandler _getCensusJourneyHandler = Stub.CensusJourneyHandler();

    public ErrorPageTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _authenticatedClient = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(_getCensusJourneyHandler)
        );
    }

    [Fact]
    public async Task A_forbidden_error_shows_the_forbidden_page()
    {
        // Arrange & Act
        var response = await _authenticatedClient.GetAsync(ErrorForbiddenUrl, _cancellationToken);
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
        var response = await _authenticatedClient.GetAsync(ErrorNotFoundUrl, _cancellationToken);
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
        var response = await _authenticatedClient.GetAsync(Error500Url, _cancellationToken);
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
        var response = await _authenticatedClient.GetAsync(requestUri, _cancellationToken);
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
        _getCensusJourneyHandler
            .Handle(Arg.Any<GetCensusJourneyQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ApplicationException("Bang!"));

        // Act
        var response = await _authenticatedClient.GetAsync(JourneyUrl, _cancellationToken);
        var errorPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        var errorPageTitle = errorPage.GetPageTitle();
        errorPageTitle.ShouldBe(ErrorTitle);

        response.StatusCode.ShouldBe(InternalServerError);
    }

    [Fact]
    public async Task A_service_error_shows_the_service_error_page_after_accepting_a_post()
    {
        // Arrange & Act
        var response = await _authenticatedClient.PostAsync(
            Error500Url,
            content: null,
            _cancellationToken
        );

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
        var response = await _authenticatedClient.PostAsync(
            requestUri,
            content: null,
            _cancellationToken
        );

        var errorPage = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            _cancellationToken
        );

        // Assert
        errorPage.IsServerErrorPageTitle().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
