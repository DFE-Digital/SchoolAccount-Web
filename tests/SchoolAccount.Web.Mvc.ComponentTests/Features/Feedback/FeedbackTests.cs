using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Feedback;

/// <summary>
/// Tests for the "Give feedback on this page" form in the footer (_Feedback.cshtml).
/// </summary>
/// <remarks>
/// <para>
/// The form posts to FeedbackController.Submit, which then redirects back to the page the
/// user was on. The controller tells this component what happened using TempData. That gives
/// three states, and the tests are grouped by them:
/// </para>
/// <list type="number">
///   <item><b>Not sent yet</b>: TempData is empty. The form is shown.</item>
///   <item><b>Sent, but invalid</b>: <c>TempData["ErrorSummary"]</c> contains an error for
///   the message field. The form is shown with the error.</item>
///   <item><b>Sent successfully</b>: <c>TempData["FeedbackSubmitted"]</c> is set. A thank
///   you message replaces the form.</item>
/// </list>
/// <para>
/// The page's JavaScript (wwwroot/lib/feedback/feedback.js) starts with only a "Give feedback"
/// button showing and opens the form when it's clicked. Without JavaScript the form is simply
/// always visible. Several tests check the IDs and attributes that JavaScript relies on.
/// </para>
/// </remarks>
public sealed class FeedbackTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Feedback/_Feedback.cshtml";

    private const string HintId = "what-do-you-want-to-tell-us-hint";
    private const string ErrorId = "FeedbackMessage-error";

    private const string MissingMessageError = "Enter your feedback or describe the problem";

    // Always shown

    [Fact]
    public async Task Feedback_area_is_a_labelled_landmark_for_screen_readers()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // A <section> with an aria-label appears in screen readers' list of page regions,
        // so users can jump straight to it.
        var section = html.QuerySelector("section#page-feedback").ShouldNotBeNull();
        section.GetAttribute("aria-label").ShouldBe("Page feedback");
    }

    [Fact]
    public async Task Feedback_javascript_is_included()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // Matches with "*=" (contains) because asp-append-version adds "?v=..." to the URL.
        html.QuerySelector("script[src*='/lib/feedback/feedback.js']").ShouldNotBeNull();
    }

    // State 1: not sent yet

    [Fact]
    public async Task Form_posts_to_the_feedback_controller()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        var form = GetForm(html);
        form.GetAttribute("action").ShouldBe("/Feedback/Submit");
        form.GetAttribute("method").ShouldBe("post");
    }

    [Fact]
    public async Task Form_includes_an_anti_forgery_token()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // FeedbackController.Submit has [ValidateAntiForgeryToken], so it rejects any post
        // without this hidden field. The token proves the post came from our own page, not
        // from another site tricking the user's browser into posting (CSRF).
        var token = GetForm(html)
            .QuerySelector("input[name='__RequestVerificationToken']")
            .ShouldNotBeNull();
        token.GetAttribute("value").ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Form_remembers_which_page_to_return_to()
    {
        // Act
        using var html = await renderer.RenderAsync<object?>(
            ViewPath,
            null,
            requestUrl: "/Journey?year=2026"
        );

        // Assert
        // After handling the post, the controller redirects the user back to this URL.
        var returnUrl = GetForm(html).QuerySelector("input[name='returnUrl']").ShouldNotBeNull();
        returnUrl.GetAttribute("type").ShouldBe("hidden");
        returnUrl.GetAttribute("value").ShouldBe("/Journey?year=2026");
    }

    [Fact]
    public async Task Message_box_is_labelled()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // label for="FeedbackMessage" links the label to the box, so clicking the label
        // focuses the box and screen readers read the label out.
        var label = html.QuerySelector("label[for='FeedbackMessage']").ShouldNotBeNull();
        label.TextContent.Trim().ShouldBe("Your feedback");
    }

    [Fact]
    public async Task Message_box_is_described_by_its_hint()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // ShouldBe rather than ShouldContain: with no error, aria-describedby must not point
        // at an error element that doesn't exist.
        var hint = html.GetElementById(HintId).ShouldNotBeNull();
        hint.TextContent.Trim().ShouldBe("Tell us what you think or describe the problem");
        GetDescribedBy(GetMessageBox(html)).ShouldBe([HintId]);
    }

    [Fact]
    public async Task Message_box_is_posted_as_FeedbackMessage()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // MVC copies posted fields into FeedbackForm by matching names, so the textarea's
        // name must match the FeedbackForm.FeedbackMessage property.
        GetMessageBox(html).GetAttribute("name").ShouldBe("FeedbackMessage");
    }

    [Fact]
    public async Task No_error_is_shown_before_the_form_is_sent()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        html.QuerySelector(".govuk-error-message").ShouldBeNull();
        GetMessageBox(html).ClassList.ShouldNotContain("govuk-textarea--error");
    }

    [Fact]
    public async Task Form_tells_the_javascript_to_start_closed_when_there_are_no_errors()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        GetFormContainer(html).GetAttribute("data-has-errors").ShouldBe("false");
    }

    [Fact]
    public async Task Give_feedback_prompt_is_hidden_until_the_javascript_shows_it()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // The prompt is the heading and "Give feedback" button. Without JavaScript the button
        // would do nothing, so the prompt starts hidden. ClassList is used instead of a CSS
        // selector because "!" has a special meaning in selectors.
        var prompt = html.GetElementById("feedback-initial").ShouldNotBeNull();
        prompt.ClassList.ShouldContain("govuk-!-display-none");
    }

    [Fact]
    public async Task Cancel_button_is_hidden_until_the_javascript_shows_it()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        GetCancelButton(html).ClassList.ShouldContain("govuk-!-display-none");
    }

    [Fact]
    public async Task Cancel_button_does_not_submit_the_form()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // A <button> inside a form submits it unless its type is "button".
        GetCancelButton(html).GetAttribute("type").ShouldBe("button");
    }

    [Fact]
    public async Task Give_feedback_button_tells_screen_readers_which_form_it_opens()
    {
        // Act
        using var html = await RenderNotSentYetAsync();

        // Assert
        // aria-controls must hold the ID of the element the button opens, so check that
        // element really exists.
        var button = html.GetElementById("open-feedback").ShouldNotBeNull();
        var controlledId = button.GetAttribute("aria-controls").ShouldNotBeNull();
        html.GetElementById(controlledId).ShouldBe(GetForm(html));
    }

    // State 2: sent, but the message was missing or too long

    [Fact]
    public async Task Error_message_is_shown_above_the_message_box()
    {
        // Act
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        // Assert
        var error = html.GetElementById(ErrorId).ShouldNotBeNull();
        error.ClassList.ShouldContain("govuk-error-message");
        error.TextContent.Trim().ShouldBe($"Error: {MissingMessageError}");
        error.CompareDocumentPosition(GetMessageBox(html)).ShouldBe(DocumentPositions.Following);
    }

    [Fact]
    public async Task Error_message_is_announced_as_an_error_by_screen_readers()
    {
        // Act
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        // Assert
        // Visually hidden "Error:" makes screen readers say "Error: Enter your feedback...",
        // so users know it isn't just hint text.
        var error = html.GetElementById(ErrorId).ShouldNotBeNull();
        var prefix = error.QuerySelector(".govuk-visually-hidden").ShouldNotBeNull();
        prefix.TextContent.ShouldBe("Error:");
    }

    [Fact]
    public async Task Form_group_and_message_box_are_styled_as_errors()
    {
        // Act
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        // Assert
        // These classes add the red border and left-hand bar from the GOV.UK design system.
        var formGroup = html.QuerySelector(".govuk-form-group").ShouldNotBeNull();
        formGroup.ClassList.ShouldContain("govuk-form-group--error");
        GetMessageBox(html).ClassList.ShouldContain("govuk-textarea--error");
    }

    [Fact]
    public async Task Message_box_is_described_by_the_error_as_well_as_the_hint()
    {
        // Act
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        // Assert
        // aria-describedby can hold several IDs separated by spaces. Screen readers read each.
        GetDescribedBy(GetMessageBox(html)).ShouldBe([HintId, ErrorId]);
    }

    [Fact]
    public async Task Form_tells_the_javascript_to_start_open_when_there_are_errors()
    {
        // Act
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        // Assert
        // Starting open means the error isn't hidden behind the "Give feedback" button.
        GetFormContainer(html).GetAttribute("data-has-errors").ShouldBe("true");
    }

    [Fact]
    public async Task Errors_for_other_forms_are_not_shown_on_the_feedback_form()
    {
        // Arrange
        // The error summary is shared by every form on the site. An error for some other
        // field shouldn't make the feedback form look broken.
        var errorForAnotherField = new Dictionary<string, string>
        {
            ["SomeOtherField"] = "Enter something",
        };

        // Act
        using var html = await RenderWithTempDataAsync(
            new Dictionary<string, object> { [TempDataKeys.ErrorSummary] = errorForAnotherField }
        );

        // Assert
        html.QuerySelector(".govuk-error-message").ShouldBeNull();
        GetFormContainer(html).GetAttribute("data-has-errors").ShouldBe("false");
    }

    // State 3: sent successfully

    [Fact]
    public async Task Thank_you_message_is_shown_after_feedback_is_sent()
    {
        // Act
        using var html = await RenderAfterSendingAsync();

        // Assert
        // The controller redirects to "...#feedback-thanks", so the browser scrolls straight
        // to the element with that ID.
        var thanks = html.GetElementById("feedback-thanks").ShouldNotBeNull();
        thanks.TextContent.Trim().ShouldBe("Thank you for your feedback");
    }

    [Fact]
    public async Task Form_is_not_shown_after_feedback_is_sent()
    {
        // Act
        using var html = await RenderAfterSendingAsync();

        // Assert
        html.QuerySelector("form").ShouldBeNull();
        html.GetElementById("open-feedback").ShouldBeNull();
    }

    // Helpers - one per state, so each test reads as "render in state X, then check Y"

    /// <summary>State 1: a normal page visit. TempData is empty.</summary>
    private Task<IDocument> RenderNotSentYetAsync()
    {
        return RenderWithTempDataAsync(new Dictionary<string, object>());
    }

    /// <summary>State 2: the controller sent back an error for the message box.</summary>
    private Task<IDocument> RenderWithMessageErrorAsync(string errorMessage)
    {
        var errors = new Dictionary<string, string> { ["FeedbackMessage"] = errorMessage };
        return RenderWithTempDataAsync(
            new Dictionary<string, object> { [TempDataKeys.ErrorSummary] = errors }
        );
    }

    /// <summary>State 3: the controller saved the feedback successfully.</summary>
    private Task<IDocument> RenderAfterSendingAsync()
    {
        return RenderWithTempDataAsync(
            new Dictionary<string, object> { [TempDataKeys.FeedbackSubmitted] = true }
        );
    }

    /// <summary>
    /// Renders the component, which has no model and is driven entirely by TempData.
    /// </summary>
    private Task<IDocument> RenderWithTempDataAsync(IDictionary<string, object> tempData)
    {
        return renderer.RenderAsync<object?>(ViewPath, null, tempData);
    }

    private static IElement GetForm(IDocument html)
    {
        return html.GetElementById("tell-us-form").ShouldNotBeNull();
    }

    /// <summary>
    /// The &lt;div&gt; around the form, which the JavaScript shows and hides.
    /// </summary>
    private static IElement GetFormContainer(IDocument html)
    {
        return html.GetElementById("feedback-form").ShouldNotBeNull();
    }

    private static IElement GetMessageBox(IDocument html)
    {
        return html.QuerySelector("textarea#FeedbackMessage").ShouldNotBeNull();
    }

    private static IElement GetCancelButton(IDocument html)
    {
        return html.GetElementById("cancel-feedback").ShouldNotBeNull();
    }

    /// <summary>
    /// Splits aria-describedby into its separate IDs, e.g. "hint error" → ["hint", "error"].
    /// </summary>
    private static string[] GetDescribedBy(IElement element)
    {
        var describedBy = element.GetAttribute("aria-describedby") ?? string.Empty;
        return describedBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}
