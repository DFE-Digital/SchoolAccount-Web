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
public class ErrorSummaryTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Shared/_ErrorSummary.cshtml";

    // The TempData key the controllers use. It must match FeedbackController exactly.
    private const string ErrorSummaryKey = "ErrorSummary";

    // =====================================================================================
    // When it appears
    // =====================================================================================

    [Fact]
    public async Task Nothing_is_shown_when_there_are_no_errors()
    {
        // Arrange - no TempData at all, as on a normal page visit.

        // Act
        using var html = await RenderWithErrorsAsync(errors: null);

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
    public async Task Summary_is_shown_with_the_standard_govuk_title_when_there_are_errors()
    {
        // Arrange
        var errors = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
        };

        // Act
        using var html = await RenderWithErrorsAsync(errors);

        // Assert
        var summary = GetErrorSummary(html).ShouldNotBeNull();
        var title = summary.QuerySelector(".govuk-error-summary__title").ShouldNotBeNull();
        title.TextContent.Trim().ShouldBe("There is a problem");
    }

    // =====================================================================================
    // The list of errors
    // =====================================================================================

    [Fact]
    public async Task Each_error_message_is_listed()
    {
        // Arrange
        var errors = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
            ["Email"] = "Enter an email address",
        };

        // Act
        using var html = await RenderWithErrorsAsync(errors);

        // Assert
        GetErrorLinks(html)
            .Select(link => link.TextContent.Trim())
            .ShouldBe(["Enter your feedback or describe the problem", "Enter an email address"]);
    }

    [Fact]
    public async Task Each_error_links_to_the_field_that_needs_fixing()
    {
        // Arrange
        var errors = new Dictionary<string, string>
        {
            ["FeedbackMessage"] = "Enter your feedback or describe the problem",
        };

        // Act
        using var html = await RenderWithErrorsAsync(errors);

        // Assert
        // "#FeedbackMessage" jumps to the element with id="FeedbackMessage". In the feedback
        // form that's the textarea, so clicking the error puts the user where they need to type.
        GetErrorLinks(html).Single().GetAttribute("href").ShouldBe("#FeedbackMessage");
    }

    [Fact]
    public async Task Error_messages_are_shown_as_text_rather_than_html()
    {
        // Arrange
        var errors = new Dictionary<string, string> { ["Field"] = "<b>Not bold</b>" };

        // Act
        using var html = await RenderWithErrorsAsync(errors);

        // Assert
        var link = GetErrorLinks(html).Single();
        link.TextContent.Trim().ShouldBe("<b>Not bold</b>");
        link.QuerySelector("b").ShouldBeNull();
    }

    // =====================================================================================
    // Accessibility
    // =====================================================================================

    [Fact]
    public async Task Summary_is_wired_up_to_the_govuk_javascript()
    {
        // Arrange
        var errors = new Dictionary<string, string> { ["FeedbackMessage"] = "Enter your feedback" };

        // Act
        using var html = await RenderWithErrorsAsync(errors);

        // Assert
        // The GOV.UK JavaScript finds the summary by this attribute and moves keyboard focus
        // to it when the page loads, so screen reader users hear about the errors straight away.
        GetErrorSummary(html)
            .ShouldNotBeNull()
            .GetAttribute("data-module")
            .ShouldBe("govuk-error-summary");
    }

    // =====================================================================================
    // Helpers
    // =====================================================================================

    /// <summary>
    /// Renders the summary with the given errors in TempData, or with empty TempData if
    /// <paramref name="errors"/> is null.
    /// </summary>
    private Task<IDocument> RenderWithErrorsAsync(Dictionary<string, string>? errors)
    {
        var tempData = new Dictionary<string, object>();

        if (errors is not null)
        {
            tempData[ErrorSummaryKey] = errors;
        }

        // No model, so we pass null. <object?> just tells C# what type that null is.
        return renderer.RenderAsync<object?>(ViewPath, null, tempData);
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
