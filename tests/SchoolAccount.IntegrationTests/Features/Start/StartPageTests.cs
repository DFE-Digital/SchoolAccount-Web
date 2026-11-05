using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static Xunit.TestContext;

namespace SchoolAccount.IntegrationTests.Features.Start;

public class StartPageTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthenticatedClient();

    [Fact]
    public async Task The_start_page_explains_how_to_sign_in()
    {
        // Arrange & Act
        var response = await _client.GetAsync(StartUrl, Current.CancellationToken);
        var startPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();

        var pageTitle = startPage.GetPageTitle();
        pageTitle.ShouldBe("Home Page");

        var bodyElement = startPage.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldSatisfyAllConditions(
            body => body.ShouldContainWithoutWhitespace("Sign in to School Account"),
            body =>
                body.ShouldContainWithoutWhitespace(
                    "School Account brings DfE services and guidance"
                ),
            body => body.ShouldContainWithoutWhitespace("You'll need a DfE Sign-in account")
        );
    }
}
