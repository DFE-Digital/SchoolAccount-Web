using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Extensions;
using SchoolAccount.E2ETests.Common.Pages.Components;

namespace SchoolAccount.E2ETests.Common.Pages;

/// <summary>The Journey page for a census return (e.g. "/Journey" - Autumn School Census).</summary>
public class JourneyPage(IPage page) : SchoolAccountBasePage(page)
{
    public ILocator PageCaption => PageContext.SpanLocatorByClass("census-title");
    public ILocator CensusReturnHeading => PageContext.Locator("h1.govuk-heading-l");
    public ILocator CensusReturnStatusTag =>
        PageContext.Locator(".govuk-grid-column-two-thirds > strong.status-tag");
    public ILocator OverviewHeading =>
        PageContext.Locator("h2").Filter(new() { HasText = "Overview" });
    public ILocator OverviewText =>
        PageContext.Locator(".govuk-grid-column-two-thirds > p.govuk-body");
    public ILocator GoToCollectPortalButton =>
        PageContext.Locator("a[href*='collectdata.education.gov.uk']");

    public CensusStepByStepComponent StepByStep { get; } = new(page);
    public ImportantDatesComponent ImportantDates { get; } = new(page);

    public async Task<string> GetCensusReturnStatusAsync() =>
        (await CensusReturnStatusTag.TextContentAsync())?.Trim() ?? string.Empty;
}
