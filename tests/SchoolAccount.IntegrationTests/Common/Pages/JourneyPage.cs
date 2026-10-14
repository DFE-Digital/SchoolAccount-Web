using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Common.Pages;

public class JourneyPage : AngleSharpPage
{
    public string? Caption => TextOf("span.govuk-caption-xl");

    public string? Status => TextOf(".govuk-tag");

    public string? Overview => TextOf("p.govuk-body");

    public IElement? ButtonLink(string href)
    {
        return Document.QuerySelector($"a.govuk-button[href='{href}']");
    }

    /// <summary>
    /// The important dates summary list, as (label, date) pairs in the order shown.
    /// </summary>
    public IReadOnlyList<(string Label, string Date)> ImportantDates =>
        Document
            .QuerySelectorAll("dl.govuk-summary-list div.govuk-summary-list__row")
            .Select(row =>
                (
                    Label: row.QuerySelector(".govuk-summary-list__key")!.TextContent.Trim(),
                    Date: row.QuerySelector(".govuk-summary-list__value")!.TextContent.Trim()
                )
            )
            .ToList();

    /// <summary>
    /// The school statuses table shown to multi-academy trusts, or null if it isn't on the page.
    /// </summary>
    public HtmlTable? SchoolStatuses
    {
        get
        {
            var table = Document.QuerySelector("table.govuk-table");
            if (table is null)
            {
                return null;
            }

            var headers = table
                .QuerySelectorAll("thead th.govuk-table__header")
                .Select(header => header.TextContent.Trim())
                .ToList();

            var rows = table
                .QuerySelectorAll("tbody tr")
                .Select(row =>
                    (IReadOnlyDictionary<string, string>)
                        row.QuerySelectorAll("td.govuk-table__cell")
                            .Select(
                                (cell, index) =>
                                    (
                                        Key: index < headers.Count
                                            ? headers[index]
                                            : $"Column{index}",
                                        Value: cell.TextContent.Trim()
                                    )
                            )
                            .ToDictionary(cell => cell.Key, cell => cell.Value)
                )
                .ToList();

            return new HtmlTable(headers, rows);
        }
    }

    public JourneyStepsElement Steps =>
        new(Document.QuerySelector("div.app-step-nav[data-module='appstepnav']"));

    public class JourneyStepsElement(IElement? wrapper)
    {
        public bool IsPresent => wrapper is not null;

        public List<JourneyStepElement> GetSteps()
        {
            return wrapper?.QuerySelectorAll("li").Select(x => new JourneyStepElement(x)).ToList()
                ?? [];
        }

        public class JourneyStepElement(IElement element)
        {
            public string? GetTitle()
            {
                return element.QuerySelector("span.js-step-title")?.TextContent;
            }
        }
    }
}

/// <summary>
/// A table's column headings, and each row's cells keyed by its column heading.
/// </summary>
public sealed record HtmlTable(
    IReadOnlyList<string> Headers,
    IReadOnlyList<IReadOnlyDictionary<string, string>> Rows
);
