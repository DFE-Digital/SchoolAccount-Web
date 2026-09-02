using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Builders;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Journey;

public class JourneyControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly StubCensusJourneyHandler _getCensusJourneyHandler = new();

    public JourneyControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateAuthorisedClient(services =>
            services.StubQueryHandler(_getCensusJourneyHandler)
        );
    }

    [Fact]
    public async Task Title_is_journey()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");
        _getCensusJourneyHandler.Returns(CensusJourneyResponseBuilder.Create().AsSuccess());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Journey");
    }

    [Fact]
    public async Task Call_to_action_button_displays_correctly()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = _factory.GeneratePath("Journey", "Journey");
        _getCensusJourneyHandler.Returns(CensusJourneyResponseBuilder.Create().AsSuccess());

        // Act
        var message = await _client.GetAsync(pageUri, token);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(message, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);

        var callToActionButton = page.GetButtonByLink(
            "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
        );
        callToActionButton.ShouldNotBeNull();
        callToActionButton.TextContent.Trim().ShouldStartWith("Go to Autumn Census 2026");
        callToActionButton.TextContent.Trim().ShouldEndWith("opens in new tab");
    }
}
