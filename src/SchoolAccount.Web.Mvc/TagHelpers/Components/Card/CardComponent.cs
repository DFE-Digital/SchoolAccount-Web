using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

public sealed class CardComponent
{
    private readonly CardOptions _options;

    public CardComponent(CardOptions options) => _options = options;

    public void ApplyTo(TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", BuildClasses());

        if (_options.Image is not null)
        {
            output.Content.AppendHtml(BuildImage(_options.Image));
        }

        var container = new TagBuilder("div");
        container.AddCssClass("dfe-card-container");
        container.InnerHtml.AppendHtml(BuildHeading());

        if (_options.Meta is not null)
        {
            container.InnerHtml.AppendHtml(BuildMeta(_options.Meta));
        }

        output.Content.AppendHtml(container);
    }

    private string BuildClasses() =>
        string.IsNullOrWhiteSpace(_options.Classes) ? "dfe-card" : $"dfe-card {_options.Classes}";

    private static TagBuilder BuildImage(CardImage image)
    {
        var tag = new TagBuilder("img");
        tag.Attributes["src"] = image.Src;
        tag.Attributes["alt"] = image.Alt;
        return tag;
    }

    private TagBuilder BuildHeading()
    {
        var heading = new TagBuilder($"h{_options.HeadingLevel}");
        heading.AddCssClass("govuk-heading-m");

        if (!string.IsNullOrWhiteSpace(_options.Href))
        {
            var link = new TagBuilder("a");
            link.AddCssClass("govuk-link");
            link.AddCssClass("dfe-card-link--header");
            link.Attributes["href"] = _options.Href;

            if (_options.OpenInNewTab)
            {
                link.Attributes["target"] = "_blank";
                link.Attributes["rel"] = "noopener noreferrer";
            }

            link.InnerHtml.Append(_options.Heading);
            heading.InnerHtml.AppendHtml(link);
        }
        else
        {
            heading.InnerHtml.Append(_options.Heading);
        }

        return heading;
    }

    private static TagBuilder BuildMeta(CardMeta meta)
    {
        var tag = new TagBuilder("p");
        tag.AddCssClass("govuk-body-s");
        tag.InnerHtml.AppendHtml(meta.Content);
        return tag;
    }
}
