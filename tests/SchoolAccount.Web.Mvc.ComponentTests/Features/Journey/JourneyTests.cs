using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Journey;
using SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;
using Shouldly;
using static SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable.MultipleSchoolsStatusTableViewModel;
using StepByStepResponse = SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses.StepByStep;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Journey;

/// <summary>
/// Tests for the census journey page (Journey.cshtml).
/// </summary>
/// <remarks>
/// <para>
/// This page has the most conditional logic in the site: most sections only appear in
/// certain situations. The approach is simple. For every <c>@if</c> in the view there is one
/// test proving the section appears when it should, and one proving it doesn't when it
/// shouldn't.
/// </para>
/// <para>
/// Page layout and what controls each part:
/// </para>
/// <code>
/// ┌────────────────────────────────────────┬──────────────────────────────┐
/// │ Caption + title (always)               │ Important dates              │
/// │ Status tag (single school only)        │   (only if there are dates)  │
/// │ Overview (only if not blank)           │ About your status            │
/// │ Schools table (trust/LA only)          │   (only if explanations, and │
/// │ Step by step (only if there are steps) │    not "Unavailable" for a   │
/// │ Call to action button (always)         │    single school)            │
/// │                                        │ Support (always)             │
/// └────────────────────────────────────────┴──────────────────────────────┘
/// </code>
/// <para>
/// The page shows a single school view or a trust/local authority view. A user counts as a
/// trust or local authority when <c>MatSchoolsStatuses</c> contains at least one school.
/// </para>
/// </remarks>
public class JourneyTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Journey/Journey.cshtml";

    // Example data shared by several tests. "static readonly" means it is created once and
    // never changed.
    private static readonly MultipleSchoolsStatusTableViewModel TrustWithTwoSchools = new()
    {
        SchoolStatuses =
        [
            new SchoolStatus { Name = "Oakwood Primary", Status = "Submitted" },
            new SchoolStatus { Name = "Riverside Academy", Status = "Not started" },
        ],
    };

    private static readonly UnderstandStatus SubmittedExplanation = new()
    {
        Name = "Submitted",
        Description = "We have received your return",
    };

    // =====================================================================================
    // Heading - always shown
    // =====================================================================================

    [Fact]
    public async Task Journey_title_is_the_main_heading()
    {
        // Arrange
        var model = AJourney(title: "School census autumn 2026");

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelector("h1")
            .ShouldNotBeNull()
            .TextContent.ShouldBe("School census autumn 2026");
    }

    [Fact]
    public async Task Caption_is_shown_above_the_heading()
    {
        // Arrange
        var model = AJourney(caption: "Statutory return");

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var caption = html.QuerySelector(".govuk-caption-xl").ShouldNotBeNull();
        caption.TextContent.ShouldBe("Statutory return");
    }

    // =====================================================================================
    // Status tag - single school only
    // =====================================================================================

    [Fact]
    public async Task Single_school_sees_its_status_as_a_tag()
    {
        // Arrange
        var model = AJourney(status: "In progress");

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetJourneyStatusTag(html).ShouldNotBeNull().TextContent.ShouldBe("In progress");
    }

    [Fact]
    public async Task Trust_does_not_see_a_single_status_tag()
    {
        // Arrange
        var model = AJourney(trustSchools: TrustWithTwoSchools);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetJourneyStatusTag(html).ShouldBeNull();
    }

    [Fact]
    public async Task Trust_with_no_schools_is_shown_the_single_school_view()
    {
        // Arrange
        // An edge case: a trust model exists, but it lists no schools. The view treats this
        // the same as a single school.
        var model = AJourney(trustSchools: new MultipleSchoolsStatusTableViewModel());

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetJourneyStatusTag(html).ShouldNotBeNull();
        html.QuerySelector("table").ShouldBeNull();
    }

    // =====================================================================================
    // Overview - only when not blank
    // =====================================================================================

    [Fact]
    public async Task Overview_is_shown_when_provided()
    {
        // Arrange
        var model = AJourney(overview: "Tell us about your pupils");

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // "h2 + p.govuk-body" means "a <p class='govuk-body'> directly after an <h2>".
        GetSectionHeadings(html).ShouldContain("Overview");
        var overview = html.QuerySelector("h2 + p.govuk-body").ShouldNotBeNull();
        overview.TextContent.ShouldBe("Tell us about your pupils");
    }

    // [Theory] runs the same test once for each [InlineData] row, passing that row's value in
    // as the method's parameter. Use it when only the input changes between tests.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Overview_is_hidden_when_blank(string? blankOverview)
    {
        // Arrange
        var model = AJourney(overview: blankOverview);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldNotContain("Overview");
    }

    // =====================================================================================
    // Schools table - trust or local authority only
    // =====================================================================================

    [Fact]
    public async Task Trust_sees_a_table_of_its_schools()
    {
        // Arrange
        var model = AJourney(trustSchools: TrustWithTwoSchools);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The table itself is tested in MultipleSchoolsStatusTableTests. Here we only check
        // that the page includes it.
        html.QuerySelectorAll("table tbody tr").Length.ShouldBe(2);
    }

    [Fact]
    public async Task Single_school_does_not_see_a_schools_table()
    {
        // Arrange
        var model = AJourney(trustSchools: null);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelector("table").ShouldBeNull();
    }

    // =====================================================================================
    // Step by step - only when there are steps
    // =====================================================================================

    [Fact]
    public async Task Step_by_step_is_shown_when_there_are_steps()
    {
        // Arrange
        var model = AJourney(steps: StepsTitled("Prepare your data"));

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The component itself is tested in StepByStepTests. Here we only check the page
        // includes it.
        GetSectionHeadings(html).ShouldContain("Step by step");
        var stepTitle = html.QuerySelector(".js-step-title").ShouldNotBeNull();
        stepTitle.TextContent.ShouldBe("Prepare your data");
    }

    [Fact]
    public async Task Step_by_step_is_hidden_when_there_are_no_steps()
    {
        // Arrange
        var model = AJourney(steps: StepsTitled());

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldNotContain("Step by step");
        html.QuerySelector(".app-step-nav").ShouldBeNull();
    }

    [Fact]
    public async Task Step_by_step_is_hidden_when_steps_are_missing()
    {
        // Arrange
        var model = AJourney(steps: null);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldNotContain("Step by step");
    }

    // =====================================================================================
    // Call to action button - always shown
    // =====================================================================================

    [Fact]
    public async Task Call_to_action_button_links_to_the_service()
    {
        // Arrange
        var model = AJourney(
            callToAction: new CallToAction
            {
                Label = "Go to Collect",
                Url = new Uri("https://collect.example.gov.uk/"),
            }
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var button = GetCallToAction(html);
        button.GetAttribute("href").ShouldBe("https://collect.example.gov.uk/");
        button.TextContent.ShouldContain("Go to Collect");
    }

    [Fact]
    public async Task Call_to_action_opens_in_a_new_tab_without_sharing_the_referrer()
    {
        // Arrange
        var model = AJourney();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // target="_blank" opens a new tab. rel="noreferrer noopener" stops the other site
        // seeing which page linked to it, or controlling this tab.
        var button = GetCallToAction(html);
        button.GetAttribute("target").ShouldBe("_blank");
        button.GetAttribute("rel").ShouldBe("noreferrer noopener");
    }

    [Fact]
    public async Task Call_to_action_tells_screen_reader_users_it_opens_in_a_new_tab()
    {
        // Arrange
        var model = AJourney();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var hiddenText = GetCallToAction(html).QuerySelector(".govuk-visually-hidden");
        hiddenText.ShouldNotBeNull().TextContent.Trim().ShouldBe("opens in new tab");
    }

    // =====================================================================================
    // Important dates - only when there are dates
    // =====================================================================================

    [Fact]
    public async Task Important_dates_are_listed_in_order_with_their_dates()
    {
        // Arrange
        var model = AJourney(
            importantDates:
            [
                new ImportantDate { Label = "Census day", FormattedDate = "1 October 2026" },
                new ImportantDate { Label = "Deadline", FormattedDate = "28 October 2026" },
            ]
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldContain("Important dates");
        GetSummaryListRows(html)
            .ShouldBe([("Census day", "1 October 2026"), ("Deadline", "28 October 2026")]);
    }

    [Fact]
    public async Task Important_dates_are_hidden_when_there_are_none()
    {
        // Arrange
        var model = AJourney(importantDates: []);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldNotContain("Important dates");
        html.QuerySelector(".govuk-summary-list").ShouldBeNull();
    }

    // =====================================================================================
    // About your status - only when there are explanations, and not "Unavailable" for a
    // single school
    // =====================================================================================

    [Fact]
    public async Task Status_explanations_are_listed_when_provided()
    {
        // Arrange
        var model = AJourney(
            understandStatuses:
            [
                SubmittedExplanation,
                new UnderstandStatus { Name = "Not started", Description = "Nothing sent yet" },
            ]
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The explanations sit inside an expandable <details> element.
        GetSectionHeadings(html).ShouldContain("About your status");
        html.QuerySelectorAll("details li")
            .Select(item => item.TextContent)
            .ShouldBe(["Submitted: We have received your return", "Not started: Nothing sent yet"]);
    }

    [Fact]
    public async Task Status_explanations_are_hidden_when_there_are_none()
    {
        // Arrange
        var model = AJourney(understandStatuses: []);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldNotContain("About your status");
    }

    [Fact]
    public async Task Status_explanations_are_hidden_when_a_single_schools_status_is_unavailable()
    {
        // Arrange
        var model = AJourney(status: "Unavailable", understandStatuses: [SubmittedExplanation]);

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldNotContain("About your status");
    }

    [Fact]
    public async Task Status_explanations_are_shown_to_a_trust_even_when_status_is_unavailable()
    {
        // Arrange
        // Same as the previous test, but for a trust: the "Unavailable" rule doesn't apply.
        var model = AJourney(
            status: "Unavailable",
            trustSchools: TrustWithTwoSchools,
            understandStatuses: [SubmittedExplanation]
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        GetSectionHeadings(html).ShouldContain("About your status");
    }

    // =====================================================================================
    // Support - always shown
    // =====================================================================================

    [Fact]
    public async Task Support_service_is_shown_with_a_link_for_help()
    {
        // Arrange
        var model = AJourney(
            supportService: new SupportService
            {
                Title = "Get help",
                Description = "Contact the support desk",
                Url = new Uri("https://support.example.gov.uk/"),
            }
        );

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var banner = html.QuerySelector(".feedback-banner").ShouldNotBeNull();
        var heading = banner.QuerySelector("h2").ShouldNotBeNull();
        var link = banner.QuerySelector("a").ShouldNotBeNull();
        heading.TextContent.ShouldBe("Get help");
        link.GetAttribute("href").ShouldBe("https://support.example.gov.uk/");
        link.TextContent.Trim().ShouldBe("Contact the support desk");
    }

    [Fact]
    public async Task Support_service_link_opens_in_a_new_tab_without_sharing_the_referrer()
    {
        // Arrange
        var model = AJourney();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        var link = html.QuerySelector(".feedback-banner a").ShouldNotBeNull();
        link.GetAttribute("target").ShouldBe("_blank");
        link.GetAttribute("rel").ShouldBe("noreferrer noopener");
    }

    // =====================================================================================
    // Helpers - these keep the tests short so each one shows only what makes it different
    // =====================================================================================

    /// <summary>
    /// Builds a journey page model for a single school, with every optional section empty.
    /// </summary>
    /// <remarks>
    /// Every parameter has a sensible default, so a test only names the values it cares
    /// about, e.g. <c>AJourney(status: "Unavailable")</c>. Anything not mentioned gets a
    /// harmless default value.
    /// </remarks>
    private static JourneyViewModel AJourney(
        string title = "School census",
        string caption = "Statutory return",
        string status = "In progress",
        string? overview = null,
        MultipleSchoolsStatusTableViewModel? trustSchools = null,
        StepByStepViewModelCollection? steps = null,
        IReadOnlyList<ImportantDate>? importantDates = null,
        IReadOnlyList<UnderstandStatus>? understandStatuses = null,
        CallToAction? callToAction = null,
        SupportService? supportService = null
    )
    {
        // "??" means "use the value on the left, or if it's null, the value on the right".
        return new JourneyViewModel
        {
            Title = title,
            Caption = caption,
            Status = status,
            Overview = overview,
            MatSchoolsStatuses = trustSchools,
            Steps = steps,
            ImportantDates = importantDates ?? [],
            UnderstandStatuses = understandStatuses ?? [],
            CallToAction =
                callToAction
                ?? new CallToAction
                {
                    Label = "Start now",
                    Url = new Uri("https://service.example.gov.uk/"),
                },
            SupportService =
                supportService
                ?? new SupportService
                {
                    Title = "Support",
                    Description = "Contact us",
                    Url = new Uri("https://support.example.gov.uk/"),
                },
        };
    }

    /// <summary>
    /// Builds a step-by-step containing one step per title. Pass none for an empty one.
    /// </summary>
    private static StepByStepViewModelCollection StepsTitled(params string[] titles)
    {
        var steps = titles.Select(title => new StepByStepResponse
        {
            Title = title,
            Body = "<p>Body</p>",
        });
        return StepByStepViewModelCollection.Create(Guid.NewGuid()).AddSteps(steps);
    }

    /// <summary>
    /// The text of every section heading on the page, e.g. ["Overview", "Important dates"].
    /// Checking this list is an easy way to see which optional sections were shown.
    /// </summary>
    private static string[] GetSectionHeadings(IDocument html)
    {
        return html.QuerySelectorAll("h2.govuk-heading-m")
            .Select(heading => heading.TextContent.Trim())
            .ToArray();
    }

    /// <summary>
    /// The single school's status tag, or null if it isn't shown.
    /// </summary>
    private static IElement? GetJourneyStatusTag(IDocument html)
    {
        // ">" means "directly inside". Step-by-step status tags also use the "status-tag"
        // class, but they are nested deeper, so this only matches the page's own tag.
        return html.QuerySelector(".govuk-grid-column-two-thirds > strong.status-tag");
    }

    private static IElement GetCallToAction(IDocument html)
    {
        return html.QuerySelector("a.govuk-button").ShouldNotBeNull();
    }

    /// <summary>
    /// Reads the "Important dates" summary list as (label, date) pairs.
    /// </summary>
    private static (string Key, string Value)[] GetSummaryListRows(IDocument html)
    {
        return html.QuerySelectorAll(".govuk-summary-list__row")
            .Select(row =>
                (
                    row.QuerySelector(".govuk-summary-list__key")
                        .ShouldNotBeNull()
                        .TextContent.Trim(),
                    row.QuerySelector(".govuk-summary-list__value")
                        .ShouldNotBeNull()
                        .TextContent.Trim()
                )
            )
            .ToArray();
    }
}
