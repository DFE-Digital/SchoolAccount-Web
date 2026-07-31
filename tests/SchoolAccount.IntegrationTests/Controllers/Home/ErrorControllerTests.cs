using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc.Models;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Controllers.Home;

public class ErrorControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    public ErrorControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                    services.AddScoped<
                        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
                    >(_ => _getTimeSpecificHelloHandler)
                )
            )
            .CreateClient();
    }

    [Fact]
    public async Task Ensure_that_the_home_controller_returns_a_404_result_on_unknown_page()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync(
            "/orangesandapples",
            TestContext.Current.CancellationToken
        );

        string html = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var page = new AngleSharpPage(html);

        // Assert
        page.ShouldNotBeNull();

        string? pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo(ErrorViewModel.NotFoundTitle);

        string? headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo("Page not found");

        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Ensure_that_when_the_home_controller_fails_it_returns_a_500_result()
    {
        // Arrange
        _getTimeSpecificHelloHandler
            .Handle(Arg.Any<GetTimeSpecificHelloQuery>(), Arg.Any<CancellationToken>())
            .Throws(new ApplicationException("Bang!"));

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            "/",
            TestContext.Current.CancellationToken
        );

        string html = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var page = new AngleSharpPage(html);

        // Assert
        page.ShouldNotBeNull();

        string? pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo(ErrorViewModel.ErrorTitle);

        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
