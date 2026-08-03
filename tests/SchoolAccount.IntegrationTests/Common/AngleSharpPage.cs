using AngleSharp;
using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Common;

public class AngleSharpPage
{
    private readonly IDocument _page;

    public AngleSharpPage(string pageContent)
    {
        var context = BrowsingContext.New(Configuration.Default);
        _page = context
            .OpenAsync(req => req.Content(pageContent), TestContext.Current.CancellationToken)
            .Result;
    }

    public string? GetTitle()
    {
        var pageTitle = _page.QuerySelector("title");
        return pageTitle?.TextContent;
    }

    public string? GetFirstHeading()
    {
        var headingElement = _page.QuerySelector("h1.govuk-heading-l");
        return headingElement?.TextContent;
    }

    public string? GetFirstBody()
    {
        var bodyElement = _page.QuerySelector("body");
        return bodyElement?.TextContent;
    }
}
