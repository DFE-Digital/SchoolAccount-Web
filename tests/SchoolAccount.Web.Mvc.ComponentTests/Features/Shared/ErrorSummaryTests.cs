using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Shared;

public sealed class ErrorSummaryTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Shared/_ErrorSummary.cshtml";

    // When it appears

    [Fact]
    public async Task Nothing_is_shown_when_there_are_no_errors()
    {
        using var html = await RenderWithoutErrorsAsync();

        GetErrorSummary(html).ShouldBeNull();
    }

    [Fact]
    public async Task Nothing_is_shown_when_the_error_list_is_empty()
    {
        var noErrors = new Dictionary<string, string>();

        using var html = await RenderWithErrorsAsync(noErrors);

        GetErrorSummary(html).ShouldBeNull();
    }

    [Fact]
    public async Task Summary_is_shown_when_there_are_errors()
    {
        var missingFeedbackError = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
        };

        using var html = await RenderWithErrorsAsync(missingFeedbackError);

        GetErrorSummary(html).ShouldNotBeNull();
    }

    // The list of errors

    [Fact]
    public async Task Each_error_message_is_listed()
    {
        var twoErrors = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
            ["Email"] = "Enter an email address",
        };

        using var html = await RenderWithErrorsAsync(twoErrors);

        // ignoreOrder: Dictionary enumeration order isn't guaranteed.
        GetErrorLinks(html)
            .Select(link => link.TextContent.Trim())
            .ShouldBe(
                ["Enter your feedback or describe the problem", "Enter an email address"],
                ignoreOrder: true
            );
    }

    [Fact]
    public async Task Each_error_links_to_the_field_that_needs_fixing()
    {
        var missingFeedbackError = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
        };

        using var html = await RenderWithErrorsAsync(missingFeedbackError);

        GetErrorLinks(html)
            .ShouldHaveSingleItem()
            .GetAttribute("href")
            .ShouldBe("#FeedbackMessage");
    }

    [Fact]
    public async Task Error_messages_are_shown_as_text_rather_than_html()
    {
        var errorContainingHtml = new Dictionary<string, string> { ["Field"] = "<b>Not bold</b>" };

        using var html = await RenderWithErrorsAsync(errorContainingHtml);

        var link = GetErrorLinks(html).ShouldHaveSingleItem();
        link.TextContent.Trim().ShouldBe("<b>Not bold</b>");
        link.QuerySelector("b").ShouldBeNull();
    }

    // Helpers

    private Task<IDocument> RenderWithoutErrorsAsync()
    {
        return renderer.RenderAsync<object?>(ViewPath, null, new Dictionary<string, object>());
    }

    private Task<IDocument> RenderWithErrorsAsync(Dictionary<string, string> errors)
    {
        return renderer.RenderAsync<object?>(
            ViewPath,
            null,
            new Dictionary<string, object> { [TempDataKeys.ErrorSummary] = errors }
        );
    }

    private static IElement? GetErrorSummary(IDocument html)
    {
        return html.QuerySelector(".govuk-error-summary");
    }

    private static IHtmlCollection<IElement> GetErrorLinks(IDocument html)
    {
        return html.QuerySelectorAll(".govuk-error-summary__list a");
    }
}
