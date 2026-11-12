using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Extensions;
using SchoolAccount.E2ETests.Common.Pages;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>The signed-in header shown on every authenticated School Account page: current school name and sign out.</summary>
public class SignedInHeaderComponent(IPage page) : BasePage(page)
{
    public ILocator SchoolName => PageContext.SpanLocatorByClass("header-navigation__school");
    public ILocator SignOutButton => PageContext.ButtonLocatorByText("Sign out");
    public ILocator ServiceNameLink => PageContext.AnchorLocatorByText("School Account");

    public async Task<string> GetSchoolNameAsync() =>
        (await SchoolName.TextContentAsync())?.Trim() ?? string.Empty;

    public async Task SignOutAsync() => await SignOutButton.ClickAsync();
}
