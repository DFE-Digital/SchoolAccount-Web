using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

[HtmlTargetElement("dfe-card-image")]
public sealed class CardImageTagHelper : TagHelper
{
    public string Src { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = (CardContext)context.Items[typeof(CardContext)]!;
        cardContext.Image = new CardImage(Src, Alt);
        output.SuppressOutput();
    }
}
