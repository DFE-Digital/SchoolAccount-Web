using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>
/// The main column of the Journey page: the census caption and heading, the return status, the
/// overview, and the Go to COLLECT button. From <c>Journey.cshtml</c>.
/// </summary>
public class JourneyComponent(IPage page)
    : BaseComponent(page.Locator(".govuk-grid-column-two-thirds:has(h1)"))
{
    public ILocator Caption => Root.Locator("span.census-title");
    public ILocator Heading => Root.Locator("h1");

    // Only the tag directly in the column: each step in the step by step has its own status tag.
    public ILocator StatusTag => Root.Locator(":scope > strong.status-tag");
    public ILocator OverviewHeading => Root.Locator("h2").Filter(new() { HasText = "Overview" });
    public ILocator OverviewText => Root.Locator(":scope > p.govuk-body");
    public ILocator GoToCollectButton => Root.Locator("a[href*='collectdata.education.gov.uk']");

    public Task<string> GetStatusAsync()
    {
        return GetTrimmedTextAsync(StatusTag);
    }
}
