using System.Net;
using System.Security.Claims;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;
using static SchoolAccount.TestCommon.Builders.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyContentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseImportantDateBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseUnderstandStatusBuilder;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Features.Journey;

public class JourneyControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly StubCensusJourneyHandler _getCensusJourneyHandler = new();
    private readonly Uri _testUri = new("https://www.gov.uk/");

    public JourneyControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(_getCensusJourneyHandler)
        );
    }

    [Fact]
    public async Task Page_content_successfully_renders()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var journeyResult = AGetCensusJourneyResponse()
            .WithContent(
                ACensusJourneyContentResponse()
                    .WithTitle("Test Journey Title")
                    .WithCaption("This is a test caption")
                    .WithOverview("This is a test overview")
                    .WithStatus("Test Status")
                    .WithSupportServiceTitle("Test Support Service")
                    .WithCallToActionLabel("Test Call To Action")
                    .WithCallToActionUrl(_testUri)
                    .WithImportantDate(
                        AnImportantDate().WithLabel("Test Important Date").WithDate(2026, 10, 1)
                    )
            )
            .AsSuccess();

        _getCensusJourneyHandler.Returns(journeyResult);

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.GetTitle().ShouldBe("Journey");
        page.GetFirstHeading().ShouldBe("Test Journey Title");
        page.GetFirstBodyParagraph().ShouldBe("This is a test overview");
        page.GetFirstCaption().ShouldBe("This is a test caption");
        page.GetFirstTag().ShouldBe("Test Status");
        page.GetFirstCaption().ShouldBe("This is a test caption");
        page.GetFirstTag().ShouldBe("Test Status");
        page.GetFirstTag().ShouldBe("Test Status");
        page.GetComponentByContent(".govuk-heading-m", "Test Support Service")
            .ShouldBe("Test Support Service");

        var pageImportantDates = page.GetSummaryListPairs();
        pageImportantDates.ShouldNotBeNull();
        pageImportantDates.Count.ShouldBe(1);
        pageImportantDates.ShouldContainKeyAndValue<string, string>(
            "Test Important Date",
            "1 October 2026"
        );

        var callToActionButton = page.GetButtonByLink(_testUri.ToString());
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

        var journeyResult = AGetCensusJourneyResponse()
            .WithContent(ACensusJourneyContentResponse().WithOverview(overview))
            .AsSuccess();

        _getCensusJourneyHandler.Returns(journeyResult);

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var overviewHeading = page.GetComponentByContent(".govuk-heading-m", "Overview");
        overviewHeading.ShouldBeNull();
    }

    [Fact]
    public async Task ImportantDates_does_not_render_when_null_or_empty()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var journeyResult = AGetCensusJourneyResponse()
            .WithContent(ACensusJourneyContentResponse())
            .AsSuccess();

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

        var journeyResult = AGetCensusJourneyResponse()
            .WithContent(
                ACensusJourneyContentResponse()
                    .WithImportantDates(
                        AnImportantDate().WithLabel("Census due").WithDate(2026, 10, 1),
                        AnImportantDate().WithLabel("Return date").WithDate(2026, 10, 28)
                    )
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

        var journeyResult = AGetCensusJourneyResponse()
            .WithContent(
                ACensusJourneyContentResponse()
                    .WithImportantDates(
                        AnImportantDate().WithLabel("Later").WithDate(2026, 11, 15),
                        AnImportantDate().WithLabel("Earlier").WithDate(2026, 10, 1)
                    )
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
        _getCensusJourneyHandler.Returns(AGetCensusJourneyResponse().AsSuccess());

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
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(ACensusJourneyContentResponse().WithSteps())
                .AsSuccess()
        );

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

    [Fact]
    public async Task School_status_table_not_displayed_when_not_a_mat()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Journey", "Journey");
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithSchoolStatuses(
                    ACensusStatusResponse()
                        .WithName("Test School 1")
                        .WithAction("Autumn Census 2026", "Not Started"),
                    ACensusStatusResponse()
                        .WithName("Test School 2")
                        .WithAction("Autumn Census 2026", "Submitted")
                )
                .AsSuccess()
        );

        // Act
        var message = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            message,
            TestContext.Current.CancellationToken
        );

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        page.GetTableHeaders().ShouldBeEmpty();
        page.GetTableRows().ShouldBeEmpty();
        page.GetTableCellByHeader().ShouldBeEmpty();
    }

    [Fact]
    public async Task School_statuses_are_displayed_for_multi_academy_trust()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var trustJson = """
            {
             "id": "2E774B32-E4DB-445B-B915-736C777FF5A4",
             "name": "Test Multi Academy Trust",
             "category": { "id": "010", "name": "Multi Academy Trust" },
             "ukprn": "10037611",
             "establishments": [
                 { "urn": "100001", "name": "Test School 1" },
                 { "urn": "100002", "name": "Test School 2" }
             ]
            }
            """;

        var client = _factory.CreateAuthorisedClient(services =>
        {
            services.StubQueryHandler(_getCensusJourneyHandler);
            services.AddSingleton(
                new MockAuthClaimsOptions
                {
                    Claims =
                    [
                        new Claim(GivenName, MockAuthHandler.FakeGivenName),
                        new Claim(FamilyName, MockAuthHandler.FakeFamilyName),
                        new Claim(Sub, "1159ee82-d515-4d34-b28d-ac138cb1506b"),
                        new Claim(Email, "test@example.com"),
                        new Claim(Organisation, trustJson),
                    ],
                }
            );
        });

        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithSchoolStatuses(
                    ACensusStatusResponse()
                        .WithName("Test School 1")
                        .WithAction("Autumn Census 2026", "Not Started"),
                    ACensusStatusResponse()
                        .WithName("Test School 2")
                        .WithAction("Autumn Census 2026", "Submitted")
                )
                .AsSuccess()
        );

        // Act
        var message = await client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var tableHeadings = page.GetTableHeaders();
        tableHeadings.ShouldNotBeNull();
        tableHeadings[0].ShouldBe("Name");
        tableHeadings[1].ShouldBe("Status");

        var tableRows = page.GetTableRows();
        tableRows.ShouldNotBeNull();
        tableRows.Count.ShouldBe(2);

        var schoolStatus = page.GetTableCellByHeader();
        schoolStatus.ShouldNotBeNull();
        schoolStatus.ShouldSatisfyAllConditions(
            () => schoolStatus.Count.ShouldBe(2),
            () => schoolStatus[0]["Name"].ShouldBe("Test School 1"),
            () => schoolStatus[0]["Status"].ShouldBe("Not Started"),
            () => schoolStatus[1]["Name"].ShouldBe("Test School 2"),
            () => schoolStatus[1]["Status"].ShouldBe("Submitted")
        );
    }

    [Fact]
    public async Task Understand_statuses_list_is_displayed_when_status_is_not_unavailable()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Journey", "Journey");
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithStatus("Authorised")
                        .WithUnderstandStatus(
                            AUnderstandStatus()
                                .WithName("Test Status")
                                .WithDescription("Test status description.")
                        )
                )
                .AsSuccess()
        );

        // Act
        var message = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            message,
            TestContext.Current.CancellationToken
        );

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.GetComponentByContent("li", "Test Status")
            .ShouldBe("Test Status: Test status description.");
    }

    [Fact]
    public async Task Understand_statuses_list_is_not_displayed_when_status_is_unavailable()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Journey", "Journey");
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithStatus("Unavailable")
                        .WithUnderstandStatus(
                            AUnderstandStatus()
                                .WithName("Test Status")
                                .WithDescription("Test status description.")
                        )
                )
                .AsSuccess()
        );

        // Act
        var message = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            message,
            TestContext.Current.CancellationToken
        );

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.GetComponentByContent("li", "Test Status").ShouldBeNull();
    }

    [Fact]
    public async Task Understand_statuses_list_is_displayed_for_multi_academy_trust()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");

        var trustJson = """
            {
             "id": "2E774B32-E4DB-445B-B915-736C777FF5A4",
             "name": "Test Multi Academy Trust",
             "category": { "id": "010", "name": "Multi Academy Trust" },
             "ukprn": "10037611",
             "establishments": [
                 { "urn": "100001", "name": "Test School 1" },
                 { "urn": "100002", "name": "Test School 2" }
             ]
            }
            """;

        var client = _factory.CreateAuthorisedClient(services =>
        {
            services.StubQueryHandler(_getCensusJourneyHandler);
            services.AddSingleton(
                new MockAuthClaimsOptions
                {
                    Claims =
                    [
                        new Claim(GivenName, MockAuthHandler.FakeGivenName),
                        new Claim(FamilyName, MockAuthHandler.FakeFamilyName),
                        new Claim(Sub, "1159ee82-d515-4d34-b28d-ac138cb1506b"),
                        new Claim(Email, "test@example.com"),
                        new Claim(Organisation, trustJson),
                    ],
                }
            );
        });

        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithUnderstandStatus(
                            AUnderstandStatus()
                                .WithName("Test Status")
                                .WithDescription("Test status description.")
                        )
                )
                .AsSuccess()
        );

        // Act
        var message = await client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.GetComponentByContent("li", "Test Status")
            .ShouldBe("Test Status: Test status description.");
    }
}
