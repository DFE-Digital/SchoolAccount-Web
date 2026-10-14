using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// A page returned by the app, parsed so tests can read it. Holds what every page has: the
/// title, the main heading, the body text and the shared layout. Anything specific to one page
/// belongs on that page's own subclass.
/// </summary>
public abstract class AngleSharpPage
{
    protected IDocument Document { get; private set; } = null!;

    public static async Task<T> FromResponseAsync<T>(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default
    )
        where T : AngleSharpPage, new()
    {
        var html = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var document = await new HtmlParser().ParseDocumentAsync(html, cancellationToken);

        return new T { Document = document };
    }

    public string? Title => TextOf("title");

    public string? Heading => TextOf("h1");

    public string? BodyText => TextOf("body");

    /// <summary>
    /// The text of the first element matching <paramref name="selector"/> that contains
    /// <paramref name="text"/>, or null if there isn't one.
    /// </summary>
    public string? FindText(string selector, string text)
    {
        return Document
            .QuerySelectorAll(selector)
            .FirstOrDefault(element => element.TextContent.Contains(text, StringComparison.Ordinal))
            ?.TextContent;
    }

    // The shared layout: header and footer.

    public string? SignOutButtonText => TextOf("header button");

    public string? OrganisationName => TextOf(".header-navigation__school");

    public IElement? Footer => Document.QuerySelector(".govuk-footer");

    public IElement? FooterLink(string href)
    {
        return Document.QuerySelector($".govuk-footer a[href='{href}']");
    }

    protected string? TextOf(string selector)
    {
        return Document.QuerySelector(selector)?.TextContent;
    }
}
