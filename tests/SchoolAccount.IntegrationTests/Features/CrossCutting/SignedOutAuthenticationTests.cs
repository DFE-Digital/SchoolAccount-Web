using NSubstitute;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.IntegrationTests.Common.Stubs;
using SchoolAccount.TestCommon.Constants;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.IntegrationTests.Common.Builders.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyResponseBuilder;
using static SchoolAccount.IntegrationTests.Common.MockOidcHandler;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class SignedOutAuthenticationTests
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly HttpClient _client;

    public SignedOutAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateUnauthenticatedClient();
    }

    [Fact]
    public async Task Signed_out_users_cannot_access_the_journey_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync(JourneyUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldBeEquivalentTo(JourneyReturnUrl);
    }

    [Fact]
    public async Task Signed_out_users_can_see_the_start_page()
    {
        // Arrange & Act
        var response = await _client.GetAsync(StartUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(Redirect);
        response.StatusCode.ShouldBe(OK);
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_the_sign_out_link()
    {
        // Arrange & Act
        var response = await _client.GetAsync(StartUrl, _cancellationToken);
        var startPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        startPage.GetElementContainingText("button", "Sign out").ShouldBeNull();
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_an_organisation_name()
    {
        // Arrange & Act
        var response = await _client.GetAsync(StartUrl, _cancellationToken);
        var startPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        var organisationName = startPage.GetOrganisationName();
        organisationName.ShouldBeNull();
    }

    [Fact]
    public async Task Signed_out_users_are_redirected_to_the_start_page_while_logged_out()
    {
        // Arrange & Act
        var response = await _client.GetAsync(LoggedOutUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }

    [Fact]
    public async Task Signed_out_users_are_redirected_to_DSI()
    {
        // Arrange & Act
        var response = await _client.GetAsync(LoginUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldStartWith(AuthoriserRedirectUrl);
    }

    [Fact]
    public async Task Signed_out_users_accessing_sign_out_get_redirected_to_start_page()
    {
        // Arrange & Act
        using var content = new StringContent(string.Empty);
        var response = await _client.PostAsync(LogoutUrl, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }
}
