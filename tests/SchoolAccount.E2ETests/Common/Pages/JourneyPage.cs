using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Pages.Components;
using SchoolAccount.TestCommon.Constants;

namespace SchoolAccount.E2ETests.Common.Pages;

/// <summary>The Journey page for a census return (<see cref="UrlConstants.JourneyUrl"/>). Everything on it is reached through its components.</summary>
public class JourneyPage(IPage page) : SchoolAccountBasePage(page)
{
    public JourneyComponent Journey { get; } = new(page);
    public StepByStepComponent StepByStep { get; } = new(page);
    public ImportantDatesComponent ImportantDates { get; } = new(page);
}
