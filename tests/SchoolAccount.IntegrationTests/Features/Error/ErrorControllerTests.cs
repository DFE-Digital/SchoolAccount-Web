using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static SchoolAccount.Web.Mvc.Features.Error.ErrorViewModel;

namespace SchoolAccount.IntegrationTests.Features.Error;

public class ErrorControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly HttpClient _authenticatedClient;
    private readonly IQueryHandler<
        GetCensusStatusesQuery,
        List<GetCensusStatusesResponse>
    > _getCensusStatusesHandler = Substitute.For<
        IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
    >();

    public ErrorControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _authenticatedClient = factory.CreateAuthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
            >(_ => _getCensusStatusesHandler)
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
        response.IsSuccessStatusCode.ShouldBeFalse();
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
        response.IsSuccessStatusCode.ShouldBeFalse();
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
        response.IsSuccessStatusCode.ShouldBeFalse();
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
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(NotFound);
    }

    [Fact]
    public async Task A_failure_while_loading_a_page_shows_the_service_error_page()
    {
        // Arrange
        _getCensusStatusesHandler
            .Handle(Arg.Any<GetCensusStatusesQuery>(), Arg.Any<CancellationToken>())
            .Throws(new ApplicationException("Bang!"));

        // Act
        var response = await _authenticatedClient.GetAsync(JourneyUrl, _cancellationToken);
        var errorPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        var errorPageTitle = errorPage.GetPageTitle();
        errorPageTitle.ShouldBeEquivalentTo(ErrorTitle);

        response.IsSuccessStatusCode.ShouldBeFalse();
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
