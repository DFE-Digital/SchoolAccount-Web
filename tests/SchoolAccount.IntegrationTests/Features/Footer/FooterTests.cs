using System.Net;
using AngleSharp.Html.Dom;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Constants;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Footer;

public class FooterTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateAuthorisedClient(services =>
        services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
    );

    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Theory]
    [InlineData(UrlConstants.JourneyUrl)]
    [InlineData(UrlConstants.BaseUrl)]
    public async Task Footer_is_displayed_on_pages_across_the_service(string pageUri)
    {
        // Arrange & Act
        var response = await _client.GetAsync(pageUri, _cancellationToken);

        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();
        page.GetFooter().ShouldNotBeNull();
    }

    [Fact]
    public async Task Footer_includes_an_accessibility_statement()
    {
        // Arrange
        var pageUri = UrlConstants.JourneyUrl;

        // Act
        var response = await _client.GetAsync(pageUri, _cancellationToken);

        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var accessibilityLink =
            page.GetComponentByContent(".govuk-footer__link", "Accessibility statement")
            as IHtmlAnchorElement;

        accessibilityLink.ShouldNotBeNull();

        accessibilityLink.Href.ShouldBe(
            "https://accessibility-statements.education.gov.uk/statement/12"
        );
    }

    [Fact]
    public async Task Footer_includes_a_privacy_notice()
    {
        // Arrange
        var pageUri = UrlConstants.JourneyUrl;

        // Act
        var response = await _client.GetAsync(pageUri, _cancellationToken);

        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var privacyNoticeLink =
            page.GetComponentByContent(".govuk-footer__link", "Privacy notice")
            as IHtmlAnchorElement;

        privacyNoticeLink.ShouldNotBeNull();

        privacyNoticeLink.Href.ShouldBe(
            "https://www.gov.uk/government/publications/privacy-information-education-providers-workforce-including-teachers/privacy-information-education-providers-workforce-including-teachers"
        );
    }
}
