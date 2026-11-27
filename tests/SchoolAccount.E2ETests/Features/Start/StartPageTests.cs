using SchoolAccount.E2ETests.Common.Fixtures;
using SchoolAccount.E2ETests.Common.Pages;

namespace SchoolAccount.E2ETests.Features.Start;

[Collection(PublicSiteCollection.Name)]
public class StartPageTests : BrowserTestBase
{
    public StartPageTests(BrowserFixture fixture)
        : base(fixture) { }

    [Fact]
    public async Task Start_page_loads_with_the_sign_in_heading()
    {
        // Arrange
        var homePage = new StartPage(Page);

        // Act
        await homePage.NavigateAsync(Settings.SiteUrl);

        // Assert
        await homePage.AssertTitle("Sign in to School Account");
    }
}
