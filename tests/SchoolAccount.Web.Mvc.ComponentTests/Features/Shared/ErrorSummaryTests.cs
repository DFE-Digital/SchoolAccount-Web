using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Shared;

/// <summary>
/// Tests for the GOV.UK error summary shown at the top of every page (_ErrorSummary.cshtml).
/// </summary>
/// <remarks>
/// <para>
/// When a form is posted with mistakes, e.g. the feedback form with no message, the
/// controller redirects back to the page the user was on. It passes the errors along in
/// <c>TempData["ErrorSummary"]</c>, a dictionary of field ID → error message. This component
/// lists those errors in a red box, each linking to the field that needs fixing.
/// </para>
/// <para>
/// It has no model. Everything it shows comes from TempData, so these tests pass errors in
/// through the renderer's <c>tempData</c> parameter.
/// </para>
/// </remarks>
public sealed class ErrorSummaryTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Shared/_ErrorSummary.cshtml";

    // When it appears

    [Fact]
    public async Task Nothing_is_shown_when_there_are_no_errors()
    {
        // Act
        using var html = await RenderWithoutErrorsAsync();

        // Assert
        GetErrorSummary(html).ShouldBeNull();
    }

    [Fact]
    public async Task Nothing_is_shown_when_the_error_list_is_empty()
    {
        // Arrange
        var noErrors = new Dictionary<string, string>();

        // Act
        using var html = await RenderWithErrorsAsync(noErrors);

        // Assert
        GetErrorSummary(html).ShouldBeNull();
    }

    [Fact]
    public async Task Summary_is_shown_when_there_are_errors()
    {
        // Arrange
        var missingFeedbackError = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
        };

        // Act
        using var html = await RenderWithErrorsAsync(missingFeedbackError);

        // Assert
        GetErrorSummary(html).ShouldNotBeNull();
    }

    // The list of errors

    [Fact]
    public async Task Each_error_message_is_listed()
    {
        // Arrange
        var twoErrors = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
            ["Email"] = "Enter an email address",
        };

        // Act
        using var html = await RenderWithErrorsAsync(twoErrors);

        // Assert
        // ignoreOrder because the view lists errors in Dictionary order, which .NET doesn't
        // guarantee.
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
        // Arrange
        var missingFeedbackError = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
        };

        // Act
        using var html = await RenderWithErrorsAsync(missingFeedbackError);

        // Assert
        // "#FeedbackMessage" jumps to the element with id="FeedbackMessage". In the feedback
        // form that's the textarea, so clicking the error puts the user where they need to type.
        GetErrorLinks(html)
            .ShouldHaveSingleItem()
            .GetAttribute("href")
            .ShouldBe("#FeedbackMessage");
    }

    [Fact]
    public async Task Error_messages_are_shown_as_text_rather_than_html()
    {
        // Arrange
        var errorContainingHtml = new Dictionary<string, string> { ["Field"] = "<b>Not bold</b>" };

        // Act
        using var html = await RenderWithErrorsAsync(errorContainingHtml);

        // Assert
        var link = GetErrorLinks(html).ShouldHaveSingleItem();
        link.TextContent.Trim().ShouldBe("<b>Not bold</b>");
        link.QuerySelector("b").ShouldBeNull();
    }

    // Helpers

    /// <summary>
    /// Renders the summary with empty TempData, as on a normal page visit.
    /// </summary>
    private Task<IDocument> RenderWithoutErrorsAsync()
    {
        return renderer.RenderAsync<object?>(ViewPath, null, new Dictionary<string, object>());
    }

    /// <summary>
    /// Renders the summary with the given errors in TempData.
    /// </summary>
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
