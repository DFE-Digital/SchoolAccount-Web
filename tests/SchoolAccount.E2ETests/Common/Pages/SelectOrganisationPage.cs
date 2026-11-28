using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Pages;

/// <summary>
/// The DfE Sign-in "Select your organisation" page, shown after sign-in to an account linked to
/// more than one organisation.
/// </summary>
public class SelectOrganisationPage(IPage page) : BasePage(page)
{
    private ILocator Organisations => PageContext.Locator(".govuk-radios__item");
    private ILocator ContinueButton =>
        PageContext.Locator("input[type='submit'][value='Continue']");

    public static bool IsShownAt(string url)
    {
        return url.Contains("/select-organisation", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Picks the organisation with the given URN and continues. URNs are unique, unlike names, and
    /// only establishments have one, so this can't pick a trust by mistake.
    /// </summary>
    public async Task SelectAsync(string urn)
    {
        var organisation = Organisations.Filter(
            new()
            {
                HasTextRegex = new Regex(
                    $@"\bURN: {Regex.Escape(urn)}\b",
                    RegexOptions.None,
                    TimeSpan.FromSeconds(1)
                ),
            }
        );

        if (await organisation.CountAsync() != 1)
        {
            throw new InvalidOperationException(
                $"The test account has no organisation with URN {urn}. Check SchoolAccount:OrganisationUrn."
            );
        }

        await organisation.Locator("input[type='radio']").CheckAsync();
        await ContinueButton.ClickAsync();
    }
}
