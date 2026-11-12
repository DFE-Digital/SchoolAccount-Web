using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Pages;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>The step by step navigation list on the census Journey page (Check the census dates, Prepare the data in your MIS, etc.).</summary>
public class CensusStepByStepComponent(IPage page) : BasePage(page)
{
    public ILocator ShowOrHideAllStepsButton =>
        PageContext.Locator("button.js-step-controls-button");
    public ILocator Steps => PageContext.Locator("li.app-step-nav__step");

    public ILocator GetStep(string stepTitle) => Steps.Filter(new() { HasText = stepTitle });

    public ILocator GetStepToggleButton(string stepTitle) =>
        GetStep(stepTitle).Locator("button.js-step-title-button");

    public ILocator GetStepStatusTag(string stepTitle) =>
        GetStep(stepTitle).Locator("strong.status-tag");

    public ILocator GetStepPanel(string stepTitle) =>
        GetStep(stepTitle).Locator("div.app-step-nav__panel");

    public async Task ShowAllStepsAsync() => await ShowOrHideAllStepsButton.ClickAsync();

    public async Task ExpandStepAsync(string stepTitle) =>
        await GetStepToggleButton(stepTitle).ClickAsync();

    public async Task<bool> HasStatusAsync(string stepTitle) =>
        await GetStepStatusTag(stepTitle).IsVisibleAsync();

    public async Task<string> GetStepStatusTextAsync(string stepTitle) =>
        (await GetStepStatusTag(stepTitle).TextContentAsync())?.Trim() ?? string.Empty;

    public async Task<string> GetStepPanelTextAsync(string stepTitle) =>
        (await GetStepPanel(stepTitle).TextContentAsync())?.Trim() ?? string.Empty;
}
