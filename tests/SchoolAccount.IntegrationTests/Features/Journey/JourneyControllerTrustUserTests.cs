using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.IntegrationTests.Common.Scenarios;
using Shouldly;
using static SchoolAccount.TestCommon.Builders.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyContentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseUnderstandStatusBuilder;

namespace SchoolAccount.IntegrationTests.Features.Journey;

/// <summary>
/// Journey page tests for a user from a multi-academy trust. They use their own scenario host,
/// so they live in their own class.
/// </summary>
public class JourneyControllerTrustUserTests(TrustUserHost host) : IClassFixture<TrustUserHost>
{
    private readonly HttpClient _client = host.CreateTestClient();

    [Fact]
    public async Task School_statuses_are_displayed_for_multi_academy_trust()
    {
        // Arrange
        var octoberFirst2026 = new DateTime(2026, 10, 1, 2, 0, 0, DateTimeKind.Utc);
        var token = TestContext.Current.CancellationToken;
        var pageUri = host.GeneratePath("Journey", "Journey");

        host.CensusJourney.Returns(
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
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var tableHeadings = page.GetTableHeaders();
        tableHeadings.ShouldNotBeNull();
        tableHeadings[0].ShouldBe("Name");
        tableHeadings[1].ShouldBe("Status");
        tableHeadings[2].ShouldBe("Last Changed");
        tableHeadings[3].ShouldBe("Errors");
        tableHeadings[4].ShouldBe("Queries");

        var tableRows = page.GetTableRows();
        tableRows.ShouldNotBeNull();
        tableRows.Count.ShouldBe(2);

        var schoolStatus = page.GetTableCellByHeader();
        schoolStatus.ShouldNotBeNull();
        schoolStatus.Count.ShouldBe(2);
        schoolStatus[0]
            .ShouldSatisfyAllConditions(
                s => s["Name"].ShouldBe("Test School 1"),
                s => s["Status"].ShouldBe("Unknown"),
                s => s["Last Changed"].ShouldBe(""),
                s => s["Errors"].ShouldBe(""),
                s => s["Queries"].ShouldBe("")
            );
        schoolStatus[1]
            .ShouldSatisfyAllConditions(
                s => s["Name"].ShouldBe("Test School 2"),
                s => s["Status"].ShouldBe("Submitted"),
                s => s["Last Changed"].ShouldBe("4 days ago"),
                s => s["Errors"].ShouldBe("1"),
                s => s["Queries"].ShouldBe("2")
            );
    }

    [Fact]
    public async Task Understand_statuses_list_is_displayed_for_multi_academy_trust()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = host.GeneratePath("Journey", "Journey");

        host.CensusJourney.Returns(
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
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<JourneyPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.GetComponentByContent("li", "Test Status")
            .ShouldBe("Test Status: Test status description.");
    }
}
