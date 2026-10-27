using System.Net;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Error;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Error;

/// <summary>
/// Tests for the error page (Error.cshtml).
/// </summary>
/// <remarks>
/// The page shows one of three messages depending on the HTTP status code: "not found"
/// (404), "access denied" (403), or a general "problem with the service" for anything else.
/// </remarks>
public sealed class ErrorTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Error/Error.cshtml";

    [Theory]
    [InlineData(HttpStatusCode.NotFound, "Page not found")]
    [InlineData(HttpStatusCode.Forbidden, "Access denied")]
    [InlineData(HttpStatusCode.InternalServerError, "Sorry, there is a problem with the service")]
    [InlineData(HttpStatusCode.BadRequest, "Sorry, there is a problem with the service")]
    public async Task Heading_explains_the_error(HttpStatusCode statusCode, string expectedHeading)
    {
        // Arrange
        var model = new ErrorViewModel(statusCode);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelector("h1").ShouldNotBeNull().TextContent.Trim().ShouldBe(expectedHeading);
    }

    [Fact]
    public async Task Page_not_found_suggests_checking_the_web_address()
    {
        // Arrange
        var model = new ErrorViewModel(HttpStatusCode.NotFound);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.Body.ShouldNotBeNull()
            .TextContent.ShouldContain("If you typed the web address, check it is correct.");
    }

    [Fact]
    public async Task Access_denied_explains_the_user_lacks_permission()
    {
        // Arrange
        var model = new ErrorViewModel(HttpStatusCode.Forbidden);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.Body.ShouldNotBeNull()
            .TextContent.ShouldContain("You do not have permission to view this page.");
    }

    [Fact]
    public async Task Unexpected_error_asks_the_user_to_try_again_later()
    {
        // Arrange
        var model = new ErrorViewModel(HttpStatusCode.InternalServerError);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.Body.ShouldNotBeNull().TextContent.ShouldContain("Try again later.");
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task Every_error_links_back_to_the_homepage(HttpStatusCode statusCode)
    {
        // Arrange
        var model = new ErrorViewModel(statusCode);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The view builds this link with Url.Action("Dashboard", "Dashboard"). The renderer
        // uses the app's real routes, and DashboardController has [Route("/{action}")], so
        // the URL is "/Dashboard". If that route changes, this test will tell us.
        var link = html.QuerySelector("a.govuk-link").ShouldNotBeNull();
        link.TextContent.Trim().ShouldBe("Go to the homepage");
        link.GetAttribute("href").ShouldBe("/Dashboard");
    }
}
