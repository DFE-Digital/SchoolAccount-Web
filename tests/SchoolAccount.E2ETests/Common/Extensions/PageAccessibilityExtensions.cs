using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Extensions;

public static class PageAccessibilityExtensions
{
    // The axe-core rules that check an element has a name a screen reader can announce.
    private static readonly List<string> AccessibleNameRules =
    [
        "aria-command-name",
        "aria-input-field-name",
        "aria-toggle-field-name",
        "button-name",
        "image-alt",
        "input-button-name",
        "input-image-alt",
        "label",
        "link-name",
        "select-name",
        "summary-name",
    ];

    /// <summary>
    /// Describes each visible element on the page that has no accessible name, one entry per element.
    /// </summary>
    public static async Task<IReadOnlyList<string>> FindElementsWithoutAccessibleNamesAsync(
        this IPage page
    )
    {
        var options = new AxeRunOptions
        {
            RunOnly = new RunOnlyOptions { Type = "rule", Values = AccessibleNameRules },
        };

        var result = await page.RunAxe(options);

        return result
            .Violations.SelectMany(violation =>
                violation.Nodes.Select(element => Describe(violation, element))
            )
            .ToList();
    }

    private static string Describe(AxeResultItem violation, AxeResultNode element) =>
        $"""

            {violation.Help} ({violation.Id})
              Element:  {element.Html}
              Selector: {element.Target}
              Guidance: {violation.HelpUrl}
            """;
}
