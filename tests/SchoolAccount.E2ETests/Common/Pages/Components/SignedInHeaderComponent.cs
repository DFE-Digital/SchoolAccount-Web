using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>The header on every signed-in page: the current school's name and Sign out. From <c>_Layout.cshtml</c>.</summary>
public class SignedInHeaderComponent(IPage page) : BaseComponent(page.Locator("header"))
{
    public ILocator SchoolName => Root.Locator("span.header-navigation__school");
    public ILocator SignOutButton => Root.GetByRole(AriaRole.Button, new() { Name = "Sign out" });

    public Task<string> GetSchoolNameAsync()
    {
        return GetTrimmedTextAsync(SchoolName);
    }

    public Task SignOutAsync()
    {
        return SignOutButton.ClickAsync();
    }
}
