using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.SharedKernel;
using Shouldly;
using static SchoolAccount.Web.Mvc.RouteConstants;

namespace SchoolAccount.IntegrationTests.Features.Start;

public class StartControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    public StartControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateUnauthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
            >(_ => _getTimeSpecificHelloHandler)
        );
    }

    [Fact]
    public async Task Ensure_that_the_start_controller_returns_a_successful_result()
    {
        // Arrange
        var stubbedGetSpecificHelloResponse = new GetTimeSpecificHelloResponse(
            GetTimeSpecificHelloHandler.Messages.Morning
        );
        _getTimeSpecificHelloHandler
            .Handle(Arg.Any<GetTimeSpecificHelloQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(stubbedGetSpecificHelloResponse));

        // Act
        var response = await _client.GetAsync(Root, TestContext.Current.CancellationToken);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();

        var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var page = new AngleSharpPage(html);

        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Home Page");

        var headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo(stubbedGetSpecificHelloResponse.Message);

        var bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContainWithoutWhitespace("In case I don't see ya");
    }
}
