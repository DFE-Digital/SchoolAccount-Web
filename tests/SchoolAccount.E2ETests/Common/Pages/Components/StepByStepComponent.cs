using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>
/// The step by step navigation (Check the census dates, Prepare the data in your MIS, etc.).
/// From <c>Shared/StepByStep/StepByStep.cshtml</c>.
/// </summary>
public class StepByStepComponent(IPage page) : BaseComponent(page.Locator(".app-step-nav"))
{
    public ILocator ShowOrHideAllStepsButton => Root.Locator("button.js-step-controls-button");
    public ILocator Steps => Root.Locator("li.app-step-nav__step");

    public ILocator GetStep(string stepTitle) => Steps.Filter(new() { HasText = stepTitle });

    public ILocator GetStepToggleButton(string stepTitle) =>
        GetStep(stepTitle).Locator("button.js-step-title-button");

    public ILocator GetStepStatusTag(string stepTitle) =>
        GetStep(stepTitle).Locator("strong.status-tag");

    public ILocator GetStepPanel(string stepTitle) =>
        GetStep(stepTitle).Locator("div.app-step-nav__panel");

    public Task ShowAllStepsAsync() => ShowOrHideAllStepsButton.ClickAsync();

    public Task ExpandStepAsync(string stepTitle) => GetStepToggleButton(stepTitle).ClickAsync();

    public Task<bool> HasStatusAsync(string stepTitle) =>
        GetStepStatusTag(stepTitle).IsVisibleAsync();

    public Task<string> GetStepStatusTextAsync(string stepTitle) =>
        GetTrimmedTextAsync(GetStepStatusTag(stepTitle));

    public Task<string> GetStepPanelTextAsync(string stepTitle) =>
        GetTrimmedTextAsync(GetStepPanel(stepTitle));
}
