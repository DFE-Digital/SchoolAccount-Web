using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Pages;

public class StartPage(IPage page) : BasePage(page)
{
    public async Task NavigateAsync(string baseUrl) => await PageContext.GotoAsync(baseUrl);

    public async Task<string> GetTitleAsync() => await PageContext.TitleAsync();
}
