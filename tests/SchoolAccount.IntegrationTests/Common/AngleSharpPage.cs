using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace SchoolAccount.IntegrationTests.Common;

public abstract partial class AngleSharpPage
{
    protected IDocument Document
    {
        get =>
            field
            ?? throw new InvalidOperationException(
                $"{GetType().Name} has no content. Create it with {nameof(FromResponseAsync)}."
            );
        private set;
    }

    public static async Task<T> FromResponseAsync<T>(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default
    )
        where T : AngleSharpPage, new()
    {
        await using var content = await responseMessage.Content.ReadAsStreamAsync(
            cancellationToken
        );
        var page = new T();
        await page.InitialiseAsync(content, cancellationToken);
        return page;
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

    protected static string Normalise(string text)
    {
        return Whitespace().Replace(text, " ").Trim();
    }

    protected static string? GetNormalisedText(IElement? element)
    {
        return element is null ? null : Normalise(element.TextContent);
    }

    protected string? GetTextBySelector(string selector)
    {
        return GetNormalisedText(Document.QuerySelector(selector));
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    private async Task InitialiseAsync(
        Stream pageContent,
        CancellationToken cancellationToken = default
    )
    {
        Document = await new HtmlParser().ParseDocumentAsync(pageContent, cancellationToken);
    }
}
