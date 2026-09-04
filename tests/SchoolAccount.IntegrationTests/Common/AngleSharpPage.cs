using AngleSharp;
using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Common;

public abstract class AngleSharpPage
{
    protected IDocument Page;

    protected AngleSharpPage() { }

    protected AngleSharpPage(string pageContent)
    {
        Initialise(pageContent);
    }

    protected void Initialise(string pageContent)
    {
        var context = BrowsingContext.New(Configuration.Default);
        Page = context
            .OpenAsync(req => req.Content(pageContent), TestContext.Current.CancellationToken)
            .Result;
    }

    public static async Task<T> FromResponseAsync<T>(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default
    )
        where T : AngleSharpPage, new()
    {
        var html = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        var page = new T();
        page.Initialise(html);

        return page;
    }

    public virtual string? GetTitle()
    {
        var pageTitle = Page.QuerySelector("title");
        return pageTitle?.TextContent;
    }

    public virtual string? GetFirstHeading()
    {
        var headingElement = Page.QuerySelector("h1.govuk-heading-l");
        return headingElement?.TextContent;
    }

    public virtual string? GetFirstBody()
    {
        var bodyElement = Page.QuerySelector("body");
        return bodyElement?.TextContent;
    }

    public virtual string? GetFirstBodyParagraph()
    {
        var bodyElement = Page.QuerySelector("p.govuk-body");
        return bodyElement?.TextContent;
    }

    public virtual string? GetFirstCaption()
    {
        var captionElement = Page.QuerySelector("span.govuk-caption-xl");
        return captionElement?.TextContent;
    }

    public virtual string? GetSignOutLink()
    {
        var signOutElement = Page.QuerySelector("button");
        return signOutElement?.TextContent;
    }

    public virtual string? GetOrganisationName()
    {
        var organisationNameElement = Page.QuerySelector(".header-navigation__school");
        return organisationNameElement?.TextContent;
    }

    public virtual string GetFirstTag()
    {
        var tagElement = Page.QuerySelector(".govuk-tag");
        return tagElement?.TextContent;
    }

    public virtual IReadOnlyDictionary<string, string> GetSummaryListPairs()
    {
        return Page.QuerySelectorAll("dl.govuk-summary-list div.govuk-summary-list__row")
            .ToDictionary(
                row => row.QuerySelector(".govuk-summary-list__key")!.TextContent.Trim(),
                row => row.QuerySelector(".govuk-summary-list__value")!.TextContent.Trim()
            );
    }

    public virtual IReadOnlyList<(string Key, string Value)> GetSummaryListRows()
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

    public virtual IElement? GetFooterLink(string href)
    {
        return Page.QuerySelector($".govuk-footer a[href='{href}']");
    }

    public virtual IElement? GetFooter()
    {
        return Page.QuerySelector(".govuk-footer");
    }

    public virtual IElement? GetButtonByLink(string href)
    {
        return Page.QuerySelector($"a.govuk-button[href='{href}']");
    }
}
