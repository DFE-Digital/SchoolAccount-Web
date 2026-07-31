using AngleSharp;
using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Controllers.Home;

public class AngleSharpPage
{
    private IDocument _page;

    public async Task Parse(string pageContent)
    {
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        _page = await context.OpenAsync(
            req => req.Content(pageContent),
            TestContext.Current.CancellationToken
        );
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
