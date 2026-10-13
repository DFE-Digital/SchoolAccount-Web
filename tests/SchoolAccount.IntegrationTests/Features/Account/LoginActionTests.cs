using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LoginActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IntegrationTest(factory)
{
    [Fact]
    public async Task Unauthorised_users_are_redirected_to_DSI()
    {
        // Arrange
        using var client = Factory.CreateUnauthenticatedClient();
        var requestUri = Factory.GeneratePath("Account", "Login");

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldStartWith(
            MockOidcHandler.AuthoriserRedirectUrl
        );
    }

    [Fact]
    public async Task Authorised_users_are_redirected_to_the_journey_page()
    {
        // Arrange
        using var client = Factory.CreateAuthorisedClient();
        var requestUri = Factory.GeneratePath("Account", "Login");

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldEndWith(
            $"{Factory.GeneratePath("Journey", "Journey")}"
        );
    }

    [Fact]
    public async Task Non_local_urls_returns_problem_response()
    {
        // Arrange
        using var client = Factory.CreateAuthorisedClient();
        var requestUri = Factory.GeneratePath(
            "Account",
            "Login",
            new { returnUrl = "https://www.google.com" }
        );

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Local_urls_returns_success_response()
    {
        // Arrange
        using var client = Factory.CreateAuthorisedClient();
        var requestUri = Factory.GeneratePath(
            "Account",
            "Login",
            new { returnUrl = Factory.GeneratePath("Dashboard", "Dashboard") }
        );

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
