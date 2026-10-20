using AngleSharp.Dom;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Shared;

/// <summary>
/// Tests for the step-by-step navigation component (StepByStep.cshtml).
/// </summary>
/// <remarks>
/// <para>
/// The component shows a numbered list of steps. Each step has a title, an optional status
/// tag (e.g. "Submitted") and a panel of HTML content that expands when clicked.
/// </para>
/// <para>
/// Many of these tests check for CSS classes and attributes whose names start with "js-" or
/// "data-". The step-by-step JavaScript (wwwroot/lib/step-by-step/step-by-step.navigation.js)
/// finds elements using those names, so if one is renamed or removed the expand/collapse
/// behaviour silently stops working. These tests catch that.
/// </para>
/// </remarks>
public class StepByStepTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Shared/StepByStep/StepByStep.cshtml";

    // A fixed ID (instead of Guid.NewGuid()) so tests can predict the element IDs it produces.
    private static readonly Guid StepByStepId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    // =====================================================================================
    // Steps and their order
    // =====================================================================================

    [Fact]
    public async Task Steps_are_shown_in_order_with_their_titles()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"), AStep("Submit your return"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // QuerySelectorAll finds every element matching a CSS selector, as in browser dev tools.
        // ".js-step-title" means "any element with the class js-step-title".
        // Shouldly's ShouldBe on a list checks the items AND their order.
        html.QuerySelectorAll(".js-step-title")
            .Select(title => title.TextContent)
            .ShouldBe(["Prepare your data", "Submit your return"]);
    }

    [Fact]
    public async Task Each_step_is_numbered_starting_from_one()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"), AStep("Submit your return"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The JavaScript reads "data-position" to know which step was clicked.
        html.QuerySelectorAll(".js-toggle-panel")
            .Select(header => header.GetAttribute("data-position"))
            .ShouldBe(["1", "2"]);
    }

    [Fact]
    public async Task No_steps_are_shown_when_there_are_none()
    {
        // Arrange
        var model = AStepByStepWith();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelectorAll(".js-step").ShouldBeEmpty();
    }

    // =====================================================================================
    // Status tags
    // =====================================================================================

    [Fact]
    public async Task Step_with_a_status_shows_a_tag_with_its_label_and_colour()
    {
        // Arrange
        var submitted = new StepByStepStatus { Label = "Submitted", Colour = "govuk-tag--green" };

        // "with" copies a record and changes only the properties listed, so this is
        // "a normal step, but with this status".
        var model = AStepByStepWith(AStep("Submit your return") with { Status = submitted });

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // ShouldNotBeNull() fails the test with a clear message if nothing matched, and
        // otherwise hands back the element so we can keep checking it.
        var tag = html.QuerySelector(".govuk-tag").ShouldNotBeNull();
        tag.TextContent.Trim().ShouldBe("Submitted");
        tag.ClassList.ShouldContain("govuk-tag--green");
    }

    [Fact]
    public async Task Step_status_without_a_colour_is_shown_as_a_grey_tag()
    {
        // Arrange
        var notStarted = new StepByStepStatus { Label = "Not started", Colour = null };
        var model = AStepByStepWith(AStep("Submit your return") with { Status = notStarted });

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var tag = html.QuerySelector(".govuk-tag").ShouldNotBeNull();
        tag.ClassList.ShouldContain("govuk-tag--grey");
    }

    [Fact]
    public async Task Step_without_a_status_shows_no_tag()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data") with { Status = null });

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelector(".govuk-tag").ShouldBeNull();
    }

    // =====================================================================================
    // Step content
    // =====================================================================================

    [Fact]
    public async Task Step_body_is_rendered_as_html_with_govuk_styles_applied()
    {
        // Arrange
        // Step bodies arrive from the API as plain HTML with no GOV.UK classes. The view model
        // adds them, so paragraphs and links look right.
        var model = AStepByStepWith(
            AStep("Prepare your data") with
            {
                Body = "<p>Read the <a href=\"/guide\">census guide</a>.</p>",
            }
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // "a[href='/guide']" means "an <a> element whose href attribute is /guide".
        var panel = html.QuerySelector(".js-panel").ShouldNotBeNull();
        var paragraph = panel.QuerySelector("p").ShouldNotBeNull();
        var link = panel.QuerySelector("a[href='/guide']").ShouldNotBeNull();
        paragraph.ClassList.ShouldContain("app-step-nav__paragraph");
        link.ClassList.ShouldContain("govuk-link");
    }

    [Fact]
    public async Task Lists_in_the_step_body_use_the_govuk_bullet_list_style()
    {
        // Arrange
        var model = AStepByStepWith(
            AStep("Prepare your data") with
            {
                Body = "<ul><li>Pupil records</li><li>Staff records</li></ul>",
            }
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // ".js-panel ul" means "a <ul> somewhere inside an element with class js-panel".
        var list = html.QuerySelector(".js-panel ul").ShouldNotBeNull();
        list.ClassList.ShouldContain("govuk-list");
        list.ClassList.ShouldContain("govuk-list--bullet");
    }

    [Fact]
    public async Task Each_step_body_is_shown_in_its_own_panel()
    {
        // Arrange
        var model = AStepByStepWith(
            AStep("Prepare your data") with
            {
                Body = "<p>Gather your records</p>",
            },
            AStep("Submit your return") with
            {
                Body = "<p>Upload your file</p>",
            }
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelectorAll(".js-panel")
            .Select(panel => panel.TextContent.Trim())
            .ShouldBe(["Gather your records", "Upload your file"]);
    }

    [Fact]
    public async Task Step_title_is_shown_as_text_rather_than_html()
    {
        // Arrange
        // Razor HTML-encodes values by default. It turns "<" into "&lt;" so text can't inject
        // tags or scripts into the page. This test makes sure that protection stays in place
        // for titles.
        var model = AStepByStepWith(AStep("<b>Prepare</b> your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The title should show the literal text, and no real <b> element should exist.
        var title = html.QuerySelector(".js-step-title").ShouldNotBeNull();
        title.TextContent.ShouldBe("<b>Prepare</b> your data");
        title.QuerySelector("b").ShouldBeNull();
    }

    // =====================================================================================
    // Opening steps and remembering which were open
    // =====================================================================================

    [Fact]
    public async Task Step_marked_as_open_is_flagged_to_be_shown_on_load()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data") with { IsOpen = true });

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The JavaScript expands any step that has a "data-show" attribute when the page loads.
        GetFirstStep(html).HasAttribute("data-show").ShouldBeTrue();
    }

    [Fact]
    public async Task Step_not_marked_as_open_is_collapsed_on_load()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data") with { IsOpen = false });

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetFirstStep(html).HasAttribute("data-show").ShouldBeFalse();
    }

    [Fact]
    public async Task Opened_steps_are_remembered_when_requested()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data")).RememberSteps();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetWrapper(html).GetAttribute("data-remember-state").ShouldBe("true");
    }

    [Fact]
    public async Task Opened_steps_are_not_remembered_by_default()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetWrapper(html).HasAttribute("data-remember-state").ShouldBeFalse();
    }

    // =====================================================================================
    // Accessibility
    // =====================================================================================

    [Fact]
    public async Task Screen_readers_announce_the_step_number_before_the_title()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // Sighted users see "1" in a circle. Screen reader users hear "Step 1", because
        // "govuk-visually-hidden" text is read aloud but not displayed.
        var stepNumber = html.QuerySelector(".app-step-nav__circle-background").ShouldNotBeNull();
        var hiddenText = stepNumber.QuerySelector(".govuk-visually-hidden").ShouldNotBeNull();
        hiddenText.TextContent.ShouldBe("Step");
        stepNumber.TextContent.ShouldContain("1");
    }

    // =====================================================================================
    // Hooks the JavaScript relies on
    // =====================================================================================

    [Fact]
    public async Task Element_ids_are_derived_from_the_step_by_step_identifier()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // Unique IDs let two step-by-steps live on the same page without clashing.
        html.GetElementById($"sbs-{StepByStepId}-wrapper").ShouldNotBeNull();
        html.GetElementById($"sbs-{StepByStepId}-step-1").ShouldNotBeNull();
        html.GetElementById($"sbs-{StepByStepId}-panel-1").ShouldNotBeNull();
    }

    [Fact]
    public async Task Step_by_step_is_hidden_until_its_javascript_has_loaded()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // "js-hidden" stops a flash of unstyled content. The JavaScript removes it once ready.
        GetWrapper(html).ClassList.ShouldContain("js-hidden");
    }

    [Fact]
    public async Task Javascript_is_initialised_against_this_step_by_step()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The component ends with an inline <script> that starts the JavaScript on this
        // component's wrapper element. [^1] means "the last item".
        var inlineScript = html.QuerySelectorAll("script")[^1].TextContent;
        inlineScript.ShouldContain($"document.querySelector('#sbs-{StepByStepId}-wrapper')");
    }

    [Fact]
    public async Task Show_and_hide_button_text_is_provided_for_the_javascript()
    {
        // Arrange
        var model = AStepByStepWith(AStep("Prepare your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The JavaScript creates the "Show all steps" button using this text.
        var wrapper = GetWrapper(html);
        wrapper.GetAttribute("data-show-all-text").ShouldBe("Show all steps");
        wrapper.GetAttribute("data-hide-all-text").ShouldBe("Hide all steps");
    }

    // =====================================================================================
    // Helpers - these keep the tests short so each one shows only what makes it different
    // =====================================================================================

    /// <summary>
    /// Builds the component's view model containing the given steps, in order.
    /// </summary>
    private static StepByStepViewModelCollection AStepByStepWith(params StepByStep[] steps)
    {
        return StepByStepViewModelCollection.Create(StepByStepId).AddSteps(steps);
    }

    /// <summary>
    /// A step as the API would send it: a title, simple body, no status and not open.
    /// Use "with { ... }" to change anything a test cares about.
    /// </summary>
    private static StepByStep AStep(string title)
    {
        return new StepByStep { Title = title, Body = "<p>Step content</p>" };
    }

    /// <summary>
    /// The outer element wrapping the whole component.
    /// </summary>
    private static IElement GetWrapper(IDocument html)
    {
        // "[data-module='appstepnav']" means "an element whose data-module attribute is appstepnav".
        return html.QuerySelector("[data-module='appstepnav']").ShouldNotBeNull();
    }

    /// <summary>
    /// The &lt;li&gt; element for the first step.
    /// </summary>
    private static IElement GetFirstStep(IDocument html)
    {
        return html.QuerySelector(".js-step").ShouldNotBeNull();
    }
}
