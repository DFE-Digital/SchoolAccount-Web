using System.Net;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.Web.Mvc.Features.Dashboard;
using SchoolAccount.Web.Mvc.Features.Error;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Error;

public class ErrorControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _authenticatedClient;

    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    public ErrorControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
            >(_ => _getTimeSpecificHelloHandler)
        );
    }

    [Fact]
    public async Task Ensure_that_the_not_found_http_status_has_corresponding_page()
    {
        // Arrange
        var requestUri = _factory.GeneratePath("Error", "Error", new { statusCode = 404 });

        // Act
        var response = await _authenticatedClient.GetAsync(
            requestUri,
            TestContext.Current.CancellationToken
        );
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        page.ShouldNotBeNull();
        page.IsNotFoundPageTitle().ShouldBeTrue();
        page.IsNotFoundPageHeading().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Ensure_that_the_internal_server_error_http_status_has_corresponding_page()
    {
        // Arrange
        var requestUri = _factory.GeneratePath("Error", "Error", new { statusCode = 500 });

        // Act
        var response = await _authenticatedClient.GetAsync(
            requestUri,
            TestContext.Current.CancellationToken
        );
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        page.ShouldNotBeNull();
        page.IsServerErrorPageTitle().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Ensure_that_if_someone_is_navigating_to_any_page_valid_or_invalid_it_will_try_to_authenticate_them()
    {
        var client = _factory.CreateUnauthorisedClient();
        var requestUri = "/this-page-does-not-exist";

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task If_someone_is_authenticated_trying_to_access_a_unknown_page_they_will_get_a_not_found_page()
    {
        // Arrange
        var requestUri = "/this-page-does-not-exist";

        // Act
        var response = await _authenticatedClient.GetAsync(
            requestUri,
            TestContext.Current.CancellationToken
        );
        var page = await AngleSharpPage.FromResponseAsync<ErrorPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        page.ShouldNotBeNull();
        page.IsNotFoundPageTitle().ShouldBeTrue();
        page.IsNotFoundPageHeading().ShouldBeTrue();
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Ensure_that_the_server_error_http_status_has_corresponding_page()
    {
        // Arrange
        var requestUri = _factory.GeneratePath("Dashboard", "Dashboard");
        _getTimeSpecificHelloHandler
            .Handle(Arg.Any<GetTimeSpecificHelloQuery>(), Arg.Any<CancellationToken>())
            .Throws(new ApplicationException("Bang!"));

        // Act
        var response = await _authenticatedClient.GetAsync(
            requestUri,
            TestContext.Current.CancellationToken
        );
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo(ErrorViewModel.ErrorTitle);

        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
