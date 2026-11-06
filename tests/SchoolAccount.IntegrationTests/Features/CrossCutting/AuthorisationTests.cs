using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.IntegrationTests.Common.MockAuthHandler;
using static SchoolAccount.IntegrationTests.Common.MockOidcHandler;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AuthorisationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    private readonly HttpClient _authorisedClient = factory.CreateAuthorisedClient(services =>
        services.StubQueryHandler(StubCensusJourneyHandler.Succeeding())
    );
    private readonly HttpClient _unauthorisedClient = factory.CreateUnauthenticatedClient();

    [Theory]
    [InlineData(StartUrl)]
    [InlineData(JourneyUrl)]
    public async Task Signed_in_users_can_access_a_page(string path)
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(path, _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(Redirect);
        response.StatusCode.ShouldBe(OK);
    }

    [Fact]
    public async Task Signed_in_users_see_the_sign_out_link()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);
        journeyPage.GetElementContainingText("button", "Sign out").ShouldNotBeNull();
    }

    [Fact]
    public async Task Signed_in_users_see_their_organisation_name()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(JourneyUrl, _cancellationToken);
        var journeyPage = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(OK);

        var organisationName = journeyPage.GetOrganisationName();
        organisationName.ShouldNotBeNull();
        organisationName.ShouldContainWithoutWhitespace(FakeOrganisationName);
    }

    [Fact]
    public async Task Signed_out_users_cannot_access_the_journey_page()
    {
        // Arrange & Act
        var response = await _unauthorisedClient.GetAsync(JourneyUrl, _cancellationToken);

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
        var response = await _unauthorisedClient.GetAsync(StartUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(Redirect);
        response.StatusCode.ShouldBe(OK);
    }

    [Fact]
    public async Task Signed_out_users_do_not_see_the_sign_out_link()
    {
        // Arrange & Act
        var response = await _unauthorisedClient.GetAsync(StartUrl, _cancellationToken);
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
        var response = await _unauthorisedClient.GetAsync(StartUrl, _cancellationToken);
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
    public async Task Signed_in_users_are_redirected_to_the_start_page_when_logged_out()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(LoggedOutUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }

    [Fact]
    public async Task Signed_out_users_are_redirected_to_the_start_page_while_logged_out()
    {
        // Arrange & Act
        var response = await _unauthorisedClient.GetAsync(LoggedOutUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }

    [Fact]
    public async Task Signed_out_users_are_redirected_to_DSI()
    {
        // Arrange & Act
        var response = await _unauthorisedClient.GetAsync(LoginUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldStartWith(AuthoriserRedirectUrl);
    }

    [Fact]
    public async Task Signed_in_users_are_redirected_to_the_journey_page()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(LoginUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(JourneyUrl);
    }

    [Fact]
    public async Task Signing_in_with_an_external_return_url_is_rejected()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(
            LoginWithExternalReturnUrl,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(BadRequest);
    }

    [Fact]
    public async Task Signing_in_with_a_local_return_url_is_accepted()
    {
        // Arrange & Act
        var response = await _authorisedClient.GetAsync(
            LoginWithJourneyReturnUrl,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(Found);
    }

    [Fact]
    public async Task Signed_in_users_can_sign_out()
    {
        // Arrange & Act
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>());
        var response = await _authorisedClient.PostAsync(LogoutUrl, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);
    }

    [Fact]
    public async Task Signed_out_users_accessing_sign_out_get_redirected_to_start_page()
    {
        // Arrange & Act
        using var content = new StringContent(string.Empty);
        var response = await _unauthorisedClient.PostAsync(LogoutUrl, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
    }
}
