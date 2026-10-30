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

    public static async Task<T> FromResponseAsync<T>(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default
    )
        where T : AngleSharpPage, new()
    {
        var html = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        var document = new T();
        document.Initialise(html);

        return document;
    }

    public string? GetElementContentByText(string querySelector, string text)
    {
        return GetNormalisedText(GetElementContainingText(querySelector, text));
    }

    public IElement? GetElementContainingText(string querySelector, string text)
    {
        return Document
            .QuerySelectorAll(querySelector)
            .FirstOrDefault(element => element.TextContent.Contains(text));
    }

    public IElement? GetElementByHref(string querySelector, string href)
    {
        return Document
            .QuerySelectorAll(querySelector)
            .FirstOrDefault(e => e.GetAttribute("href") == href);
    }

    public string? GetPageTitle()
    {
        return GetTextBySelector("title");
    }

    public string? GetFirstHeading()
    {
        return GetTextBySelector("h1.govuk-heading-l");
    }

    public string? GetFirstBody()
    {
        return GetTextBySelector("body");
    }

    public string? GetFirstBodyParagraph()
    {
        return GetTextBySelector("p.govuk-body");
    }

    public string? GetOrganisationName()
    {
        return GetTextBySelector(".header-navigation__school");
    }

    public IElement? GetFooter()
    {
        return Document.QuerySelector(".govuk-footer");
    }

    public string? GetVisuallyHiddenTextOnElement(IElement? element)
    {
        return GetNormalisedText(element?.QuerySelector(".govuk-visually-hidden"));
    }

    public string? GetTextOf(IElement? element)
    {
        return GetNormalisedText(element);
    }

    protected string? GetTextBySelector(string selector)
    {
        return GetNormalisedText(Document.QuerySelector(selector));
    }

    protected static string Normalise(string text)
    {
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    protected static string? GetNormalisedText(IElement? element)
    {
        return element is null ? null : Normalise(element.TextContent);
    }

    private void Initialise(string pageContent)
    {
        Document = new HtmlParser().ParseDocument(pageContent);
    }
}
