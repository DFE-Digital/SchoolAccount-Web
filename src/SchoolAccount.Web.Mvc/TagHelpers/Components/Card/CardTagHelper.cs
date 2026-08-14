using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

[HtmlTargetElement("dfe-card")]
public sealed class CardTagHelper(ICardGenerator generator) : TagHelper
{
    public string? Href { get; set; }
    public int HeadingLevel { get; set; } = 3;
    public bool OpenInNewTab { get; set; }
    public string? Class { get; set; }

    public override void Init(TagHelperContext context)
    {
        context.Items[typeof(CardContext)] = new CardContext();
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (HeadingLevel is < 2 or > 4)
        {
            throw new InvalidDataException(
                $"{nameof(HeadingLevel)}: Card heading level must be between 2 and 4."
            );
        }

        var childContent = await output.GetChildContentAsync();
        var cardContext = (CardContext)context.Items[typeof(CardContext)]!;

        var options = new CardOptions
        {
            Href = Href,
            HeadingLevel = HeadingLevel,
            Heading = childContent.GetContent().Trim(),
            Image = cardContext.Image,
            Meta = cardContext.Meta,
            OpenInNewTab = OpenInNewTab,
            Classes = Class,
        };

        generator.Generate(options).ApplyTo(output);
    }
}
