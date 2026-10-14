using System.Net;
using System.Text.RegularExpressions;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public static class LogoutActionTests
{
    public class SignedIn(AuthorisedWebApplicationFactory factory)
        : IClassFixture<AuthorisedWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClientWithDefaults();

        [Fact]
        public async Task Authorised_users_can_sign_out()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var token = await GetAntiforgeryTokenAsync(_client, "/", cancellationToken);
            var requestUri = factory.GeneratePath("Account", "Logout");

            using var content = new FormUrlEncodedContent(
                new Dictionary<string, string> { ["__RequestVerificationToken"] = token }
            );

            // Act
            var response = await _client.PostAsync(requestUri, content, cancellationToken);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Headers.Location?.OriginalString.ShouldStartWith(
                MockOidcHandler.AuthoriserRedirectUrl
            );
        }

        [Fact]
        public async Task Authenticated_pages_display_the_sign_out_link_for_authorised_users()
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

        private static async Task<string> GetAntiforgeryTokenAsync(
            HttpClient client,
            string path,
            CancellationToken ct
        )
        {
            var html = await client.GetStringAsync(path, ct);
            var match = Regex.Match(
                html,
                @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)"""
            );
            match.Success.ShouldBeTrue($"No antiforgery token found on {path}");
            return match.Groups[1].Value;
        }
    }

    public class SignedOut(UnauthenticatedWebApplicationFactory factory)
        : IClassFixture<UnauthenticatedWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClientWithDefaults(
            ClientOptions.AllowRedirects
        );

        [Fact]
        public async Task Unauthorised_users_accessing_sign_out_get_redirected_to_start_page()
        {
            // Arrange
            var requestUri = factory.GeneratePath("Account", "Logout");

            // Act
            using var content = new StringContent(string.Empty);
            var response = await _client.PostAsync(
                requestUri,
                content,
                TestContext.Current.CancellationToken
            );

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Headers.Location?.OriginalString.ShouldEndWith(
                factory.GeneratePath("Start", "Start")
            );
        }
    }
}
