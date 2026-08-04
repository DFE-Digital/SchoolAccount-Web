using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Authentication.Login;

public class LoginControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _authorisedClient = factory.CreateAuthorisedClient();
    private readonly HttpClient _unauthorisedClient = factory.CreateUnauthorisedClient();

    [Fact]
    public async Task Unauthorised_users_are_redirected_to_DSI()
    {
        // Act
        var response = await _unauthorisedClient.GetAsync(
            "/login",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldStartWith("https://test-oidc.signin");
    }

    [Fact]
    public async Task Authorised_users_are_redirected_to_the_dashboard()
    {
        // Act
        var response = await _authorisedClient.GetAsync(
            "/login",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldEndWith("/dashboard");
    }
}
