using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Common.Pages;

public class JourneyPage : AngleSharpPage
{
    public JourneyStepsElement GetStepsComponent()
    {
        return new JourneyStepsElement(
            Page.QuerySelector("div.app-step-nav[data-module='appstepnav']")
        );
    }

    public string? GetFirstTag()
    {
        return GetText(".govuk-tag");
    }

    public string? GetFirstCaption()
    {
        return GetText("span.govuk-caption-xl");
    }

    public IReadOnlyList<(string Key, string Value)> GetImportantDateSummaryListRows()
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

    public IReadOnlyDictionary<string, string> GetImportantDateSummaryListPairs()
    {
        return GetImportantDateSummaryListRows().ToDictionary(row => row.Key, row => row.Value);
    }

    public IReadOnlyList<string> GetSchoolStatusTableHeaders()
    {
        return Page.QuerySelectorAll("table.govuk-table thead th.govuk-table__header")
            .Select(header => Normalise(header.TextContent))
            .ToList();
    }

    public IReadOnlyList<IReadOnlyList<string>> GetSchoolStatusTableRows()
    {
        return Page.QuerySelectorAll("table.govuk-table tbody tr")
            .Select(row =>
                (IReadOnlyList<string>)
                    row.QuerySelectorAll("td.govuk-table__cell")
                        .Select(cell => Normalise(cell.TextContent))
                        .ToList()
            )
            .ToList();
    }

    public IReadOnlyList<IReadOnlyDictionary<string, string>> GetSchoolStatusTableCellByHeader()
    {
        var headers = GetSchoolStatusTableHeaders();

        return GetSchoolStatusTableRows()
            .Select(cells =>
                (IReadOnlyDictionary<string, string>)
                    cells
                        .Select(
                            (value, index) =>
                                (
                                    Key: index < headers.Count ? headers[index] : $"Column{index}",
                                    Value: value
                                )
                        )
                        .ToDictionary(cell => cell.Key, cell => cell.Value)
            )
            .ToList();
    }

    public class JourneyStepsElement(IElement? wrapper)
    {
        public bool IsPresent => wrapper is not null;

        public IReadOnlyList<JourneyStepElement> GetSteps()
        {
            return wrapper?.QuerySelectorAll("li").Select(li => new JourneyStepElement(li)).ToList()
                ?? [];
        }

        public class JourneyStepElement(IElement element)
        {
            public string? GetTitle()
            {
                return TextOf(element.QuerySelector("span.js-step-title"));
            }
        }
    }
}
