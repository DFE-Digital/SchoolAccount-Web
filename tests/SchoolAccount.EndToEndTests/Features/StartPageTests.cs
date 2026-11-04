using SchoolAccount.EndToEndTests.Common;
using static Microsoft.Playwright.Assertions;

namespace SchoolAccount.EndToEndTests.Features;

// SignedInSessions is an assembly fixture, so xUnit passes in the one shared instance.
public class StartPageTests(SignedInSessions sessions)
{
    private const string HeaderSchool = ".header-navigation__school";

    [Theory]
    [InlineData(TestUsers.AcademyA, TestUsers.AcademyAName)]
    [InlineData(TestUsers.AcademyB, TestUsers.AcademyBName)]
    public async Task Ensure_the_header_shows_the_signed_in_academy(
        string username,
        string academyName
    )
    {
        Assert.SkipUnless(EndToEndSettings.IsEnabled, EndToEndSettings.SkipReason);

        // Arrange: no sign-in here; the context already has this academy's cookies.
        await using var context = await sessions.NewContextAsync(username);
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync("/");

        // Assert
        await Expect(page.Locator(HeaderSchool)).ToHaveTextAsync(academyName);
    }

    [Fact]
    public async Task Ensure_a_second_test_for_the_same_academy_reuses_the_session()
    {
        Assert.SkipUnless(EndToEndSettings.IsEnabled, EndToEndSettings.SkipReason);

        // Arrange: academy A has already signed in for an earlier test, or signs in now.
        await using var context = await sessions.NewContextAsync(TestUsers.AcademyA);
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync("/");

        // Assert
        await Expect(page.Locator(HeaderSchool)).ToHaveTextAsync(TestUsers.AcademyAName);
    }
}
