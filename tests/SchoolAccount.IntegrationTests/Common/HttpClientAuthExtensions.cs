using System.Security.Claims;
using System.Text.Json;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Sets which user an authorised client signs in as. The claims travel on every request the
/// client makes, so they never leak into other clients or other tests.
/// </summary>
public static class HttpClientAuthExtensions
{
    private static readonly JsonSerializerOptions OrganisationJsonOptions = new(
        JsonSerializerDefaults.Web
    );

    /// <summary>
    /// Signs in as exactly these claims, replacing the default test user.
    /// </summary>
    public static HttpClient WithClaims(this HttpClient client, IEnumerable<Claim> claims)
    {
        // Serialised JSON escapes newlines and non-ASCII, so it's safe as a header value.
        var claimsJson = JsonSerializer.Serialize(
            claims.ToDictionary(claim => claim.Type, claim => claim.Value)
        );
        client.DefaultRequestHeaders.Add(MockAuthHandler.ClaimsHeader, claimsJson);

        return client;
    }

    /// <summary>
    /// Signs in as the default test user, belonging to a different organisation.
    /// </summary>
    public static HttpClient WithOrganisation(this HttpClient client, Organisation organisation)
    {
        return client.WithRawOrganisationClaim(
            JsonSerializer.Serialize(organisation, OrganisationJsonOptions)
        );
    }

    /// <summary>
    /// Signs in as the default test user with this exact organisation claim. Only for tests about
    /// claims the app can't read, such as empty or malformed JSON; use
    /// <see cref="WithOrganisation"/> otherwise.
    /// </summary>
    public static HttpClient WithRawOrganisationClaim(
        this HttpClient client,
        string organisationJson
    )
    {
        return client.WithClaims(MockAuthHandler.DefaultClaims(organisationJson));
    }
}
