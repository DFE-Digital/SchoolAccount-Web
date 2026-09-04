using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.CensusJourneyResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseImportantDateBuilder;

namespace SchoolAccount.IntegrationTests.Features.Journey;

public class JourneyControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly Uri _callToActionUri = new("https://www.gov.uk/");
    private readonly HttpClient _client;
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly StubCensusJourneyHandler _getCensusJourneyHandler = new();

    public JourneyControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(_getCensusJourneyHandler)
        );
    }

    [Fact]
    public async Task Page_successfully_renders()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var journeyResult = ACensusJourneyResponse()
            .WithTitle("Test Journey Title")
            .WithCaption("This is a test caption")
            .WithOverview("This is a test overview")
            .WithStatus("Test Status")
            .WithCallToActionLabel("Test Call To Action")
            .WithCallToActionUrl(_callToActionUri)
            .WithImportantDate(
                AnImportantDate().WithLabel("Test Important Date").WithDate(2026, 10, 1)
            )
            .AsSuccess();

        _getCensusJourneyHandler.Returns(journeyResult);

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Journey");

        var pageHeading = page.GetFirstHeading();
        pageHeading.ShouldNotBeNull();
        pageHeading.ShouldBeEquivalentTo("Test Journey Title");

        var pageBody = page.GetFirstBodyParagraph();
        pageBody.ShouldNotBeNull();
        pageBody.ShouldBeEquivalentTo("This is a test overview");

        var pageCaption = page.GetFirstCaption();
        pageCaption.ShouldNotBeNull();
        pageCaption.ShouldBeEquivalentTo("This is a test caption");

        var pageTag = page.GetFirstTag();
        pageTag.ShouldNotBeNull();
        pageTag.ShouldBeEquivalentTo("Test Status");

        var pageImportantDates = page.GetSummaryListPairs();
        pageImportantDates.ShouldNotBeNull();
        pageImportantDates.Count.ShouldBe(1);
        pageImportantDates.ShouldContainKeyAndValue<string, string>(
            "Test Important Date",
            "1 October 2026"
        );

        var callToActionButton = page.GetButtonByLink(_callToActionUri.ToString());
        callToActionButton.ShouldNotBeNull();
        callToActionButton.TextContent.Trim().ShouldStartWith("Test Call To Action");
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public async Task Overview_does_not_render_for_an_empty_string(string overview)
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var journeyResult = ACensusJourneyResponse().WithOverview(overview).AsSuccess();

        _getCensusJourneyHandler.Returns(journeyResult);

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pageBody = page.GetFirstBodyParagraph();
        pageBody.ShouldBeNull();
    }

    [Fact]
    public async Task ImportantDates_does_not_render_when_null_or_empty()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var journeyResult = ACensusJourneyResponse().AsSuccess();

        _getCensusJourneyHandler.Returns(journeyResult);

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pageImportantDates = page.GetSummaryListPairs();
        pageImportantDates.ShouldNotBeNull();
    }

    [Fact]
    public async Task ImportantDates_renders_multiple_dates()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var journeyResult = ACensusJourneyResponse()
            .WithImportantDates(
                AnImportantDate().WithLabel("Census due").WithDate(2026, 10, 1),
                AnImportantDate().WithLabel("Return date").WithDate(2026, 10, 28)
            )
            .AsSuccess();

        _getCensusJourneyHandler.Returns(journeyResult);

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pageImportantDates = page.GetSummaryListPairs();
        pageImportantDates.ShouldNotBeNull();
        pageImportantDates.Count.ShouldBe(2);
        pageImportantDates.ShouldContainKeyAndValue<string, string>("Census due", "1 October 2026");
        pageImportantDates.ShouldContainKeyAndValue<string, string>(
            "Return date",
            "28 October 2026"
        );
    }

    [Fact]
    public async Task Multiple_ImportantDates_are_ordered_earliest_first()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var journeyResult = ACensusJourneyResponse()
            .WithImportantDates(
                AnImportantDate().WithLabel("Later").WithDate(2026, 11, 15),
                AnImportantDate().WithLabel("Earlier").WithDate(2026, 10, 1)
            )
            .AsSuccess();

        _getCensusJourneyHandler.Returns(journeyResult);

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var importantDateRows = page.GetSummaryListRows();
        importantDateRows.ShouldNotBeNull();
        importantDateRows[0].ShouldBe(("Earlier", "1 October 2026"));
        importantDateRows[1].ShouldBe(("Later", "15 November 2026"));
    }

    [Fact]
    public async Task Call_to_action_button_has_hidden_opens_in_new_tab_text_for_screen_readers()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");
        _getCensusJourneyHandler.Returns(ACensusJourneyResponse().AsSuccess());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var callToActionButton = page.GetButtonByLink(
            "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
        );
        var hiddenSpan = callToActionButton?.QuerySelector("span.govuk-visually-hidden");

        callToActionButton.ShouldNotBeNull();
        hiddenSpan.ShouldNotBeNull();
        callToActionButton.TextContent.Trim().ShouldStartWith("Go to Autumn Census 2026");
        callToActionButton.TextContent.Trim().ShouldEndWith("opens in new tab");
        hiddenSpan.TextContent.Trim().ShouldStartWith("opens in new tab");
    }

    [Fact]
    public async Task There_are_steps_on_the_page_by_default()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Journey", "Journey");
        _getCensusJourneyHandler.Returns(ACensusJourneyResponse().WithSteps().AsSuccess());

        // Act
        var message = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            message,
            TestContext.Current.CancellationToken
        );

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var component = page.GetStepsComponent();
        component.IsPresent.ShouldBeTrue();

        var steps = component.GetSteps();
        steps.Count.ShouldBeGreaterThan(0);
        steps
            .Select(x => x.GetTitle())
            .ShouldBeSubsetOf(["This is a fake step 1", "This is a fake step 2"]);
    }
}
