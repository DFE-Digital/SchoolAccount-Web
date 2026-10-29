using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LoggedOutActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Ensure_an_authenticated_user_accessing_LoggedOut_are_redirected_to_the_start_page()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();
        var requestUri = UrlConstants.LoggedOutUrl;

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(UrlConstants.BaseUrl);
    }

    [Fact]
    public async Task Ensure_an_unauthenticated_user_accessing_LoggedOut_are_redirected_to_the_start_page()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var requestUri = UrlConstants.LoggedOutUrl;

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(UrlConstants.BaseUrl);
    }
}
