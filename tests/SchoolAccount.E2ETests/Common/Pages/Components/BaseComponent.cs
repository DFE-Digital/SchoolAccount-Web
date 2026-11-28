using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Pages.Components;

/// <summary>
/// A part of a page. Every locator in a component is found inside <see cref="Root"/>, so the same
/// class name elsewhere on the page can't be picked up by mistake.
/// </summary>
public abstract class BaseComponent(ILocator root)
{
    protected ILocator Root { get; } = root;

    protected static async Task<string> GetTrimmedTextAsync(ILocator locator)
    {
        return (await locator.TextContentAsync())?.Trim() ?? string.Empty;
    }
}
