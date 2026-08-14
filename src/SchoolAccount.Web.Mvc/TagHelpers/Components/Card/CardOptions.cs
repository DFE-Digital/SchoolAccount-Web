namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

public sealed record CardOptions
{
    public string? Href { get; init; }
    public int HeadingLevel { get; init; } = 3;
    public string Heading { get; init; } = string.Empty;
    public CardImage? Image { get; init; }
    public CardMeta? Meta { get; init; }
    public bool OpenInNewTab { get; init; }
    public string? Classes { get; init; }
}
