using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Dashboard;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Dashboard;

/// <summary>
/// Tests for the dashboard page (Dashboard.cshtml).
/// </summary>
/// <remarks>
/// The page a user lands on after signing in. It greets them by name and lists their census
/// statuses.
/// </remarks>
public sealed class DashboardTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Dashboard/Dashboard.cshtml";

    [Fact]
    public async Task User_is_greeted_by_name()
    {
        // Arrange
        var model = new DashboardViewModel("Jo Bloggs", []);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelector("h1").ShouldNotBeNull().TextContent.Trim().ShouldBe("Hello Jo Bloggs");
    }

    [Fact]
    public async Task Users_name_is_shown_as_text_rather_than_html()
    {
        // Arrange
        // Razor should encode the name so it is shown as text and never runs. This is a basic
        // protection against cross-site scripting (XSS).
        var model = new DashboardViewModel("<script>alert('hi')</script>", []);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var heading = html.QuerySelector("h1").ShouldNotBeNull();
        heading.TextContent.Trim().ShouldBe("Hello <script>alert('hi')</script>");
        heading.QuerySelector("script").ShouldBeNull();
    }

    [Fact]
    public async Task Each_census_status_is_shown()
    {
        // Arrange
        var model = new DashboardViewModel(
            "Jo Bloggs",
            ["Autumn: Submitted", "Spring: Not started"]
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var statuses = html.QuerySelector("p.govuk-body").ShouldNotBeNull().TextContent;
        statuses.ShouldContain("Autumn: Submitted");
        statuses.ShouldContain("Spring: Not started");
    }
}
