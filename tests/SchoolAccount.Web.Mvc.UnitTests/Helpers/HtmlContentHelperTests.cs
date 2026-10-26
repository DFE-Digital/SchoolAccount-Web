using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Html;
using SchoolAccount.Web.Mvc.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

public class HtmlContentHelperTests
{
    [Fact]
    public async Task Govuk_classes_are_added_to_paragraphs()
    {
        // Arrange
        var content = "<p>Test</p>";
        var requiredClasses = new Dictionary<string, string[]> { ["p"] = ["govuk-paragraph"] };

        // Act
        var corrected = HtmlContentHelper.AddClassesToNodes(content, requiredClasses);

        // Arrange
        var document = Parse(corrected);
        var paragraph = document.QuerySelector(".govuk-paragraph");
        paragraph.ShouldNotBeNull();
        paragraph.TextContent.ShouldBeEquivalentTo("Test");
    }

    [Fact]
    public async Task Govuk_classes_are_added_to_lists_and_their_items()
    {
        // Arrange
        var content = "<ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>";
        var requiredClasses = new Dictionary<string, string[]>
        {
            ["ul"] = ["govuk-unsorted-list"],
            ["li"] = ["govuk-unsorted-list__item"],
        };

        // Act
        var corrected = HtmlContentHelper.AddClassesToNodes(content, requiredClasses);

        // Arrange
        var document = Parse(corrected);
        var list = document.QuerySelector<IHtmlUnorderedListElement>(".govuk-unsorted-list");
        list.ShouldNotBeNull();
        list.Children.ShouldNotBeEmpty();
        list.Children.Count.ShouldBe(3);
        list.Children.ShouldAllBe(x => x.ClassList.Contains("govuk-unsorted-list__item"));
    }

    private static AngleSharp.Dom.IDocument Parse(IHtmlContent html)
    {
        return Parse(html.ToString() ?? string.Empty);
    }

    private static AngleSharp.Dom.IDocument Parse(string html)
    {
        var parser = new HtmlParser();
        return parser.ParseDocument($"<body>{html}</body>");
    }
}
