using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc.Features.Accounts;
using SchoolAccount.Web.Mvc.Features.Start;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LogoutActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Authorised_users_can_sign_out()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient(options: ClientOptions.AllowRedirects);
        var requestUri = factory.GeneratePath<AccountController>(nameof(AccountController.Logout));

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(
            requestUri,
            content,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.OriginalString.ShouldStartWith(
            MockOidcHandler.AuthoriserRedirectUrl
        );
    }

    [Fact]
    public async Task Unauthorised_users_accessing_sign_out_get_redirected_to_start_page()
    {
        // Arrange
        var client = factory.CreateUnauthorisedClient(options: ClientOptions.AllowRedirects);
        var requestUri = factory.GeneratePath<AccountController>(nameof(AccountController.Logout));

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(
            requestUri,
            content,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.OriginalString.ShouldEndWith(
            factory.GeneratePath<StartController>(nameof(StartController.Start))
        );
    }
}
