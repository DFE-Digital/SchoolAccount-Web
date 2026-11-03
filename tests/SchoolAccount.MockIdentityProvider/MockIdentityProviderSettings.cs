namespace SchoolAccount.MockIdentityProvider;

internal sealed record MockIdentityProviderSettings
{
    public const string SectionName = "MockIdentityProvider";

    /// <summary>
    /// The address the web app uses to reach this provider. It goes in the
    /// <c>iss</c> claim, so it must match the web app's configured Authority.
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// The address the browser uses to reach this provider. Inside Docker this
    /// differs from <see cref="Issuer"/>, which is a container hostname.
    /// </summary>
    public required string PublicUrl { get; init; }

    public required string ClientId { get; init; }

    public required string[] AllowedRedirectUris { get; init; }

    /// <summary>
    /// JSON file listing the users that can sign in, one per test academy.
    /// </summary>
    public required string UsersFile { get; init; }
}
