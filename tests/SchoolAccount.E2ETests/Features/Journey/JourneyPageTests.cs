using SchoolAccount.E2ETests.Common.Fixtures;
using SchoolAccount.E2ETests.Common.Pages;
using SchoolAccount.TestCommon.Constants;
using Shouldly;
using static Microsoft.Playwright.Assertions;

namespace SchoolAccount.E2ETests.Features.Journey;

[Collection(SchoolAccountCollection.Name)]
public class JourneyPageTests : BrowserTestBase
{
    public JourneyPageTests(SchoolAccountBrowserFixture fixture)
        : base(fixture) { }

    // A page object only holds locators, so creating one on each use costs nothing.
    private JourneyPage JourneyPage => new(Page);

    protected override async Task OnInitializedAsync()
    {
        await Page.GotoAsync($"{Settings.SiteUrl}{UrlConstants.JourneyUrl}");
    }

    [Fact]
    public async Task Header_shows_the_signed_in_school_name()
    {
        // Act
        var schoolName = await JourneyPage.Header.GetSchoolNameAsync();

        // Assert
        Assert.NotEmpty(schoolName);
    }

    [Fact]
    public async Task Heading_names_the_census()
    {
        // Act & Assert
        await Expect(JourneyPage.Journey.Heading).ToContainTextAsync("census");
    }

    [Fact]
    public async Task Census_return_status_is_shown()
    {
        // Act
        var status = await JourneyPage.Journey.GetStatusAsync();

        // Assert
        Assert.NotEmpty(status);
    }

    [Fact]
    public async Task Step_by_step_shows_at_least_one_step()
    {
        // Act
        var stepCount = await JourneyPage.StepByStep.Steps.CountAsync();

        // Assert
        stepCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Important_dates_show_census_day_and_return_due_date()
    {
        // Act
        var censusDay = await JourneyPage.ImportantDates.GetCensusDayAsync();
        var returnDue = await JourneyPage.ImportantDates.GetReturnDueDateAsync();

        // Assert
        Assert.NotEmpty(censusDay);
        Assert.NotEmpty(returnDue);
    }

    [Fact]
    public async Task Go_to_COLLECT_button_is_shown()
    {
        // Act & Assert
        await Expect(JourneyPage.Journey.GoToCollectButton).ToBeVisibleAsync();
    }
}
