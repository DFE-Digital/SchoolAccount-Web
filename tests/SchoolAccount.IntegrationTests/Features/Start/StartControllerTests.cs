using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Constants;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Start;

public class StartControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthenticatedClient();

    [Fact]
    public async Task The_start_page_explains_how_to_sign_in()
    {
        // Arrange
        var pageUri = UrlConstants.BaseUrl;

        // Act
        var response = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        page.ShouldNotBeNull();

        var pageTitle = page.GetPageTitle();
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
