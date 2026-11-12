using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Extensions;
using static Microsoft.Playwright.Assertions;

namespace SchoolAccount.E2ETests.Common.Pages;

/// <summary>Page object for the DfE Sign-in login page reached from School Account.</summary>
public class SchoolAccountLoginPage(IPage page) : BasePage(page)
{
    private ILocator EmailInput => PageContext.InputLocatorByName("username");
    private ILocator PasswordInput => PageContext.InputLocatorById("password");
    private ILocator NextButton => PageContext.ButtonLocatorById("button-next");
    private ILocator SignInButton => PageContext.ButtonLocatorById("button-sign-in");
    private ILocator SignInOrRegisterButton => PageContext.GetByText("Sign in or register");
    private ILocator ErrorSummary => PageContext.Locator(".govuk-error-summary");

    public async Task LoginAsync(string email, string password)
    {
        // DfE Sign-in sometimes shows a "Sign in or register" step before the email field.
        // Wait for whichever appears first, rather than checking once and possibly too early.
        await Expect(EmailInput.Or(SignInOrRegisterButton).First).ToBeVisibleAsync();
        if (await SignInOrRegisterButton.IsVisibleAsync())
        {
            await SignInOrRegisterButton.ClickAsync();
        }

        await Expect(EmailInput).ToBeVisibleAsync();
        await Expect(EmailInput).ToBeEnabledAsync();
        await EmailInput.FillAsync(email);
        await NextButton.ClickAsync();

        await Expect(PasswordInput).ToBeVisibleAsync();
        await Expect(PasswordInput).ToBeEnabledAsync();
        await PasswordInput.FillAsync(password);
        await SignInButton.ClickAsync();
    }

    /// <summary>The text of the error DfE Sign-in is showing, or null if there isn't one.</summary>
    public async Task<string?> GetErrorMessageAsync() =>
        await ErrorSummary.IsVisibleAsync() ? (await ErrorSummary.InnerTextAsync()).Trim() : null;
}
