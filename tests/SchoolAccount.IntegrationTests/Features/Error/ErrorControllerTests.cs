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
    private readonly HttpClient _client;

    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    public ErrorControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateAuthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
            >(_ => _getTimeSpecificHelloHandler)
        );
    }

    [Fact]
    public async Task Ensure_that_the_not_found_http_status_has_corresponding_page()
    {
        // Arrange
        var response = await _client.GetAsync("/error/404", TestContext.Current.CancellationToken);

        // Act
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
        var response = await _client.GetAsync("/error/500", TestContext.Current.CancellationToken);

        // Act
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

    // Test to ensure that the server error page triggers correctly
    // [Fact]
    // public async Task Ensure_that_the_server_error_http_status_has_corresponding_page()
    // {
    //     // Arrange
    //     var response = await _client.GetAsync(
    //         "/error/500",
    //         TestContext.Current.CancellationToken
    //     );
    //
    //     var requestUri = UrlBuilder.GeneratePath<DashboardController>(
    //         nameof(DashboardController.Dashboard)
    //     );
    //
    //     _getTimeSpecificHelloHandler
    //         .Handle(Arg.Any<GetTimeSpecificHelloQuery>(), Arg.Any<CancellationToken>())
    //         .Throws(new ApplicationException("Bang!"));
    //
    //     // Act
    //     var response = await _client.GetAsync(requestUri, TestContext.Current.CancellationToken);
    //
    //     var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    //     var page = new AngleSharpPage(html);
    //
    //     // Assert
    //     page.ShouldNotBeNull();
    //
    //     var pageTitle = page.GetTitle();
    //     pageTitle.ShouldNotBeNull();
    //     pageTitle.ShouldBeEquivalentTo(ErrorViewModel.ErrorTitle);
    //
    //     response.IsSuccessStatusCode.ShouldBeFalse();
    //     response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    // }
}
