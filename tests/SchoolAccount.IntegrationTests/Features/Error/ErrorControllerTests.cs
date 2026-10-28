using System.Net;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.Web.Mvc.Features.Error;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Error;

public class ErrorControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
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
        _factory = factory;
        _authenticatedClient = factory.CreateAuthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
            >(_ => _getCensusStatusesHandler)
        );
    }

    [Fact]
    public async Task A_not_found_error_shows_the_page_not_found_page()
    {
        // Arrange
        var requestUri = _factory.GeneratePath("Error", "Error", new { statusCode = 404 });

        // Act
        var response = await _authenticatedClient.GetAsync(requestUri, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(response, _cancellationToken);

        // Assert
        page.IsNotFoundPageTitle().ShouldBeTrue();
        page.IsNotFoundPageHeading().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task A_service_error_shows_the_service_error_page()
    {
        // Arrange
        var requestUri = _factory.GeneratePath("Error", "Error", new { statusCode = 500 });

        // Act
        var response = await _authenticatedClient.GetAsync(requestUri, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(response, _cancellationToken);

        // Assert
        page.IsServerErrorPageTitle().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task If_page_is_unknown_it_provides_you_a_not_found_page()
    {
        // Arrange
        var requestUri = "/this-page-does-not-exist";

        // Act
        var response = await _authenticatedClient.GetAsync(requestUri, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(response, _cancellationToken);

        // Assert
        page.IsNotFoundPageTitle().ShouldBeTrue();
        page.IsNotFoundPageHeading().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task A_failure_while_loading_a_page_shows_the_service_error_page()
    {
        // Arrange
        var requestUri = _factory.GeneratePath("Journey", "Journey");

        _getCensusStatusesHandler
            .Handle(Arg.Any<GetCensusStatusesQuery>(), Arg.Any<CancellationToken>())
            .Throws(new ApplicationException("Bang!"));

        // Act
        var response = await _authenticatedClient.GetAsync(requestUri, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo(ErrorViewModel.ErrorTitle);

        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Error_page_is_rendered_when_re_executed_with_a_non_get_method()
    {
        // Arrange
        var requestUri = _factory.GeneratePath("Error", "Error", new { statusCode = 500 });

        // Act
        var response = await _authenticatedClient.PostAsync(
            requestUri,
            content: null,
            _cancellationToken
        );

        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(response, _cancellationToken);

        // Assert
        page.IsServerErrorPageTitle().ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task A_post_to_an_unknown_page_renders_an_error_page()
    {
        // Arrange
        var requestUri = "/this-page-does-not-exist";

        // Act
        var response = await _authenticatedClient.PostAsync(
            requestUri,
            content: null,
            _cancellationToken
        );

        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(response, _cancellationToken);

        // Assert
        page.IsServerErrorPageTitle().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
