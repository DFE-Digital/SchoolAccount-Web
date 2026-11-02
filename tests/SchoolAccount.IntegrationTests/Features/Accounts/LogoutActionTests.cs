using SchoolAccount.IntegrationTests.Common;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.Accounts;

public class LogoutActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Signed_in_users_can_sign_out()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>());

        // Act
        var response = await client.PostAsync(LogoutUrl, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);
    }

    [Fact]
    public async Task Signed_out_users_accessing_sign_out_get_redirected_to_start_page()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();

        // Act
        using var content = new StringContent(string.Empty);
        var response = await client.PostAsync(LogoutUrl, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }
}
