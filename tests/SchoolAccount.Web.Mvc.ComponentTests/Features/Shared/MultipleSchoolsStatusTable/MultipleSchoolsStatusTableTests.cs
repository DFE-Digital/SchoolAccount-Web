using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;
using Shouldly;
using static SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable.MultipleSchoolsStatusTableViewModel;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Shared.MultipleSchoolsStatusTable;

public sealed class MultipleSchoolsStatusTableTests(ComponentRenderer renderer)
    : IClassFixture<ComponentRenderer>
{
    private const string ViewPath =
        "~/Features/Shared/MultipleSchoolsStatusTable/MultipleSchoolsStatusTable.cshtml";

    // Table contents

    [Fact]
    public async Task Table_caption_explains_the_schools_belong_to_the_trust()
    {
        var model = ATableOf();

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelector("caption")
            .ShouldNotBeNull()
            .TextContent.Trim()
            .ShouldBe("Schools in your trust");
    }

    [Fact]
    public async Task Table_has_a_column_for_each_school_detail()
    {
        var model = ATableOf();

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetHeaders(html)
            .Select(header => header.TextContent.Trim())
            .ShouldBe(["Name", "Status", "Last Changed", "Errors", "Queries"]);
    }

    [Fact]
    public async Task Each_school_is_shown_as_a_row_with_all_its_details()
    {
        var model = ATableOf(
            ASchool("Oakwood Primary", "Submitted", updatedAt: "2 days ago", errors: 0, queries: 3),
            ASchool("Riverside Academy", "Not started", updatedAt: "Today", errors: 5, queries: 1)
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

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
        // Missing data must render blank, not "0" (which would mean "checked, and none").
        var model = ATableOf(
            ASchool("Oakwood Primary", "Not started", updatedAt: "", errors: null, queries: null)
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetRowCells(html)
            .ShouldHaveSingleItem()
            .ShouldBe(["Oakwood Primary", "Not started", "", "", ""]);
    }

    [Fact]
    public async Task Table_body_is_empty_when_the_trust_has_no_schools()
    {
        var model = ATableOf();

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll("tbody tr").ShouldBeEmpty();
    }

    [Fact]
    public async Task School_names_are_shown_as_text_rather_than_html()
    {
        var model = ATableOf(ASchool("St Mary & St John <Primary>", "Submitted"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        var nameCell = GetRowCells(html).ShouldHaveSingleItem()[0];
        nameCell.ShouldBe("St Mary & St John <Primary>");
    }

    // Styling

    [Fact]
    public async Task Rows_use_the_govuk_table_row_style()
    {
        var model = ATableOf(ASchool("Oakwood Primary", "Submitted"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll("tr")
            .Select(row => row.ClassName)
            .ShouldBe(["govuk-table__row", "govuk-table__row"]);
    }

    // Accessibility and sorting

    [Fact]
    public async Task Column_headers_are_announced_as_column_headers_by_screen_readers()
    {
        var model = ATableOf();

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetHeaders(html)
            .Select(header => header.GetAttribute("scope"))
            .ShouldBe(["col", "col", "col", "col", "col"]);
    }

    [Fact]
    public async Task Table_is_sortable()
    {
        var model = ATableOf();

        using var html = await renderer.RenderAsync(ViewPath, model);

        var table = html.QuerySelector("table").ShouldNotBeNull();
        table.GetAttribute("data-module").ShouldBe("moj-sortable-table");
    }

    [Fact]
    public async Task Status_column_is_marked_as_sorted_descending()
    {
        var model = ATableOf();

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetHeaders(html)
            .Select(header => header.GetAttribute("aria-sort"))
            .ShouldBe(["none", "descending", "none", "none", "none"]);
    }

    // Helpers

    private static MultipleSchoolsStatusTableViewModel ATableOf(params SchoolStatus[] schools)
    {
        return new MultipleSchoolsStatusTableViewModel { SchoolStatuses = schools };
    }

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

    private static string[][] GetRowCells(IDocument html)
    {
        return html.QuerySelectorAll("tbody tr")
            .Select(row =>
                row.QuerySelectorAll("td").Select(cell => cell.TextContent.Trim()).ToArray()
            )
            .ToArray();
    }
}
