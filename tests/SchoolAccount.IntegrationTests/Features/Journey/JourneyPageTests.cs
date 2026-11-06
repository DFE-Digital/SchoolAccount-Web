using System.Security.Claims;
using NSubstitute;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Stubs;
using SchoolAccount.Web.Mvc.Features.Journey;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyContentResponse;
using static SchoolAccount.TestCommon.Builders.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyContentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseImportantDateBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseUnderstandStatusBuilder;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Features.Journey;

public class JourneyPageTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private const string matJson = """
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

    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly HttpClient _client;
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly StubCensusJourneyHandler _getCensusJourneyHandler = new();
    private readonly Uri _testUri = new("https://www.gov.uk/");

    public JourneyPageTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(_getCensusJourneyHandler)
        );
    }

    [Fact]
    public async Task The_journey_page_shows_the_census_content()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithTitle("Test Journey Title")
                        .WithCaption("This is a test caption")
                        .WithOverview("This is a test overview")
                        .WithStatus("Test Status")
                        .WithSupportServiceTitle("Test Support Service")
                )
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        journeyPage.ShouldSatisfyAllConditions(
            page => page.GetPageTitle().ShouldBe("Journey"),
            page => page.GetFirstHeading().ShouldBe("Test Journey Title"),
            page => page.GetFirstBodyParagraph().ShouldBe("This is a test overview"),
            page => page.GetFirstCaption().ShouldBe("This is a test caption"),
            page => page.GetFirstTag().ShouldBe("Test Status"),
            page =>
                page.GetElementContentByText(".govuk-heading-m", "Test Support Service")
                    .ShouldBe("Test Support Service")
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public async Task The_overview_is_hidden_when_it_is_blank(string overview)
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(ACensusJourneyContentResponse().WithOverview(overview))
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var overviewHeading = journeyPage.GetElementContentByText(".govuk-heading-m", "Overview");
        overviewHeading.ShouldBeNull();
    }

    [Fact]
    public async Task Important_dates_are_visible_on_the_journey_page()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithImportantDate(
                            AnImportantDate().WithLabel("Test Important Date").WithDate(2026, 10, 1)
                        )
                )
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var importantDates = journeyPage.GetSummaryListPairs();
        importantDates.ShouldNotBeNull();
        importantDates.Count.ShouldBe(1);
        importantDates.ShouldContainKeyAndValue<string, string>(
            "Test Important Date",
            "1 October 2026"
        );
    }

    [Fact]
    public async Task Important_dates_do_not_render_when_empty()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse().WithContent(ACensusJourneyContentResponse()).AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var importantDates = journeyPage.GetSummaryListPairs();
        importantDates.ShouldBeEmpty();
    }

    [Fact]
    public async Task Every_important_date_is_shown()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithImportantDates(
                            AnImportantDate().WithLabel("Census due").WithDate(2026, 10, 1),
                            AnImportantDate().WithLabel("Return date").WithDate(2026, 10, 28)
                        )
                )
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var importantDates = journeyPage.GetSummaryListPairs();
        importantDates.ShouldSatisfyAllConditions(
            dateList => dateList.Count.ShouldBe(2),
            dateList =>
                dateList.ShouldContainKeyAndValue<string, string>("Census due", "1 October 2026"),
            dateList =>
                dateList.ShouldContainKeyAndValue<string, string>("Return date", "28 October 2026")
        );
    }

    [Fact]
    public async Task Important_dates_are_shown_earliest_first()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithImportantDates(
                            AnImportantDate().WithLabel("Later").WithDate(2026, 11, 15),
                            AnImportantDate().WithLabel("Earlier").WithDate(2026, 10, 1)
                        )
                )
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var importantDateRows = journeyPage.GetSummaryListRows();
        importantDateRows.ShouldNotBeNull();
        importantDateRows[0].ShouldBe(("Earlier", "1 October 2026"));
        importantDateRows[1].ShouldBe(("Later", "15 November 2026"));
    }

    [Fact]
    public async Task Call_to_action_button_is_visible_on_the_journey_page()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithCallToActionLabel("Test Call To Action")
                        .WithCallToActionUrl(_testUri)
                )
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var callToActionButton = journeyPage.GetElementByHref(
            "a.govuk-button",
            _testUri.ToString()
        );

        callToActionButton.ShouldNotBeNull();
        journeyPage.GetTextOf(callToActionButton).ShouldBe("Test Call To Action opens in new tab");
    }

    [Fact]
    public async Task Call_to_action_button_has_hidden_opens_in_new_tab_text_for_screen_readers()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(AGetCensusJourneyResponse().AsSuccess());

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var callToActionButton = journeyPage.GetElementByHref(
            "a.govuk-button",
            "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
        );

        callToActionButton.ShouldNotBeNull();
        journeyPage
            .GetTextOf(callToActionButton)
            .ShouldBe("Go to Autumn Census 2026 opens in new tab");
        journeyPage.GetVisuallyHiddenTextOnElement(callToActionButton).ShouldBe("opens in new tab");
    }

    [Fact]
    public async Task Call_to_action_button_is_hidden_for_MAT_users()
    {
        // Arrange
        var client = CreateMatClient();

        _getCensusJourneyHandler.Returns(AGetCensusJourneyResponse().AsSuccess());

        // Act
        var response = await client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var callToActionButton = journeyPage.GetElementByHref(
            "a.govuk-button",
            "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
        );

        callToActionButton.ShouldBeNull();
    }

    [Fact]
    public async Task There_are_steps_on_the_page_by_default()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(ACensusJourneyContentResponse().WithSteps())
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var component = journeyPage.GetStepsComponent();
        component.IsPresent.ShouldBeTrue();

        var steps = component.GetSteps();
        steps.Count.ShouldBeGreaterThan(0);
        steps
            .Select(x => x.GetTitle())
            .ShouldBeSubsetOf(["This is a fake step 1", "This is a fake step 2"]);
    }

    [Fact]
    public async Task The_school_status_table_is_not_shown_to_a_single_school()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithSchoolStatuses(
                    ACensusStatusResponse()
                        .WithName("Test School 1")
                        .WithAction(
                            "Autumn Census 2026",
                            "Not Started",
                            1,
                            2,
                            new DateTime(2026, 10, 3, 9, 0, 0, DateTimeKind.Utc)
                        )
                )
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        journeyPage.GetTableHeaders().ShouldBeEmpty();
        journeyPage.GetTableRows().ShouldBeEmpty();
        journeyPage.GetTableCellByHeader().ShouldBeEmpty();
    }

    [Fact]
    public async Task School_statuses_are_displayed_for_multi_academy_trust()
    {
        // Arrange
        var octoberFirst2026 = new DateTime(2026, 10, 1, 2, 0, 0, DateTimeKind.Utc);
        var octoberFourth2026 = new DateTime(2026, 10, 4, 9, 0, 0, DateTimeKind.Utc);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(octoberFourth2026);

        var client = CreateMatClient(services => services.AddSingleton(dateTimeProvider));

        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithSchoolStatuses(
                    ACensusStatusResponse()
                        .WithName("Test School 1")
                        .WithAction("Autumn Census 2026", "Unknown"),
                    ACensusStatusResponse()
                        .WithName("Test School 2")
                        .WithAction("Autumn Census 2026", "Submitted", 1, 2, octoberFirst2026)
                )
                .AsSuccess()
        );

        // Act
        var response = await client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var tableHeadings = journeyPage.GetTableHeaders();
        tableHeadings.ShouldSatisfyAllConditions(
            headings => headings[0].ShouldBe("Name"),
            headings => headings[1].ShouldBe("Status"),
            headings => headings[2].ShouldBe("Last Changed"),
            headings => headings[3].ShouldBe("Errors"),
            headings => headings[4].ShouldBe("Queries")
        );

        var tableRows = journeyPage.GetTableRows();
        tableRows.Count.ShouldBe(2);

        var schoolStatus = journeyPage.GetTableCellByHeader();
        schoolStatus.Count.ShouldBe(2);
        schoolStatus[0]
            .ShouldSatisfyAllConditions(
                status => status["Name"].ShouldBe("Test School 1"),
                status => status["Status"].ShouldBe("Unknown"),
                status => status["Last Changed"].ShouldBe(""),
                status => status["Errors"].ShouldBe(""),
                status => status["Queries"].ShouldBe("")
            );
        schoolStatus[1]
            .ShouldSatisfyAllConditions(
                status => status["Name"].ShouldBe("Test School 2"),
                status => status["Status"].ShouldBe("Submitted"),
                status => status["Last Changed"].ShouldBe("4 days ago"),
                status => status["Errors"].ShouldBe("1"),
                status => status["Queries"].ShouldBe("2")
            );
    }

    [Fact]
    public async Task No_schools_message_is_displayed_when_trust_has_no_schools()
    {
        // Arrange
        var client = CreateMatClient();

        _getCensusJourneyHandler.Returns(AGetCensusJourneyResponse().AsSuccess());

        // Act
        var response = await client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        journeyPage.HasTable().ShouldBeFalse();
        journeyPage
            .GetElementContentByText(
                ".govuk-inset-text p",
                "There are currently no schools to display for Test Multi Academy Trust."
            )
            .ShouldNotBeNull();

        var contactLink = journeyPage
            .GetElementByHref("a.govuk-link", "https://help.signin.education.gov.uk/contact-us")
            .ShouldNotBeNull();
        journeyPage
            .GetTextOf(contactLink)
            .ShouldBe("contact the DfE Sign In team (opens in a new tab)");
    }

    [Fact]
    public async Task Status_explanations_are_shown_when_the_census_is_available()
    {
        // Arrange
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
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        journeyPage
            .GetElementContentByText("li", "Test Status")
            .ShouldBe("Test Status: Test status description.");
    }

    [Fact]
    public async Task Status_explanations_are_hidden_when_the_census_is_unavailable()
    {
        // Arrange
        _getCensusJourneyHandler.Returns(
            AGetCensusJourneyResponse()
                .WithContent(
                    ACensusJourneyContentResponse()
                        .WithStatus(UnavailableStatus)
                        .WithUnderstandStatus(
                            AUnderstandStatus()
                                .WithName("Test Status")
                                .WithDescription("Test status description.")
                        )
                )
                .AsSuccess()
        );

        // Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        journeyPage.GetElementContentByText("li", "Test Status").ShouldBeNull();
    }

    [Fact]
    public async Task Status_explanations_are_shown_for_a_multi_academy_trust()
    {
        // Arrange
        var client = CreateMatClient();

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
        var response = await client.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<JourneyPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        journeyPage
            .GetElementContentByText("li", "Test Status")
            .ShouldBe("Test Status: Test status description.");
    }

    private HttpClient CreateMatClient(Action<IServiceCollection>? configureServices = null)
    {
        return _factory.CreateAuthorisedClient(services =>
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
                        new Claim(Organisation, matJson),
                    ],
                }
            );

            configureServices?.Invoke(services);
        });
    }
}
