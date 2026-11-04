using SchoolAccount.EndToEndTests.Common;
using Shouldly;
using static Microsoft.Playwright.Assertions;

namespace SchoolAccount.EndToEndTests.Features;

public class SignInTests(EndToEndApp app)
{
    private const string HeaderSchool = ".header-navigation__school";

    [Theory]
    [InlineData(TestUsers.AcademyA, TestUsers.AcademyAName)]
    [InlineData(TestUsers.AcademyB, TestUsers.AcademyBName)]
    public async Task Ensure_a_signed_in_user_sees_their_academy_in_the_header(
        string username,
        string academyName
    )
    {
        Assert.SkipUnless(EndToEndSettings.IsEnabled, EndToEndSettings.SkipReason);

        // Arrange
        await using var context = await app.NewContextAsync();
        var page = await context.NewPageAsync();

        // Act
        await MockSignIn.SignInAsAsync(page, username);

        // Assert
        await Expect(page.Locator(HeaderSchool)).ToHaveTextAsync(academyName);
    }

    [Fact]
    public async Task Ensure_a_saved_session_is_reused_without_signing_in_again()
    {
        Assert.SkipUnless(EndToEndSettings.IsEnabled, EndToEndSettings.SkipReason);

        // Arrange: sign in once and save the session cookies.
        string storageState;
        await using (var signInContext = await app.NewContextAsync())
        {
            var signInPage = await signInContext.NewPageAsync();
            await MockSignIn.SignInAsAsync(signInPage, TestUsers.AcademyA);
            storageState = await signInContext.StorageStateAsync();
        }

        // Act: a fresh context starts already signed in.
        await using var context = await app.NewContextAsync(storageState);
        var page = await context.NewPageAsync();
        var response = await page.GotoAsync("/");

        // Assert
        response.ShouldNotBeNull().Url.ShouldBe($"{app.BaseUrl}/");
        await Expect(page.Locator(HeaderSchool)).ToHaveTextAsync(TestUsers.AcademyAName);
    }
}
