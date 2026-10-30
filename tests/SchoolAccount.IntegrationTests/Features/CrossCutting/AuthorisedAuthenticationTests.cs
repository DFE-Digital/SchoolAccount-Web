using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AuthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    private readonly HttpClient _client = factory.CreateAuthorisedClient(services =>
        services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
    );

    [Theory]
    [InlineData("/")]
    [InlineData("Journey")]
    public async Task Signed_in_users_can_access_a_page(string path)
    {
        // Arrange & Act
        var response = await _client.GetAsync(path, _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Signed_in_users_see_the_sign_out_link()
    {
        // Arrange
        var pageUri = factory.GeneratePath("Journey", "Journey");

        // Act
        var response = await _client.GetAsync(pageUri, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.GetElementContainingText("button", "Sign out").ShouldNotBeNull();
    }

    [Fact]
    public async Task Signed_in_users_see_their_organisation_name()
    {
        // Arrange
        var pageUri = factory.GeneratePath("Journey", "Journey");

        // Act
        var response = await _client.GetAsync(pageUri, _cancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(response, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pageOrganisationName = page.GetOrganisationName();
        pageOrganisationName.ShouldNotBeNull();
        pageOrganisationName.ShouldContainWithoutWhitespace(MockAuthHandler.FakeOrganisationName);
    }
}
