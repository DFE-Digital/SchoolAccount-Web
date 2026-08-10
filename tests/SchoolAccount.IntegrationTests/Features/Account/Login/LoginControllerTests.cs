using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc;
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

        // Act
        var response = await client.GetAsync(RouteConstants.Account.Login, TestContext.Current.CancellationToken);

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
        var response = await client.GetAsync(RouteConstants.Account.Login, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldEndWith($"/{RouteConstants.Dashboard}");
    }

    [Fact]
    public async Task Non_local_urls_returns_problem_response()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync(
            $"/{RouteConstants.Account.Login}?returnUrl={WebUtility.UrlEncode("https://www.google.com")}",
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
            $"/{RouteConstants.Account.Login}?returnUrl={WebUtility.UrlEncode($"/{RouteConstants.Dashboard}")}",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
