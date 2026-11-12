using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Pages;

namespace SchoolAccount.E2ETests.Common.Fixtures;

/// <summary>Logs into School Account once per collection; every test in the collection starts already signed in.</summary>
public class SchoolAccountBrowserFixture : AuthenticatedBrowserFixture
{
    // How long DfE Sign-in has to send the user back to the site after they sign in.
    private const float ReturnToSiteTimeoutMilliseconds = 30_000;

    protected override async Task LoginAsync(IPage page)
    {
        var account = Settings.SchoolAccount;
        if (string.IsNullOrWhiteSpace(account.Email) || string.IsNullOrWhiteSpace(account.Password))
        {
            throw new InvalidOperationException(
                "SchoolAccount:Email and SchoolAccount:Password must be set in user secrets to run the signed-in tests."
            );
        }

        var journeyUrl = $"{Settings.SiteUrl}/Journey";

        // The site's own home page just links out to /Account/Login, which redirects straight
        // into the DfE Sign-in form - go there directly rather than clicking through.
        await page.GotoAsync($"{Settings.SiteUrl}/Account/Login");

        var loginPage = new SchoolAccountLoginPage(page);
        try
        {
            await loginPage.LoginAsync(account.Email, account.Password);

            // Sign-in has worked once DfE Sign-in sends the user back to the site's Journey page.
            await page.WaitForURLAsync(
                url => url.StartsWith(journeyUrl, StringComparison.OrdinalIgnoreCase),
                new PageWaitForURLOptions { Timeout = ReturnToSiteTimeoutMilliseconds }
            );
        }
        catch (PlaywrightException)
        {
            var signInError = await loginPage.GetErrorMessageAsync();
            if (signInError is not null)
            {
                throw new InvalidOperationException(
                    $"DfE Sign-in showed an error, so sign-in won't be retried: {signInError}"
                );
            }

            throw;
        }
    }
}
