using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Pages;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>The "Important dates" summary list in the Journey page sidebar (Census day, Return due).</summary>
public class ImportantDatesComponent(IPage page) : BasePage(page)
{
    public ILocator CensusDayValue => GetDateRowValue("Census day");
    public ILocator ReturnDueValue => GetDateRowValue("Return due");

    public async Task<string> GetCensusDayAsync() =>
        (await CensusDayValue.TextContentAsync())?.Trim() ?? string.Empty;

    public async Task<string> GetReturnDueDateAsync() =>
        (await ReturnDueValue.TextContentAsync())?.Trim() ?? string.Empty;

    private ILocator GetDateRowValue(string label) =>
        PageContext
            .Locator("div.govuk-summary-list__row")
            .Filter(new() { HasText = label })
            .Locator("dd.govuk-summary-list__value");
}
