using AngleSharp;
using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Common;

public class AngleSharpPage
{
    private readonly IDocument _page;

    public AngleSharpPage(string pageContent)
    {
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        _page = context.OpenAsync(
            req => req.Content(pageContent),
            TestContext.Current.CancellationToken
        ).Result;
    }

    public string? GetTitle()
    {
        IElement? pageTitle = _page.QuerySelector("title");
        return pageTitle?.TextContent;
    }

    public string? GetFirstHeading()
    {
        IElement? headingElement = _page.QuerySelector("h1.govuk-heading-l");
        return headingElement?.TextContent;
    }

    public string? GetFirstBody()
    {
        IElement? bodyElement = _page.QuerySelector("body");
        return bodyElement?.TextContent;
    }
}
