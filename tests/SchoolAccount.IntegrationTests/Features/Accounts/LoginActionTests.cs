using SchoolAccount.IntegrationTests.Common;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.IntegrationTests.Common.MockOidcHandler;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LoginActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Signed_out_users_are_redirected_to_DSI()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();

        // Act
        var response = await client.GetAsync(LoginUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldStartWith(AuthoriserRedirectUrl);
    }

    [Fact]
    public async Task Signed_in_users_are_redirected_to_the_journey_page()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync(LoginUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(JourneyUrl);
    }

    [Fact]
    public async Task Signing_in_with_an_external_return_url_is_rejected()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync(LoginWithExternalReturnUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(BadRequest);
    }

    [Fact]
    public async Task Signing_in_with_a_local_return_url_is_accepted()
    {
        // Arrange
        var client = factory.CreateAuthorisedClient();

        // Act
        var response = await client.GetAsync(LoginWithJourneyReturnUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Found);
    }
}
