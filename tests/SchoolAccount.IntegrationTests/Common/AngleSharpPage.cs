using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace SchoolAccount.IntegrationTests.Common;

public abstract class AngleSharpPage
{
    protected IDocument Document = null!;

    protected AngleSharpPage() { }

    protected AngleSharpPage(string pageContent)
    {
        Initialise(pageContent);
    }

    protected string? GetText(string selector)
    {
        return TextOf(Document.QuerySelector(selector));
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

        var Document = new T();
        Document.Initialise(html);

        return Document;
    }

    public string? GetComponentTextByContent(string querySelector, string text)
    {
        return Document
            .QuerySelectorAll(querySelector)
            .Select(TextOf)
            .FirstOrDefault(content => content!.Contains(text));
    }

    public IElement? GetComponentByContent(string querySelector, string content)
    {
        return Document
            .QuerySelectorAll(querySelector)
            .FirstOrDefault(h => h.TextContent?.Contains(content) ?? false);
    }

    public IElement? GetElementByLink(string selector, string href)
    {
        return Document
            .QuerySelectorAll(selector)
            .FirstOrDefault(e => e.GetAttribute("href") == href);
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
        return Document.QuerySelector(".govuk-footer");
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
        Document = new HtmlParser().ParseDocument(pageContent);
    }
}
