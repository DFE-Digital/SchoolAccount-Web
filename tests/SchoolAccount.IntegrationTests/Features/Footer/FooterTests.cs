using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Footer;

public class FooterTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateAuthorisedClient(services =>
        services.StubQueryHandler(StubCensusStatusesHandler.Succeeding())
    );

    [Theory]
    [InlineData("Journey", "Journey")]
    [InlineData("Start", "Start")]
    public async Task Footer_is_displayed_on_pages_across_the_service(
        string controller,
        string action
    )
    {
        // Arrange
        var pageUri = factory.GeneratePath(controller, action);

        // Act
        var response = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);

        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();
        page.GetFooter().ShouldNotBeNull();
    }

    [Theory]
    [InlineData("#", "Accessibility statement")]
    [InlineData(
        "https://www.gov.uk/government/publications/privacy-information-education-providers-workforce-including-teachers/privacy-information-education-providers-workforce-including-teachers",
        "Privacy notice"
    )]
    public async Task Footer_displays_the_expected_links(string expectedHref, string expectedText)
    {
        // Arrange
        var pageUri = factory.GeneratePath("Journey", "Journey");

        // Act
        var response = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);

        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var footerLink = page.GetFooterLink(expectedHref);

        footerLink.ShouldNotBeNull();
        footerLink.TextContent.ShouldContainWithoutWhitespace(expectedText);
    }
}
