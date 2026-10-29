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
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Signed_in_users_can_sign_out()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();
        var token = await GetAntiforgeryTokenAsync(client, "/", _cancellationToken);
        var requestUri = factory.GeneratePath("Account", "Logout");

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { ["__RequestVerificationToken"] = token }
        );

        // Act
        var response = await client.PostAsync(requestUri, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.OriginalString.ShouldStartWith(
            MockOidcHandler.AuthoriserRedirectUrl
        );
    }

    [Fact]
    public async Task Signed_out_users_accessing_sign_out_get_redirected_to_start_page()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient(options: ClientOptions.AllowRedirects);
        var requestUri = factory.GeneratePath("Account", "Logout");

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(requestUri, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.OriginalString.ShouldEndWith(
            factory.GeneratePath("Start", "Start")
        );
    }

    private static async Task<string> GetAntiforgeryTokenAsync(
        HttpClient client,
        string path,
        CancellationToken cancellationToken
    )
    {
        var html = await client.GetStringAsync(path, cancellationToken);
        var match = Regex.Match(
            html,
            @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)"""
        );
        match.Success.ShouldBeTrue($"No antiforgery token found on {path}");
        return match.Groups[1].Value;
    }
}
