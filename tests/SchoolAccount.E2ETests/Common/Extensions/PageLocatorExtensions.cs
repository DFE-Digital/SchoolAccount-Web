using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Extensions;

public static class PageLocatorExtensions
{
    public static ILocator InputLocatorById(this IPage page, string id) =>
        page.Locator($"input[id='{id}']");

    public static ILocator InputLocatorByName(this IPage page, string name) =>
        page.Locator($"input[name='{name}']");

    public static ILocator ButtonLocatorById(this IPage page, string id) =>
        page.Locator($"button[id='{id}']");
}
