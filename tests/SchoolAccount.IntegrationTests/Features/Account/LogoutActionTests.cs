using System.Net;
using System.Text.RegularExpressions;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LogoutActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Authorised_users_can_sign_out()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var client = factory.CreateAuthorisedClient();
        var requestUri = factory.GeneratePath("Account", "Logout");

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>());

        // Act
        var response = await client.PostAsync(requestUri, content, cancellationToken);

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
        var client = factory.CreateUnauthenticatedClient(options: ClientOptions.AllowRedirects);
        var requestUri = factory.GeneratePath("Account", "Logout");

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
            factory.GeneratePath("Start", "Start")
        );
    }
}
