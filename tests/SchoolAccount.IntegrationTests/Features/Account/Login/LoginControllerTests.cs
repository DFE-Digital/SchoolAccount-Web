using System.Net;
using Microsoft.AspNetCore.Mvc.Routing;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc;
using SchoolAccount.Web.Mvc.Features.Accounts;
using SchoolAccount.Web.Mvc.Features.Dashboard;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account.Login;

public class LoginControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Unauthorised_users_are_redirected_to_DSI()
    {
        // Assert
        var client = factory.CreateUnauthorisedClient();
        var requestUri = UrlBuilder.GeneratePath<AccountController>(
            nameof(AccountController.SignIn)
        );

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

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
            nameof(AccountController.SignIn)
        );

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

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
            nameof(AccountController.SignIn),
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
        var client = factory.CreateAuthorisedClient();
        var requestUri = UrlBuilder.GeneratePath<AccountController>(
            nameof(AccountController.SignIn),
            new { returnUrl = UrlBuilder.GeneratePath(nameof(DashboardController.Dashboard)) }
        );

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
