using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LoginActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Signed_out_users_are_redirected_to_DSI()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var requestUri = factory.GeneratePath("Account", "Login");

        // Act
        var response = await client.GetAsync(requestUri, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldStartWith(MockOidcHandler.AuthoriserRedirectUrl);
    }

    [Fact]
    public async Task Signed_in_users_are_redirected_to_the_journey_page()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();
        var requestUri = factory.GeneratePath("Account", "Login");

        // Act
        var response = await client.GetAsync(requestUri, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe("/Journey");
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
        var response = await client.GetAsync(requestUri, _cancellationToken);

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
        var response = await client.GetAsync(requestUri, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
