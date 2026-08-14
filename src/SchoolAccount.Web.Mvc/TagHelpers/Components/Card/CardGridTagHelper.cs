using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

[HtmlTargetElement("dfe-card-grid")]
public sealed class CardGridTagHelper : TagHelper
{
    public string? Class { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var classes = "dfe-grid-container";
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        output.Attributes.SetAttribute("class", classes);
        output.Content.SetHtmlContent(content);
    }
}
