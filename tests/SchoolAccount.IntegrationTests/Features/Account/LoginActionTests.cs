using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LoginActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Unauthorised_users_are_redirected_to_DSI()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var requestUri = factory.GeneratePath("Account", "Login");

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
        var client = factory.CreateAuthorisedClient();
        var requestUri = factory.GeneratePath("Account", "Login");

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldEndWith(
            $"{factory.GeneratePath("Journey", "Journey")}"
        );
    }

    [Fact]
    public async Task Signing_in_with_an_external_return_url_is_rejected()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();
        var requestUri = factory.GeneratePath(
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
    public async Task Signing_in_with_a_local_return_url_is_accepted()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();
        var requestUri = factory.GeneratePath(
            "Account",
            "Login",
            new { returnUrl = factory.GeneratePath("Journey", "Journey") }
        );

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
