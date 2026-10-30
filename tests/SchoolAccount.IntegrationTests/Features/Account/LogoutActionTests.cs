using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.TestCommon.Constants;
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
        var requestUri = UrlConstants.LogoutUrl;

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>());

        // Act
        var response = await client.PostAsync(requestUri, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Signed_out_users_accessing_sign_out_get_redirected_to_start_page()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var requestUri = UrlConstants.LogoutUrl;

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(requestUri, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(UrlConstants.BaseUrl);
    }
}
