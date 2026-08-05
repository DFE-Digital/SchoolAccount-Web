using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Authentication.Login;

public class LoginControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Unauthorised_users_are_redirected_to_DSI()
    {
        // Assert
        var client = factory.CreateUnauthorisedClient();

        // Act
        var response = await client.GetAsync("/login", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldStartWith(
            MockOidcHandler.AuthoriserRedirectUrl
        );
    }

    [Fact]
    public async Task Authorised_users_are_redirected_to_the_dashboard()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync("/login", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldEndWith("/dashboard");
    }

    [Fact]
    public async Task Non_local_urls_returns_problem_response()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync(
            $"/login?returnUrl={WebUtility.UrlEncode("https://www.google.com")}",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Local_urls_returns_success_response()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync(
            $"/login?returnUrl={WebUtility.UrlEncode("/dashboard")}",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
