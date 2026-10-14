using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Start;

public class StartControllerTests : IClassFixture<UnauthenticatedWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly UnauthenticatedWebApplicationFactory _factory;

    public StartControllerTests(UnauthenticatedWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClientWithDefaults();
    }

    [Fact]
    public async Task Ensure_that_the_start_controller_returns_a_successful_result()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Start", "Start");

        // Act
        var response = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
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
        bodyElement.ShouldContainWithoutWhitespace("Sign in to School Account");
        bodyElement.ShouldContainWithoutWhitespace(
            "School Account brings DfE services and guidance"
        );
        bodyElement.ShouldContainWithoutWhitespace("You'll need a DfE Sign-in account");
    }
}
