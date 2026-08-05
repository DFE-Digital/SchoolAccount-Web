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
        var response = await client.GetAsync(
            "/login",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldStartWith(MockOidcHandler.AuthoriserRedirectUrl);
    }

    [Fact]
    public async Task Authorised_users_are_redirected_to_the_dashboard()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();
        
        // Act
        var response = await client.GetAsync(
            "/login",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldEndWith("/dashboard");
    }
}
