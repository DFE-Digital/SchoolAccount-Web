using System.Net;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Error;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Error;

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
        var model = new ErrorViewModel(statusCode);

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelector("h1").ShouldNotBeNull().TextContent.Trim().ShouldBe(expectedHeading);
    }

    [Fact]
    public async Task Page_not_found_suggests_checking_the_web_address()
    {
        var model = new ErrorViewModel(HttpStatusCode.NotFound);

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.Body.ShouldNotBeNull()
            .TextContent.ShouldContain("If you typed the web address, check it is correct.");
    }

    [Fact]
    public async Task Access_denied_explains_the_user_lacks_permission()
    {
        var model = new ErrorViewModel(HttpStatusCode.Forbidden);

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.Body.ShouldNotBeNull()
            .TextContent.ShouldContain("You do not have permission to view this page.");
    }

    [Fact]
    public async Task Unexpected_error_asks_the_user_to_try_again_later()
    {
        var model = new ErrorViewModel(HttpStatusCode.InternalServerError);

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.Body.ShouldNotBeNull().TextContent.ShouldContain("Try again later.");
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task Every_error_links_back_to_the_homepage(HttpStatusCode statusCode)
    {
        var model = new ErrorViewModel(statusCode);

        using var html = await renderer.RenderAsync(ViewPath, model);

        var link = html.QuerySelector("a.govuk-link").ShouldNotBeNull();
        link.TextContent.Trim().ShouldBe("Go to the homepage");
        link.GetAttribute("href").ShouldBe("/Journey");
    }
}
