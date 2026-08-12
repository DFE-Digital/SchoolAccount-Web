using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc.Features.Accounts;
using SchoolAccount.Web.Mvc.Features.Dashboard;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LoginActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Unauthorised_users_are_redirected_to_DSI()
    {
        // Arrange
        var client = factory.CreateUnauthorisedClient();
        var requestUri = UrlBuilder.GeneratePath<AccountController>(
            nameof(AccountController.Login)
        );

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(
            requestUri,
            content,
            TestContext.Current.CancellationToken
        );

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
        var requestUri = UrlBuilder.GeneratePath<AccountController>(
            nameof(AccountController.Login)
        );

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(
            requestUri,
            content,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldEndWith(
            $"{UrlBuilder.GeneratePath<DashboardController>(nameof(DashboardController.Dashboard))}"
        );
    }

    [Fact]
    public async Task Non_local_urls_returns_problem_response()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();
        var requestUri = UrlBuilder.GeneratePath<AccountController>(
            nameof(AccountController.Login),
            new { returnUrl = "https://www.google.com" }
        );

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(
            requestUri,
            content,
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
        var requestUri = UrlBuilder.GeneratePath<AccountController>(
            nameof(AccountController.Login),
            new { returnUrl = UrlBuilder.GeneratePath(nameof(DashboardController.Dashboard)) }
        );

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(
            requestUri,
            content,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
