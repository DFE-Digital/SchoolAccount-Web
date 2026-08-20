using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AuthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateAuthorisedClient();

    [Theory]
    [InlineData("/")]
    [InlineData("Dashboard")]
    public async Task Authorised_users_can_access_a_page(string path)
    {
        // Arrange & Act
        var response = await _client.GetAsync(path, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Authenticated_pages_display_the_sign_out_for_authorised_users()
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

        var pageSignOutLink = page.GetSignOutLink();
        pageSignOutLink.ShouldNotBeNull();
        pageSignOutLink.ShouldContainWithoutWhitespace("Sign out");
    }

    [Fact]
    public async Task Authenticated_pages_display_the_organisation_name_link_for_authorised_users()
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

        var pageOrganisationName = page.GetOrganisationName();
        pageOrganisationName.ShouldNotBeNull();
        pageOrganisationName.ShouldContainWithoutWhitespace(MockAuthHandler.FakeOrganisationName);
    }
}
