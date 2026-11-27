using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>The "Important dates" summary list in the Journey page sidebar (Census day, Return due). From <c>Journey.cshtml</c>.</summary>
public class ImportantDatesComponent(IPage page)
    : BaseComponent(page.Locator(".govuk-grid-column-one-third .govuk-summary-list"))
{
    public ILocator CensusDayValue => GetDateValue("Census day");
    public ILocator ReturnDueValue => GetDateValue("Return due");

    public Task<string> GetCensusDayAsync()
    {
        return GetTrimmedTextAsync(CensusDayValue);
    }

    public Task<string> GetReturnDueDateAsync()
    {
        return GetTrimmedTextAsync(ReturnDueValue);
    }

    private ILocator GetDateValue(string label)
    {
        return Root.Locator("div.govuk-summary-list__row")
            .Filter(new() { HasText = label })
            .Locator("dd.govuk-summary-list__value");
    }
}
