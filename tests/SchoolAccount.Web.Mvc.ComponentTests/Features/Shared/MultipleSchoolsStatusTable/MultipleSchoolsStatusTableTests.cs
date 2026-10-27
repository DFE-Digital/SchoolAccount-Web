using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;
using Shouldly;
using static SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable.MultipleSchoolsStatusTableViewModel;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Shared.MultipleSchoolsStatusTable;

/// <summary>
/// Tests for the table of schools and their statuses (MultipleSchoolsStatusTable.cshtml).
/// </summary>
/// <remarks>
/// Users who look after more than one school, such as a multi-academy trust (MAT) or local
/// authority, see this table instead of a single status. It is sortable by clicking a column
/// header, using the Ministry of Justice (MOJ) "sortable table" JavaScript.
/// </remarks>
public sealed class MultipleSchoolsStatusTableTests(ComponentRenderer renderer)
    : IClassFixture<ComponentRenderer>
{
    private const string ViewPath =
        "~/Features/Shared/MultipleSchoolsStatusTable/MultipleSchoolsStatusTable.cshtml";

    // Table contents

    [Fact]
    public async Task Table_caption_explains_the_schools_belong_to_the_trust()
    {
        // Arrange
        var model = ATableOf();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelector("caption")
            .ShouldNotBeNull()
            .TextContent.Trim()
            .ShouldBe("Schools in your trust");
    }

    [Fact]
    public async Task Table_has_a_column_for_each_school_detail()
    {
        // Arrange
        var model = ATableOf();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetHeaders(html)
            .Select(header => header.TextContent.Trim())
            .ShouldBe(["Name", "Status", "Last Changed", "Errors", "Queries"]);
    }

    [Fact]
    public async Task Each_school_is_shown_as_a_row_with_all_its_details()
    {
        // Arrange
        var model = ATableOf(
            ASchool("Oakwood Primary", "Submitted", updatedAt: "2 days ago", errors: 0, queries: 3),
            ASchool("Riverside Academy", "Not started", updatedAt: "Today", errors: 5, queries: 1)
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // Each inner list is one row, in column order: [name, status, last changed, errors, queries].
        GetRowCells(html)
            .ShouldBe([
                ["Oakwood Primary", "Submitted", "2 days ago", "0", "3"],
                ["Riverside Academy", "Not started", "Today", "5", "1"],
            ]);
    }

    [Fact]
    public async Task Missing_details_are_shown_as_empty_cells()
    {
        // Arrange
        // Errors and Queries are null when there's no data, and UpdatedAt is an empty string
        // when the school has never changed. In both cases the cell should be blank. In
        // particular, "no data" must not show as "0", which would mean "checked, and none".
        var model = ATableOf(
            ASchool("Oakwood Primary", "Not started", updatedAt: "", errors: null, queries: null)
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetRowCells(html)
            .ShouldHaveSingleItem()
            .ShouldBe(["Oakwood Primary", "Not started", "", "", ""]);
    }

    [Fact]
    public async Task Table_body_is_empty_when_the_trust_has_no_schools()
    {
        // Arrange
        var model = ATableOf();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelectorAll("tbody tr").ShouldBeEmpty();
    }

    [Fact]
    public async Task School_names_are_shown_as_text_rather_than_html()
    {
        // Arrange
        // If Razor didn't encode "<", the browser would read "<Primary>" as an element and the
        // name would lose that part.
        var model = ATableOf(ASchool("St Mary & St John <Primary>", "Submitted"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var nameCell = GetRowCells(html).ShouldHaveSingleItem()[0];
        nameCell.ShouldBe("St Mary & St John <Primary>");
    }

    // Styling

    [Fact]
    public async Task Rows_use_the_govuk_table_row_style()
    {
        // Arrange
        var model = ATableOf(ASchool("Oakwood Primary", "Submitted"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // One header row and one body row. Listing them stops the test passing when no rows
        // are found at all.
        html.QuerySelectorAll("tr")
            .Select(row => row.ClassName)
            .ShouldBe(["govuk-table__row", "govuk-table__row"]);
    }

    // Accessibility and sorting

    [Fact]
    public async Task Column_headers_are_announced_as_column_headers_by_screen_readers()
    {
        // Arrange
        var model = ATableOf();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // scope="col" tells screen readers each header labels the column below it, so they
        // can announce "Status: Submitted" rather than just "Submitted".
        GetHeaders(html)
            .Select(header => header.GetAttribute("scope"))
            .ShouldBe(["col", "col", "col", "col", "col"]);
    }

    [Fact]
    public async Task Table_is_sortable()
    {
        // Arrange
        var model = ATableOf();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The MOJ JavaScript makes any table with this data-module attribute sortable.
        var table = html.QuerySelector("table").ShouldNotBeNull();
        table.GetAttribute("data-module").ShouldBe("moj-sortable-table");
    }

    [Fact]
    public async Task Table_is_initially_sorted_by_status()
    {
        // Arrange
        var model = ATableOf();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // aria-sort tells screen readers (and the sorting JavaScript) which column is sorted.
        GetHeaders(html)
            .Select(header => header.GetAttribute("aria-sort"))
            .ShouldBe(["none", "descending", "none", "none", "none"]);
    }

    // Helpers

    /// <summary>
    /// Builds the table's view model containing the given schools. Pass none for an empty table.
    /// </summary>
    private static MultipleSchoolsStatusTableViewModel ATableOf(params SchoolStatus[] schools)
    {
        return new MultipleSchoolsStatusTableViewModel { SchoolStatuses = schools };
    }

    /// <summary>
    /// A row in the table. Only name and status are required. The other details are optional,
    /// so tests that don't care about them can leave them out.
    /// </summary>
    private static SchoolStatus ASchool(
        string name,
        string status,
        string updatedAt = "",
        int? errors = null,
        int? queries = null
    )
    {
        return new SchoolStatus
        {
            Name = name,
            Status = status,
            UpdatedAt = updatedAt,
            Errors = errors,
            Queries = queries,
        };
    }

    private static IHtmlCollection<IElement> GetHeaders(IDocument html)
    {
        return html.QuerySelectorAll("th");
    }

    /// <summary>
    /// Reads the table body into a grid of text, one inner array per row,
    /// e.g. [["Oakwood Primary", "Submitted", "2 days ago", "0", "3"], [...]].
    /// </summary>
    private static string[][] GetRowCells(IDocument html)
    {
        return html.QuerySelectorAll("tbody tr")
            .Select(row =>
                row.QuerySelectorAll("td").Select(cell => cell.TextContent.Trim()).ToArray()
            )
            .ToArray();
    }
}
