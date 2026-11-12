using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SchoolAccount.E2ETests.Common.Pages;

public abstract class BasePage
{
    protected ILocator Title =>
        PageContext.Locator("h1.govuk-heading-l,h1.govuk-heading-xl,h1.dfe-heading-l");

    protected BasePage(IPage pageContext)
    {
        PageContext = pageContext;
    }

    protected IPage PageContext { get; }

    public async Task AssertTitle(string expectedTitle) =>
        await Expect(Title).ToContainTextAsync(expectedTitle);
}
