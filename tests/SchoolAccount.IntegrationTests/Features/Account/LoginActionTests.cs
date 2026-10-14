using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public static class LoginActionTests
{
    public class SignedOut(UnauthenticatedWebApplicationFactory factory)
        : IClassFixture<UnauthenticatedWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClientWithDefaults();

        [Fact]
        public async Task Unauthorised_users_are_redirected_to_DSI()
        {
            // Arrange
            var requestUri = factory.GeneratePath("Account", "Login");

            // Act
            var response = await _client.GetAsync(
                requestUri,
                TestContext.Current.CancellationToken
            );

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
            response.Headers.Location?.OriginalString.ShouldStartWith(
                MockOidcHandler.AuthoriserRedirectUrl
            );
        }
    }

    public class SignedIn(AuthorisedWebApplicationFactory factory)
        : IClassFixture<AuthorisedWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClientWithDefaults();

        [Fact]
        public async Task Authorised_users_are_redirected_to_the_journey_page()
        {
            // Arrange
            var requestUri = factory.GeneratePath("Account", "Login");

            // Act
            var response = await _client.GetAsync(
                requestUri,
                TestContext.Current.CancellationToken
            );

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
            response.Headers.Location?.OriginalString.ShouldEndWith(
                $"{factory.GeneratePath("Journey", "Journey")}"
            );
        }

        [Fact]
        public async Task Non_local_urls_returns_problem_response()
        {
            // Arrange
            var requestUri = factory.GeneratePath(
                "Account",
                "Login",
                new { returnUrl = "https://www.google.com" }
            );

            // Act
            var response = await _client.GetAsync(
                requestUri,
                TestContext.Current.CancellationToken
            );

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Local_urls_returns_success_response()
        {
            // Arrange
            var requestUri = factory.GeneratePath(
                "Account",
                "Login",
                new { returnUrl = factory.GeneratePath("Dashboard", "Dashboard") }
            );

            // Act
            var response = await _client.GetAsync(
                requestUri,
                TestContext.Current.CancellationToken
            );

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Found);
        }
    }
}
