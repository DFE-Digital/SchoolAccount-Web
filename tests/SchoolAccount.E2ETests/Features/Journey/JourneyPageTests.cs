using SchoolAccount.E2ETests.Common.Fixtures;
using SchoolAccount.E2ETests.Common.Pages;
using static Microsoft.Playwright.Assertions;

namespace SchoolAccount.E2ETests.Features.Journey;

[Collection(SchoolAccountCollection.Name)]
public class JourneyPageTests : BrowserTestBase
{
    private JourneyPage _journeyPage = null!;

    public JourneyPageTests(SchoolAccountBrowserFixture fixture)
        : base(fixture) { }

    protected override async Task OnInitializedAsync()
    {
        await Page.GotoAsync($"{Settings.SiteUrl}/Journey");
        _journeyPage = new JourneyPage(Page);
    }

    [Fact]
    public async Task Header_shows_the_signed_in_school_name()
    {
        var schoolName = await _journeyPage.Header.GetSchoolNameAsync();
        Assert.NotEmpty(schoolName);
    }

    [Fact]
    public async Task Heading_names_the_census()
    {
        await Expect(_journeyPage.CensusReturnHeading).ToContainTextAsync("census");
    }

    [Fact]
    public async Task Census_return_status_is_shown()
    {
        var status = await _journeyPage.GetCensusReturnStatusAsync();
        Assert.NotEmpty(status);
    }

    [Fact]
    public async Task Step_by_step_shows_at_least_one_step()
    {
        var stepCount = await _journeyPage.StepByStep.Steps.CountAsync();
        Assert.True(stepCount > 0, "Expected at least one step in the step by step navigation.");
    }

    [Fact]
    public async Task Important_dates_show_census_day_and_return_due_date()
    {
        var censusDay = await _journeyPage.ImportantDates.GetCensusDayAsync();
        var returnDue = await _journeyPage.ImportantDates.GetReturnDueDateAsync();

        Assert.NotEmpty(censusDay);
        Assert.NotEmpty(returnDue);
    }

    [Fact]
    public async Task Go_to_COLLECT_button_is_shown()
    {
        await Expect(_journeyPage.GoToCollectPortalButton).ToBeVisibleAsync();
    }
}
