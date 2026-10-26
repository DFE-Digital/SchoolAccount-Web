using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class UnauthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthenticatedClient();

    [Fact]
    public async Task Users_that_are_not_signed_in_cannot_access_the_journey_page()
    {
        // Arrange
        var journeyUri = factory.GeneratePath("Journey", "Journey");

        // Act
        var response = await _client.GetAsync(journeyUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldBeEquivalentTo(
            factory.GeneratePath("Start", "Start", new { ReturnUrl = journeyUri })
        );
    }

    [Fact]
    public async Task Signed_out_users_can_see_the_start_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unauthenticated_pages_do_not_display_the_organisation_name_for_authorised_users()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var pageSignOutLink = page.GetSignOutLink();
        pageSignOutLink.ShouldBeNull();
    }

    [Fact]
    public async Task Unauthenticated_pages_do_not_display_the_sign_out_link_for_authorised_users()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var pageOrganisationName = page.GetOrganisationName();
        pageOrganisationName.ShouldBeNull();
    }
}
