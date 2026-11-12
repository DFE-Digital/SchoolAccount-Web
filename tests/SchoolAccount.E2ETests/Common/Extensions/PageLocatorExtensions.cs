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

    public static ILocator ButtonLocatorByText(this IPage page, string text) =>
        page.Locator("button").Filter(new() { HasText = text });

    public static ILocator AnchorLocatorByText(this IPage page, string linkText) =>
        page.Locator("a").Filter(new() { HasText = linkText.Trim() });

    public static ILocator SpanLocatorByClass(this IPage page, string className) =>
        page.Locator($"span.{className}");
}
