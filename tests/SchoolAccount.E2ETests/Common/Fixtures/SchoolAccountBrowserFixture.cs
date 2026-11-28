using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Pages;
using SchoolAccount.TestCommon.Constants;

namespace SchoolAccount.E2ETests.Common.Fixtures;

/// <summary>Logs into School Account once per collection; every test in the collection starts already signed in.</summary>
public class SchoolAccountBrowserFixture : AuthenticatedBrowserFixture
{
    // How long DfE Sign-in has to send the user back to the site after they sign in.
    private const float _returnToSiteTimeoutMilliseconds = 30_000;

    protected override async Task LoginAsync(IPage page)
    {
        var account = Settings.SchoolAccount;
        if (string.IsNullOrWhiteSpace(account.Email) || string.IsNullOrWhiteSpace(account.Password))
        {
            throw new InvalidOperationException(
                "SchoolAccount:Email and SchoolAccount:Password must be set in user secrets to run the signed-in tests."
            );
        }

        var journeyUrl = $"{Settings.SiteUrl}{UrlConstants.JourneyUrl}";

        // The site's own home page just links out to the login URL, which redirects straight
        // into the DfE Sign-in form - go there directly rather than clicking through.
        await page.GotoAsync($"{Settings.SiteUrl}{UrlConstants.LoginUrl}");

        var loginPage = new SchoolAccountLoginPage(page);
        try
        {
            await loginPage.LoginAsync(account.Email, account.Password);
            await SelectOrganisationIfAskedAsync(page, journeyUrl);

            // Sign-in has worked once DfE Sign-in sends the user back to the site's Journey page.
            await page.WaitForURLAsync(
                url => url.StartsWith(journeyUrl, StringComparison.OrdinalIgnoreCase),
                new PageWaitForURLOptions { Timeout = _returnToSiteTimeoutMilliseconds }
            );
        }
        catch (Exception failure) when (failure is PlaywrightException or TimeoutException)
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

    /// <summary>
    /// An account linked to more than one organisation is asked to pick one before DfE Sign-in
    /// sends it back to the site. Picks the school set in SchoolAccount:OrganisationUrn.
    /// </summary>
    private async Task SelectOrganisationIfAskedAsync(IPage page, string journeyUrl)
    {
        await page.WaitForURLAsync(
            url =>
                url.StartsWith(journeyUrl, StringComparison.OrdinalIgnoreCase)
                || SelectOrganisationPage.IsShownAt(url),
            new PageWaitForURLOptions { Timeout = _returnToSiteTimeoutMilliseconds }
        );

        if (!SelectOrganisationPage.IsShownAt(page.Url))
        {
            return;
        }

        var urn = Settings.SchoolAccount.OrganisationUrn;
        if (string.IsNullOrWhiteSpace(urn))
        {
            throw new InvalidOperationException(
                "The test account is linked to more than one organisation, so DfE Sign-in asks which "
                    + "one to use. Set SchoolAccount:OrganisationUrn in user secrets to the URN of the "
                    + "school to sign in as."
            );
        }

        await new SelectOrganisationPage(page).SelectAsync(urn);
    }
}
