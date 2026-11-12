using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Pages.Components;

namespace SchoolAccount.E2ETests.Common.Pages;

/// <summary>Base page for authenticated School Account pages, holding the chrome common to all of them.</summary>
public class SchoolAccountBasePage(IPage page) : BasePage(page)
{
    public SignedInHeaderComponent Header { get; } = new(page);
}
