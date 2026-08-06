using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc.Features.Error;
using Shouldly;
using static SchoolAccount.Web.Mvc.RouteConstants;

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
        _client = factory.CreateUnauthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
            >(_ => _getTimeSpecificHelloHandler)
        );
    }

    [Fact]
    public async Task Ensure_that_the_home_controller_returns_a_404_result_on_unknown_page()
    {
        // Act
        var response = await _client.GetAsync(
            "/orangesandapples",
            TestContext.Current.CancellationToken
        );

        var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var page = new AngleSharpPage(html);

        // Assert
        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo(ErrorViewModel.NotFoundTitle);

        var headingElement = page.GetFirstHeading();
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
        var response = await _client.GetAsync(Root, TestContext.Current.CancellationToken);

        var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var page = new AngleSharpPage(html);

        // Assert
        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo(ErrorViewModel.ErrorTitle);

        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
