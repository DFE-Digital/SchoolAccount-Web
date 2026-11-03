using Microsoft.Playwright;

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// Test users defined in the mock identity provider's test-users.json.
/// </summary>
internal static class TestUsers
{
    public const string AcademyA = "academy-a";
    public const string AcademyAName = "Test Academy A";

    public const string AcademyB = "academy-b";
    public const string AcademyBName = "Test Academy B";
}

internal static class MockSignIn
{
    /// <summary>
    /// Signs in through the mock identity provider and lands on the start
    /// page, which needs no backing APIs.
    /// </summary>
    public static async Task SignInAsAsync(IPage page, string userId)
    {
        await page.GotoAsync("/account/login?returnUrl=%2F");
        await page.Locator($"button#{userId}").ClickAsync();
        await page.WaitForURLAsync($"{EndToEndSettings.BaseUrl}/");
    }
}
