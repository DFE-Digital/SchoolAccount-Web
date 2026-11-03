using SchoolAccount.IntegrationTests.Common;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;
using static Xunit.TestContext;

namespace SchoolAccount.IntegrationTests.Features.Accounts;

public class LoggedOutActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Ensure_an_authenticated_user_accessing_LoggedOut_are_redirected_to_the_start_page()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync(LoggedOutUrl, Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }

    [Fact]
    public async Task Ensure_an_unauthenticated_user_accessing_LoggedOut_are_redirected_to_the_start_page()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();

        // Act
        var response = await client.GetAsync(LoggedOutUrl, Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }
}
