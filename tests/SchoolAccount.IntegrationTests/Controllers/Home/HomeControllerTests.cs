using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Models;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Controllers.Home;

public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    public HomeControllerTests(WebApplicationFactory<Program> factory)
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
    public async Task Ensure_that_the_home_controller_returns_a_successful_result()
    {
        // Arrange
        var stubbedGetSpecificHelloResponse = new GetTimeSpecificHelloResponse(
            GetTimeSpecificHelloHandler.Messages.Morning
        );
        _getTimeSpecificHelloHandler
            .Handle(Arg.Any<GetTimeSpecificHelloQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(stubbedGetSpecificHelloResponse));

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            "/",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();

        string html = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var page = new AngleSharpPage();
        await page.Parse(html);

        page.ShouldNotBeNull();

        string? pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Home Page");

        string? headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo(stubbedGetSpecificHelloResponse.Message);

        string? bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContainWithoutWhitespace("In case I don't see ya");
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
        var page = new AngleSharpPage();
        await page.Parse(html);

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
        var page = new AngleSharpPage();
        await page.Parse(html);

        // Assert
        page.ShouldNotBeNull();

        string? pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo(ErrorViewModel.ErrorTitle);

        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
