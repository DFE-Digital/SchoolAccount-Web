using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Common.Pages;

public class JourneyPage : AngleSharpPage
{
    public virtual JourneyStepsElement GetStepsComponent()
    {
        return new JourneyStepsElement(
            Page.QuerySelector("div.app-step-nav[data-module='appstepnav']")
        );
    }

    public virtual string GetFirstTag()
    {
        var tagElement = Page.QuerySelector(".govuk-tag");
        return tagElement?.TextContent;
    }

    public virtual IReadOnlyDictionary<string, string> GetImportantDateSummaryListPairs()
    {
        return Page.QuerySelectorAll("dl.govuk-summary-list div.govuk-summary-list__row")
            .ToDictionary(
                row => row.QuerySelector(".govuk-summary-list__key")!.TextContent.Trim(),
                row => row.QuerySelector(".govuk-summary-list__value")!.TextContent.Trim()
            );
    }

    public virtual IReadOnlyList<(string Key, string Value)> GetImportantDateSummaryListRows()
    {
        return Page.QuerySelectorAll("dl.govuk-summary-list div.govuk-summary-list__row")
            .Select(row =>
                (
                    Key: row.QuerySelector(".govuk-summary-list__key")!.TextContent.Trim(),
                    Value: row.QuerySelector(".govuk-summary-list__value")!.TextContent.Trim()
                )
            )
            .ToList();
    }

    public virtual IReadOnlyList<string> GetSchoolStatusTableHeaders()
    {
        return Page.QuerySelectorAll("table.govuk-table thead th.govuk-table__header")
            .Select(header => header.TextContent.Trim())
            .ToList();
    }

    public virtual IReadOnlyList<IReadOnlyList<string>> GetSchoolStatusTableRows()
    {
        return Page.QuerySelectorAll("table.govuk-table tbody tr")
            .Select(row =>
                (IReadOnlyList<string>)
                    row.QuerySelectorAll("td.govuk-table__cell")
                        .Select(cell => cell.TextContent.Trim())
                        .ToList()
            )
            .ToList();
    }

    public virtual IReadOnlyList<
        IReadOnlyDictionary<string, string>
    > GetSchoolStatusTableCellByHeader()
    {
        var headers = GetSchoolStatusTableHeaders();

        return Page.QuerySelectorAll("table.govuk-table tbody tr")
            .Select(row =>
                (IReadOnlyDictionary<string, string>)
                    row.QuerySelectorAll("td.govuk-table__cell")
                        .Select(
                            (cell, index) =>
                                new
                                {
                                    Key = index < headers.Count ? headers[index] : $"Column{index}",
                                    Value = cell.TextContent.Trim(),
                                }
                        )
                        .ToDictionary(cell => cell.Key, cell => cell.Value)
            )
            .ToList();
    }

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
