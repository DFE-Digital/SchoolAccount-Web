using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace SchoolAccount.IntegrationTests.Common;

public abstract class AngleSharpPage
{
    protected IDocument Page = null!;

    protected AngleSharpPage() { }

    protected AngleSharpPage(string pageContent)
    {
        Initialise(pageContent);
    }

    protected string? GetText(string selector)
    {
        return TextOf(Page.QuerySelector(selector));
    }

    protected static string Normalise(string text)
    {
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    protected static string? TextOf(IElement? element)
    {
        return element is null ? null : Normalise(element.TextContent);
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

    public string? GetSelectorByText(string selector, string text)
    {
        return Page.QuerySelectorAll(selector)
            .Select(TextOf)
            .FirstOrDefault(content => content!.Contains(text));
    }

    public IElement? GetElementByLink(string selector, string href)
    {
        return Page.QuerySelectorAll(selector).FirstOrDefault(e => e.GetAttribute("href") == href);
    }

    public string? GetTitle()
    {
        return GetText("title");
    }

    public string? GetFirstHeading()
    {
        return GetText("h1.govuk-heading-l");
    }

    public string? GetFirstBody()
    {
        return GetText("body");
    }

    public string? GetFirstBodyParagraph()
    {
        return GetText("p.govuk-body");
    }

    public string? GetOrganisationName()
    {
        return GetText(".header-navigation__school");
    }

    public IElement? GetFooter()
    {
        return Page.QuerySelector(".govuk-footer");
    }

    public string? GetHiddenTextOnElement(IElement? element)
    {
        return TextOf(element?.QuerySelector(".govuk-visually-hidden"));
    }

    public string? GetTextOf(IElement? element)
    {
        return TextOf(element);
    }

    private void Initialise(string pageContent)
    {
        Page = new HtmlParser().ParseDocument(pageContent);
    }
}
