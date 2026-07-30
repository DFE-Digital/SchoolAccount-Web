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
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument page = await context.OpenAsync(
            req => req.Content(html),
            TestContext.Current.CancellationToken
        );

        page.ShouldNotBeNull();

        IElement? pageTitle = page.QuerySelector("title");
        pageTitle.ShouldNotBeNull();
        pageTitle.TextContent.ShouldBeEquivalentTo("Home Page");

        IElement? headingElement = page.QuerySelector("h1.govuk-heading-l");
        headingElement.ShouldNotBeNull();
        headingElement.TextContent.ShouldBeEquivalentTo(stubbedGetSpecificHelloResponse.Message);

        IElement? bodyElement = page.QuerySelector("p.govuk-body");
        bodyElement.ShouldNotBeNull();
        bodyElement.TextContent.ShouldContainWithoutWhitespace("In case I don't see ya");
    }

    [Fact]
    public async Task Ensure_that_the_home_controller_returns_a_404_result_on_unknown_page()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync(
            "/orangesandapples",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Ensure_that_the_home_controller_returns_a_500_result()
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

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
