using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Start;

public class StartControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;

    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    public StartControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
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
        var pageUri = _factory.GeneratePath("Start", "Start");

        // Act
        var response = await _client.GetAsync(
            pageUri,
            TestContext.Current.CancellationToken
        );

        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Home Page");

        var bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContainWithoutWhitespace(
            "Sign in to school account"
        );
    }
}
