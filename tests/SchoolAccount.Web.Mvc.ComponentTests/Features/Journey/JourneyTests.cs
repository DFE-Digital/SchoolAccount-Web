using AngleSharp.Dom;
using SchoolAccount.SharedKernel.Authentication;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Journey;
using SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;
using SchoolAccount.Web.Mvc.Features.Shared.StepByStep;
using Shouldly;
using static SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable.MultipleSchoolsStatusTableViewModel;
using StepByStepResponse = SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses.StepByStep;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Journey;

public sealed class JourneyTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Journey/Journey.cshtml";

    private static readonly Guid StepByStepId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    private static readonly MultipleSchoolsStatusTableViewModel TrustWithTwoSchools = new()
    {
        SchoolStatuses =
        [
            new SchoolStatus { Name = "Oakwood Primary", Status = "Submitted" },
            new SchoolStatus { Name = "Riverside Academy", Status = "Not started" },
        ],
    };

    private static readonly Organisation AnEstablishment = new()
    {
        Id = "establishment-1",
        Name = "Oakwood Primary",
        Category = new Category
        {
            Id = Organisation.EstablishmentCategoryId,
            Name = "Establishment",
        },
    };

    private static readonly Organisation ATrust = new()
    {
        Id = "trust-1",
        Name = "Oakwood Academy Trust",
        Category = new Category { Id = "010", Name = "Multi Academy Trust" },
    };

    private static readonly UnderstandStatus SubmittedExplanation = new()
    {
        Name = "Submitted",
        Description = "We have received your return",
    };

    // Heading - always shown

    [Fact]
    public async Task Journey_title_is_the_main_heading()
    {
        var model = AJourney(title: "School census autumn 2026");

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelector("h1")
            .ShouldNotBeNull()
            .TextContent.Trim()
            .ShouldBe("School census autumn 2026");
    }

    [Fact]
    public async Task Caption_is_shown_above_the_heading()
    {
        var model = AJourney(caption: "Statutory return");

        using var html = await renderer.RenderAsync(ViewPath, model);

        var caption = html.QuerySelector(".govuk-caption-xl").ShouldNotBeNull();
        var heading = html.QuerySelector("h1").ShouldNotBeNull();
        caption.TextContent.Trim().ShouldBe("Statutory return");
        caption.CompareDocumentPosition(heading).ShouldBe(DocumentPositions.Following);
    }

    // Status tag - establishment only

    [Fact]
    public async Task Establishment_sees_its_status_as_a_tag()
    {
        var model = AJourney(organisation: AnEstablishment, status: "In progress");

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetJourneyStatusTag(html).ShouldNotBeNull().TextContent.Trim().ShouldBe("In progress");
    }

    [Fact]
    public async Task Trust_does_not_see_a_status_tag()
    {
        var model = AJourney(organisation: ATrust, trustSchools: TrustWithTwoSchools);

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetJourneyStatusTag(html).ShouldBeNull();
    }

    // Overview - only when not blank

    [Fact]
    public async Task Overview_is_shown_when_provided()
    {
        var model = AJourney(overview: "Tell us about your pupils");

        using var html = await renderer.RenderAsync(ViewPath, model);

        var overview = GetSectionHeading(html, "Overview").NextElementSibling.ShouldNotBeNull();
        overview.TextContent.Trim().ShouldBe("Tell us about your pupils");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Overview_is_hidden_when_blank(string? blankOverview)
    {
        var model = AJourney(overview: blankOverview);

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldNotContain("Overview");
    }

    // Schools table - only when there are schools

    [Fact]
    public async Task Schools_table_is_shown_when_there_are_school_statuses()
    {
        var model = AJourney(organisation: ATrust, trustSchools: TrustWithTwoSchools);

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll("table tbody tr")
            .Length.ShouldBe(TrustWithTwoSchools.SchoolStatuses.Count);
    }

    [Fact]
    public async Task Schools_table_is_hidden_when_school_statuses_are_missing()
    {
        var model = AJourney(organisation: AnEstablishment, trustSchools: null);

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelector("table").ShouldBeNull();
    }

    [Fact]
    public async Task Schools_table_is_hidden_when_there_are_no_school_statuses()
    {
        var model = AJourney(
            organisation: ATrust,
            trustSchools: new MultipleSchoolsStatusTableViewModel()
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelector("table").ShouldBeNull();
    }

    // Step by step - only when there are steps

    [Fact]
    public async Task Step_by_step_is_shown_when_there_are_steps()
    {
        var model = AJourney(steps: StepsTitled("Prepare your data"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldContain("Step by step");
        var stepTitle = html.QuerySelector(".js-step-title").ShouldNotBeNull();
        stepTitle.TextContent.Trim().ShouldBe("Prepare your data");
    }

    [Fact]
    public async Task Step_by_step_is_hidden_when_there_are_no_steps()
    {
        var model = AJourney(steps: StepsTitled());

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldNotContain("Step by step");
        html.QuerySelector(".app-step-nav").ShouldBeNull();
    }

    [Fact]
    public async Task Step_by_step_is_hidden_when_steps_are_missing()
    {
        var model = AJourney(steps: null);

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldNotContain("Step by step");
        html.QuerySelector(".app-step-nav").ShouldBeNull();
    }

    // Call to action button - establishment only

    [Fact]
    public async Task Call_to_action_button_links_to_the_service()
    {
        var model = AJourney(
            callToAction: new CallToAction
            {
                Label = "Go to Collect",
                Url = new Uri("https://collect.example.gov.uk/"),
            }
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        // ShouldContain: the button also holds visually hidden text.
        var button = GetCallToAction(html);
        button.GetAttribute("href").ShouldBe("https://collect.example.gov.uk/");
        button.TextContent.ShouldContain("Go to Collect");
    }

    [Fact]
    public async Task Trust_does_not_see_the_call_to_action()
    {
        var model = AJourney(organisation: ATrust, trustSchools: TrustWithTwoSchools);

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelector("a.govuk-button").ShouldBeNull();
    }

    [Fact]
    public async Task Call_to_action_opens_in_a_new_tab_without_sharing_the_referrer()
    {
        var model = AJourney();

        using var html = await renderer.RenderAsync(ViewPath, model);

        var button = GetCallToAction(html);
        button.GetAttribute("target").ShouldBe("_blank");
        button.GetAttribute("rel").ShouldBe("noreferrer noopener");
    }

    [Fact]
    public async Task Call_to_action_tells_screen_reader_users_it_opens_in_a_new_tab()
    {
        var model = AJourney();

        using var html = await renderer.RenderAsync(ViewPath, model);

        var hiddenText = GetCallToAction(html).QuerySelector(".govuk-visually-hidden");
        hiddenText.ShouldNotBeNull().TextContent.Trim().ShouldBe("opens in new tab");
    }

    // Important dates - only when there are dates

    [Fact]
    public async Task Important_dates_are_listed_in_order_with_their_dates()
    {
        var model = AJourney(
            importantDates:
            [
                new ImportantDate { Label = "Census day", FormattedDate = "1 October 2026" },
                new ImportantDate { Label = "Deadline", FormattedDate = "28 October 2026" },
            ]
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldContain("Important dates");
        GetSummaryListRows(html)
            .ShouldBe([("Census day", "1 October 2026"), ("Deadline", "28 October 2026")]);
    }

    [Fact]
    public async Task Important_dates_are_hidden_when_there_are_none()
    {
        var model = AJourney(importantDates: []);

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldNotContain("Important dates");
        html.QuerySelector(".govuk-summary-list").ShouldBeNull();
    }

    // About your status - hidden for "Unavailable" unless the schools table is shown

    [Fact]
    public async Task Status_explanations_are_listed_when_provided()
    {
        var model = AJourney(
            understandStatuses:
            [
                SubmittedExplanation,
                new UnderstandStatus { Name = "Not started", Description = "Nothing sent yet" },
            ]
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldContain("About your status");
        html.QuerySelectorAll("details li")
            .Select(item => item.TextContent.Trim())
            .ShouldBe(["Submitted: We have received your return", "Not started: Nothing sent yet"]);
    }

    [Fact]
    public async Task Status_explanations_are_hidden_when_there_are_none()
    {
        var model = AJourney(understandStatuses: []);

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldNotContain("About your status");
    }

    [Fact]
    public async Task Status_explanations_are_hidden_when_status_is_unavailable()
    {
        var model = AJourney(status: "Unavailable", understandStatuses: [SubmittedExplanation]);

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldNotContain("About your status");
    }

    [Fact]
    public async Task Status_explanations_are_shown_when_status_is_unavailable_but_the_schools_table_is_shown()
    {
        // The explanations describe the table's statuses, so the "Unavailable" rule doesn't apply.
        var model = AJourney(
            organisation: ATrust,
            status: "Unavailable",
            trustSchools: TrustWithTwoSchools,
            understandStatuses: [SubmittedExplanation]
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        GetSectionHeadings(html).ShouldContain("About your status");
    }

    // Support - always shown

    [Fact]
    public async Task Support_section_is_headed_with_the_service_title()
    {
        var model = AJourney(supportService: ASupportService(title: "Get help"));

        using var html = await renderer.RenderAsync(ViewPath, model);

        var heading = GetSupportSection(html).QuerySelector("h2").ShouldNotBeNull();
        heading.TextContent.Trim().ShouldBe("Get help");
    }

    [Fact]
    public async Task Support_section_links_to_the_service_for_help()
    {
        var model = AJourney(
            supportService: ASupportService(
                description: "Contact the support desk",
                url: "https://support.example.gov.uk/"
            )
        );

        using var html = await renderer.RenderAsync(ViewPath, model);

        var link = GetSupportLink(html);
        link.GetAttribute("href").ShouldBe("https://support.example.gov.uk/");
        link.TextContent.Trim().ShouldBe("Contact the support desk");
    }

    [Fact]
    public async Task Support_service_link_opens_in_a_new_tab_without_sharing_the_referrer()
    {
        var model = AJourney();

        using var html = await renderer.RenderAsync(ViewPath, model);

        var link = GetSupportLink(html);
        link.GetAttribute("target").ShouldBe("_blank");
        link.GetAttribute("rel").ShouldBe("noreferrer noopener");
    }

    // Helpers

    private static JourneyViewModel AJourney(
        Organisation? organisation = null,
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
        return new JourneyViewModel
        {
            Organisation = organisation ?? AnEstablishment,
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
            SupportService = supportService ?? ASupportService(),
        };
    }

    private static SupportService ASupportService(
        string title = "Support",
        string description = "Contact us",
        string url = "https://support.example.gov.uk/"
    )
    {
        return new SupportService
        {
            Title = title,
            Description = description,
            Url = new Uri(url),
        };
    }

    private static StepByStepViewModelCollection StepsTitled(params string[] titles)
    {
        var steps = titles.Select(title => new StepByStepResponse
        {
            Title = title,
            Body = "<p>Body</p>",
        });
        return StepByStepViewModelCollection.Create(StepByStepId).AddSteps(steps);
    }

    private static string[] GetSectionHeadings(IDocument html)
    {
        return html.QuerySelectorAll("h2.govuk-heading-m")
            .Select(heading => heading.TextContent.Trim())
            .ToArray();
    }

    private static IElement GetSectionHeading(IDocument html, string text)
    {
        return html.QuerySelectorAll("h2.govuk-heading-m")
            .Where(heading => heading.TextContent.Trim() == text)
            .ShouldHaveSingleItem();
    }

    private static IElement? GetJourneyStatusTag(IDocument html)
    {
        // Direct child only: step-by-step tags also use .status-tag.
        return html.QuerySelector(".govuk-grid-column-two-thirds > strong.status-tag");
    }

    private static IElement GetCallToAction(IDocument html)
    {
        return html.QuerySelector("a.govuk-button").ShouldNotBeNull();
    }

    private static IElement GetSupportSection(IDocument html)
    {
        return html.QuerySelector(".feedback-banner").ShouldNotBeNull();
    }

    private static IElement GetSupportLink(IDocument html)
    {
        return GetSupportSection(html).QuerySelector("a").ShouldNotBeNull();
    }

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
