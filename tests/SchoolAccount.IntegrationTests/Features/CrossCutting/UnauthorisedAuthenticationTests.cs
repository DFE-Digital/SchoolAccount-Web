using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class UnauthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateUnauthorisedClient();

    [Theory]
    [InlineData("/dashboard")]
    public async Task Ensure_that_the_controller_redirects_for_unauthorised_users(string endpoint)
    {
        // Act
        var response = await _client.GetAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldStartWith(
            MockOidcHandler.AuthoriserRedirectUrl
        );
    }

    [Fact]
    public async Task Ensure_that_the_home_controller_allows_unauthorised_users()
    {
        // Act
        var response = await _client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
