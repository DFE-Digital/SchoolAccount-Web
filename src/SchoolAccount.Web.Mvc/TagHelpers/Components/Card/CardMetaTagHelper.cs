using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

[HtmlTargetElement("dfe-card-meta")]
public sealed class CardMetaTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = (CardContext)context.Items[typeof(CardContext)]!;
        var content = await output.GetChildContentAsync();

        cardContext.Meta = new CardMeta(content);
        output.SuppressOutput();
    }
}
