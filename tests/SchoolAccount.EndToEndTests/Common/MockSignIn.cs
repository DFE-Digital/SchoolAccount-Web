using Microsoft.Playwright;

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// Test users defined in tests/oidc-server-mock/users.json. Each signs in
/// with its username and the shared password.
/// </summary>
internal static class TestUsers
{
    public const string AcademyA = "academy-a";
    public const string AcademyAName = "Test Academy A";

    public const string AcademyB = "academy-b";
    public const string AcademyBName = "Test Academy B";

    public const string Password = "password";
}

internal static class MockSignIn
{
    /// <summary>
    /// Signs in through oidc-server-mock's login form and lands on the start
    /// page, which needs no backing APIs.
    /// </summary>
    public static async Task SignInAsAsync(IPage page, string username)
    {
        await page.GotoAsync("/account/login?returnUrl=%2F");
        await page.Locator("#Input_Username").FillAsync(username);
        await page.Locator("#Input_Password").FillAsync(TestUsers.Password);
        await page.Locator("button[value=login]").ClickAsync();
        await page.WaitForURLAsync($"{EndToEndSettings.BaseUrl}/");
    }
}
