using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Feedback;

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
        using var html = await RenderNotSentYetAsync();

        var section = html.QuerySelector("section#page-feedback").ShouldNotBeNull();
        section.GetAttribute("aria-label").ShouldBe("Page feedback");
    }

    [Fact]
    public async Task Feedback_javascript_is_included()
    {
        using var html = await RenderNotSentYetAsync();

        // *= because asp-append-version appends "?v=...".
        html.QuerySelector("script[src*='/lib/feedback/feedback.js']").ShouldNotBeNull();
    }

    // State 1: not sent yet

    [Fact]
    public async Task Form_posts_to_the_feedback_controller()
    {
        using var html = await RenderNotSentYetAsync();

        var form = GetForm(html);
        form.GetAttribute("action").ShouldBe("/Feedback/Submit");
        form.GetAttribute("method").ShouldBe("post");
    }

    [Fact]
    public async Task Form_includes_an_anti_forgery_token()
    {
        using var html = await RenderNotSentYetAsync();

        var token = GetForm(html)
            .QuerySelector("input[name='__RequestVerificationToken']")
            .ShouldNotBeNull();
        token.GetAttribute("value").ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Form_remembers_which_page_to_return_to()
    {
        using var html = await renderer.RenderAsync<object?>(
            ViewPath,
            null,
            requestUrl: "/Journey?year=2026"
        );

        var returnUrl = GetForm(html).QuerySelector("input[name='returnUrl']").ShouldNotBeNull();
        returnUrl.GetAttribute("type").ShouldBe("hidden");
        returnUrl.GetAttribute("value").ShouldBe("/Journey?year=2026");
    }

    [Fact]
    public async Task Message_box_is_labelled()
    {
        using var html = await RenderNotSentYetAsync();

        var label = html.QuerySelector("label[for='FeedbackMessage']").ShouldNotBeNull();
        label.TextContent.Trim().ShouldBe("Your feedback");
    }

    [Fact]
    public async Task Message_box_is_described_by_its_hint()
    {
        using var html = await RenderNotSentYetAsync();

        // ShouldBe: with no error, aria-describedby mustn't reference the error element.
        var hint = html.GetElementById(HintId).ShouldNotBeNull();
        hint.TextContent.Trim().ShouldBe("Tell us what you think or describe the problem");
        GetDescribedBy(GetMessageBox(html)).ShouldBe([HintId]);
    }

    [Fact]
    public async Task Message_box_is_posted_as_FeedbackMessage()
    {
        using var html = await RenderNotSentYetAsync();

        GetMessageBox(html).GetAttribute("name").ShouldBe("FeedbackMessage");
    }

    [Fact]
    public async Task No_error_is_shown_before_the_form_is_sent()
    {
        using var html = await RenderNotSentYetAsync();

        html.QuerySelector(".govuk-error-message").ShouldBeNull();
        GetMessageBox(html).ClassList.ShouldNotContain("govuk-textarea--error");
    }

    [Fact]
    public async Task Form_tells_the_javascript_to_start_closed_when_there_are_no_errors()
    {
        using var html = await RenderNotSentYetAsync();

        GetFormContainer(html).GetAttribute("data-has-errors").ShouldBe("false");
    }

    [Fact]
    public async Task Give_feedback_prompt_is_hidden_until_the_javascript_shows_it()
    {
        using var html = await RenderNotSentYetAsync();

        // ClassList rather than a selector because "!" is special in CSS selectors.
        var prompt = html.GetElementById("feedback-initial").ShouldNotBeNull();
        prompt.ClassList.ShouldContain("govuk-!-display-none");
    }

    [Fact]
    public async Task Cancel_button_is_hidden_until_the_javascript_shows_it()
    {
        using var html = await RenderNotSentYetAsync();

        GetCancelButton(html).ClassList.ShouldContain("govuk-!-display-none");
    }

    [Fact]
    public async Task Cancel_button_does_not_submit_the_form()
    {
        using var html = await RenderNotSentYetAsync();

        GetCancelButton(html).GetAttribute("type").ShouldBe("button");
    }

    [Fact]
    public async Task Give_feedback_button_tells_screen_readers_which_form_it_opens()
    {
        using var html = await RenderNotSentYetAsync();

        var button = html.GetElementById("open-feedback").ShouldNotBeNull();
        var controlledId = button.GetAttribute("aria-controls").ShouldNotBeNull();
        html.GetElementById(controlledId).ShouldBe(GetForm(html));
    }

    // State 2: sent, but the message was missing or too long

    [Fact]
    public async Task Error_message_is_shown_above_the_message_box()
    {
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        var error = html.GetElementById(ErrorId).ShouldNotBeNull();
        error.ClassList.ShouldContain("govuk-error-message");
        error.TextContent.Trim().ShouldBe($"Error: {MissingMessageError}");
        error.CompareDocumentPosition(GetMessageBox(html)).ShouldBe(DocumentPositions.Following);
    }

    [Fact]
    public async Task Error_message_is_announced_as_an_error_by_screen_readers()
    {
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        var error = html.GetElementById(ErrorId).ShouldNotBeNull();
        var prefix = error.QuerySelector(".govuk-visually-hidden").ShouldNotBeNull();
        prefix.TextContent.ShouldBe("Error:");
    }

    [Fact]
    public async Task Form_group_and_message_box_are_styled_as_errors()
    {
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        var formGroup = html.QuerySelector(".govuk-form-group").ShouldNotBeNull();
        formGroup.ClassList.ShouldContain("govuk-form-group--error");
        GetMessageBox(html).ClassList.ShouldContain("govuk-textarea--error");
    }

    [Fact]
    public async Task Message_box_is_described_by_the_error_as_well_as_the_hint()
    {
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        GetDescribedBy(GetMessageBox(html)).ShouldBe([HintId, ErrorId]);
    }

    [Fact]
    public async Task Form_tells_the_javascript_to_start_open_when_there_are_errors()
    {
        using var html = await RenderWithMessageErrorAsync(MissingMessageError);

        GetFormContainer(html).GetAttribute("data-has-errors").ShouldBe("true");
    }

    [Fact]
    public async Task Errors_for_other_fields_are_not_shown_on_the_feedback_form()
    {
        var errorForAnotherField = new Dictionary<string, string>
        {
            ["SomeOtherField"] = "Enter something",
        };

        using var html = await RenderWithTempDataAsync(
            new Dictionary<string, object> { [TempDataKeys.ErrorSummary] = errorForAnotherField }
        );

        html.QuerySelector(".govuk-error-message").ShouldBeNull();
        GetFormContainer(html).GetAttribute("data-has-errors").ShouldBe("false");
    }

    // State 3: sent successfully

    [Fact]
    public async Task Thank_you_message_is_shown_after_feedback_is_sent()
    {
        using var html = await RenderAfterSendingAsync();

        var thanks = html.GetElementById("feedback-thanks").ShouldNotBeNull();
        thanks.TextContent.Trim().ShouldBe("Thank you for your feedback");
    }

    [Fact]
    public async Task Form_and_give_feedback_button_are_not_shown_after_feedback_is_sent()
    {
        using var html = await RenderAfterSendingAsync();

        html.QuerySelector("form").ShouldBeNull();
        html.GetElementById("open-feedback").ShouldBeNull();
    }

    // Helpers

    private Task<IDocument> RenderNotSentYetAsync()
    {
        return RenderWithTempDataAsync(new Dictionary<string, object>());
    }

    private Task<IDocument> RenderWithMessageErrorAsync(string errorMessage)
    {
        var errors = new Dictionary<string, string> { ["FeedbackMessage"] = errorMessage };
        return RenderWithTempDataAsync(
            new Dictionary<string, object> { [TempDataKeys.ErrorSummary] = errors }
        );
    }

    private Task<IDocument> RenderAfterSendingAsync()
    {
        return RenderWithTempDataAsync(
            new Dictionary<string, object> { [TempDataKeys.FeedbackSubmitted] = true }
        );
    }

    private Task<IDocument> RenderWithTempDataAsync(IDictionary<string, object> tempData)
    {
        return renderer.RenderAsync<object?>(ViewPath, null, tempData);
    }

    private static IElement GetForm(IDocument html)
    {
        return html.GetElementById("tell-us-form").ShouldNotBeNull();
    }

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

    private static string[] GetDescribedBy(IElement element)
    {
        var describedBy = element.GetAttribute("aria-describedby") ?? string.Empty;
        return describedBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}
