using Microsoft.Net.Http.Headers;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;
using SameSiteMode = Microsoft.Net.Http.Headers.SameSiteMode;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

// The cookie name is agreed with the firewall team, so it is deliberately repeated here
// rather than shared with the middleware: renaming it should fail these tests
public class TrackingCookieTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private const string _cookieName = "SchoolAccountSessionId";
    private const string _staticAssetPath = "/assets/images/card-1.jpg";

    [Fact]
    public async Task A_first_visit_to_a_page_is_given_a_tracking_cookie_holding_a_guid()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var pageUri = factory.GeneratePath("Start", "Start");

        // Act
        var response = await client.GetAsync(pageUri, TestContext.Current.CancellationToken);

        // Assert
        var trackingCookie = GetTrackingCookie(response);
        trackingCookie.ShouldNotBeNull();
        Guid.TryParseExact(trackingCookie.Value.Value, "D", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task The_tracking_cookie_is_http_only_secure_and_sent_cross_site()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var pageUri = factory.GeneratePath("Start", "Start");

        // Act
        var response = await client.GetAsync(pageUri, TestContext.Current.CancellationToken);

        // Assert
        var trackingCookie = GetTrackingCookie(response);
        trackingCookie.ShouldNotBeNull();
        trackingCookie.HttpOnly.ShouldBeTrue();
        trackingCookie.Secure.ShouldBeTrue();
        trackingCookie.SameSite.ShouldBe(SameSiteMode.None);
    }

    [Fact]
    public async Task The_tracking_cookie_lasts_for_the_browser_session()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var pageUri = factory.GeneratePath("Start", "Start");

        // Act
        var response = await client.GetAsync(pageUri, TestContext.Current.CancellationToken);

        // Assert
        var trackingCookie = GetTrackingCookie(response);
        trackingCookie.ShouldNotBeNull();
        trackingCookie.Expires.ShouldBeNull();
        trackingCookie.MaxAge.ShouldBeNull();
    }

    [Fact]
    public async Task A_browser_that_already_has_a_tracking_cookie_keeps_it()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var pageUri = factory.GeneratePath("Start", "Start");
        var existingIdentifier = Guid.NewGuid();

        // Act
        var response = await GetWithTrackingCookieAsync(
            client,
            pageUri,
            existingIdentifier.ToString()
        );

        // Assert
        GetTrackingCookie(response).ShouldBeNull();
    }

    [Fact]
    public async Task A_tracking_cookie_that_is_not_a_guid_is_replaced()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();
        var pageUri = factory.GeneratePath("Start", "Start");
        const string notAGuid = "not-a-guid";

        // Act
        var response = await GetWithTrackingCookieAsync(client, pageUri, notAGuid);

        // Assert
        var trackingCookie = GetTrackingCookie(response);
        trackingCookie.ShouldNotBeNull();
        Guid.TryParseExact(trackingCookie.Value.Value, "D", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task Static_assets_are_not_given_a_tracking_cookie()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();

        // Act
        var response = await client.GetAsync(
            _staticAssetPath,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        GetTrackingCookie(response).ShouldBeNull();
    }

    private static async Task<HttpResponseMessage> GetWithTrackingCookieAsync(
        HttpClient client,
        string uri,
        string cookieValue
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Add(HeaderNames.Cookie, $"{_cookieName}={cookieValue}");

        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private static SetCookieHeaderValue? GetTrackingCookie(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues(HeaderNames.SetCookie, out var setCookieValues))
        {
            return null;
        }

        return SetCookieHeaderValue
            .ParseList([.. setCookieValues])
            .SingleOrDefault(cookie => cookie.Name.Equals(_cookieName, StringComparison.Ordinal));
    }
}
