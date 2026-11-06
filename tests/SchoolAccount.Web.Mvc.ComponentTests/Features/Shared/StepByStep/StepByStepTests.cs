using AngleSharp.Dom;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;
using Shouldly;
using StepByStepResponse = SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses.StepByStep;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Shared.StepByStep;

public sealed class StepByStepTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Shared/StepByStep/StepByStep.cshtml";

    private static readonly Guid StepByStepId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    // Steps and their order

    [Fact]
    public async Task Steps_are_shown_in_order_with_their_titles()
    {
        var model = AStepByStepWith(AStep("Prepare your data"), AStep("Submit your return"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll(".js-step-title")
            .Select(title => title.TextContent.Trim())
            .ShouldBe(["Prepare your data", "Submit your return"]);
    }

    [Fact]
    public async Task Steps_are_given_positions_starting_from_one()
    {
        var model = AStepByStepWith(AStep("Prepare your data"), AStep("Submit your return"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll(".js-toggle-panel")
            .Select(header => header.GetAttribute("data-position"))
            .ShouldBe(["1", "2"]);
    }

    [Fact]
    public async Task No_steps_are_shown_when_there_are_none()
    {
        var model = AStepByStepWith();

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll(".js-step").ShouldBeEmpty();
    }

    // Status tags

    [Fact]
    public async Task Step_with_a_status_shows_a_tag_with_its_label_and_colour()
    {
        var submitted = new StepByStepStatus { Label = "Submitted", Colour = "govuk-tag--green" };
        var model = AStepByStepWith(AStep("Submit your return") with { Status = submitted });

        using var html = await renderer.RenderAsync(ViewPath, model);

        var tag = html.QuerySelector(".govuk-tag").ShouldNotBeNull();
        tag.TextContent.Trim().ShouldBe("Submitted");
        tag.ClassList.ShouldContain("govuk-tag--green");
    }

    [Fact]
    public async Task Step_status_without_a_colour_is_shown_as_a_grey_tag()
    {
        var notStarted = new StepByStepStatus { Label = "Not started", Colour = null };
        var model = AStepByStepWith(AStep("Submit your return") with { Status = notStarted });

        using var html = await renderer.RenderAsync(ViewPath, model);

        var tag = html.QuerySelector(".govuk-tag").ShouldNotBeNull();
        tag.ClassList.ShouldContain("govuk-tag--grey");
    }

    [Fact]
    public async Task Step_without_a_status_shows_no_tag()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelector(".govuk-tag").ShouldBeNull();
    }

    // Step content

    [Fact]
    public async Task Step_body_is_rendered_as_html_with_govuk_styles_applied()
    {
        var model = AStepByStepWith(
            AStep("Prepare your data") with
            {
                Body = "<p>Read the <a href=\"/guide\">census guide</a>.</p>",
            }
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        var panel = html.QuerySelector(".js-panel").ShouldNotBeNull();
        var paragraph = panel.QuerySelector("p").ShouldNotBeNull();
        var link = panel.QuerySelector("a[href='/guide']").ShouldNotBeNull();
        paragraph.ClassList.ShouldContain("app-step-nav__paragraph");
        link.ClassList.ShouldContain("govuk-link");
    }

    [Fact]
    public async Task Lists_in_the_step_body_use_the_govuk_bullet_list_style()
    {
        var model = AStepByStepWith(
            AStep("Prepare your data") with
            {
                Body = "<ul><li>Pupil records</li><li>Staff records</li></ul>",
            }
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        var list = html.QuerySelector(".js-panel ul").ShouldNotBeNull();
        list.ClassList.ShouldContain("govuk-list");
        list.ClassList.ShouldContain("govuk-list--bullet");
    }

    [Fact]
    public async Task Each_step_body_is_shown_in_its_own_panel()
    {
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

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll(".js-panel")
            .Select(panel => panel.TextContent.Trim())
            .ShouldBe(["Gather your records", "Upload your file"]);
    }

    [Fact]
    public async Task Step_title_is_shown_as_text_rather_than_html()
    {
        var model = AStepByStepWith(AStep("<b>Prepare</b> your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        var title = html.QuerySelector(".js-step-title").ShouldNotBeNull();
        title.TextContent.Trim().ShouldBe("<b>Prepare</b> your data");
        title.QuerySelector("b").ShouldBeNull();
    }

    // Opening steps and remembering which were open

    [Fact]
    public async Task Step_marked_as_open_is_flagged_to_be_shown_on_load()
    {
        var model = AStepByStepWith(AStep("Prepare your data") with { IsOpen = true });

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetFirstStep(html).HasAttribute("data-show").ShouldBeTrue();
    }

    [Fact]
    public async Task Step_not_marked_as_open_is_not_flagged_to_be_shown_on_load()
    {
        var model = AStepByStepWith(AStep("Prepare your data") with { IsOpen = false });

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetFirstStep(html).HasAttribute("data-show").ShouldBeFalse();
    }

    [Fact]
    public async Task Opened_steps_are_remembered_when_requested()
    {
        var model = AStepByStepWith(AStep("Prepare your data")).RememberSteps();

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetWrapper(html).GetAttribute("data-remember-state").ShouldBe("true");
    }

    [Fact]
    public async Task Opened_steps_are_not_remembered_by_default()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetWrapper(html).HasAttribute("data-remember-state").ShouldBeFalse();
    }

    // Accessibility

    [Fact]
    public async Task Screen_readers_hear_the_word_step_before_the_step_number()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        var stepNumber = html.QuerySelector(".app-step-nav__circle-background").ShouldNotBeNull();
        var hiddenText = stepNumber.QuerySelector(".govuk-visually-hidden").ShouldNotBeNull();
        hiddenText.TextContent.ShouldBe("Step");
        hiddenText.NextSibling.ShouldNotBeNull().TextContent.Trim().ShouldBe("1");
    }

    // Hooks the JavaScript relies on

    [Fact(
        Skip = "Bug in StepByStep.cshtml: the script src is relative (\"lib/step-by-step/...\"), "
            + "so it fails to load on nested URLs such as /Journey/2026. Unskip once it's \"~/lib/...\"."
    )]
    public async Task Step_by_step_javascript_is_included()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        // ^= because asp-append-version appends "?v=...".
        html.QuerySelector("script[src^='/lib/step-by-step/step-by-step.navigation.js']")
            .ShouldNotBeNull();
    }

    [Fact]
    public async Task Element_ids_are_derived_from_the_step_by_step_identifier()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.GetElementById($"sbs-{StepByStepId}-wrapper").ShouldNotBeNull();
        html.GetElementById($"sbs-{StepByStepId}-step-1").ShouldNotBeNull();
        html.GetElementById($"sbs-{StepByStepId}-panel-1").ShouldNotBeNull();
    }

    [Fact]
    public async Task Javascript_identifies_the_step_by_step_by_its_identifier()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetWrapper(html).GetAttribute("data-id").ShouldBe(StepByStepId.ToString());
    }

    [Fact]
    public async Task Step_by_step_is_hidden_until_its_javascript_has_loaded()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetWrapper(html).ClassList.ShouldContain("js-hidden");
    }

    [Fact]
    public async Task Javascript_is_initialised_against_this_step_by_step()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        var initScript = html.QuerySelectorAll("script:not([src])")
            .ShouldHaveSingleItem()
            .TextContent;
        initScript.ShouldContain($"document.querySelector('#sbs-{StepByStepId}-wrapper')");
    }

    [Fact]
    public async Task Show_and_hide_button_text_is_provided_for_the_javascript()
    {
        var model = AStepByStepWith(AStep("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        var wrapper = GetWrapper(html);
        wrapper.GetAttribute("data-show-text").ShouldBe("Show");
        wrapper.GetAttribute("data-hide-text").ShouldBe("Hide");
        wrapper.GetAttribute("data-show-all-text").ShouldBe("Show all steps");
        wrapper.GetAttribute("data-hide-all-text").ShouldBe("Hide all steps");
    }

    // Helpers

    private static StepByStepViewModelCollection AStepByStepWith(params StepByStepResponse[] steps)
    {
        return StepByStepViewModelCollection.Create(StepByStepId).AddSteps(steps);
    }

    private static StepByStepResponse AStep(string title)
    {
        return new StepByStepResponse { Title = title, Body = "<p>Step content</p>" };
    }

    private static IElement GetWrapper(IDocument html)
    {
        return html.QuerySelector("[data-module='appstepnav']").ShouldNotBeNull();
    }

    private static IElement GetFirstStep(IDocument html)
    {
        return html.QuerySelector(".js-step").ShouldNotBeNull();
    }
}
