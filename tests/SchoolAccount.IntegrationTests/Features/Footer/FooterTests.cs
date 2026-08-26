using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.IntegrationTests.Common.Stubs;
using SchoolAccount.Web.Mvc.Features.Shared;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Footer;

public class FooterTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateAuthorisedClient(services =>
        services.StubQueryHandler(StubCensusStatusesHandler.Succeeding())
    );

    [Theory]
    [InlineData(FooterUrls.PrivacyNotice, "Privacy notice")]
    [InlineData(FooterUrls.OpenGovernmentLicence, "Open Government Licence v3.0")]
    [InlineData(FooterUrls.CrownCopyright, "© Crown copyright")]
    public async Task Footer_displays_the_expected_links(string expectedHref, string expectedText)
    {
        // Arrange
        var pageUri = factory.GeneratePath("Dashboard", "Dashboard");

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

    [Fact]
    public async Task Page_displays_the_footer()
    {
        // Arrange
        var pageUri = factory.GeneratePath("Dashboard", "Dashboard");

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
    [InlineData("Dashboard", "Dashboard")]
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
}
