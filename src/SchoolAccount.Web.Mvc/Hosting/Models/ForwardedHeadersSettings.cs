namespace SchoolAccount.Web.Mvc.Hosting.Models;

public class ForwardedHeadersSettings
{
    public const string SectionName = "ForwardedHeadersSettings";

    /// <summary>
    /// When true, the X-Forwarded-For/X-Forwarded-Proto headers set by the reverse proxy
    /// (Caddy, Azure Container Apps ingress, etc.) are trusted from any source. Use this
    /// when the proxy's address isn't known ahead of time and the network topology already
    /// prevents anything else from reaching the app directly (e.g. a non-VNET Container
    /// Apps environment).
    /// </summary>
    public bool TrustAllNetworks { get; init; } = true;

    /// <summary>
    /// CIDR ranges (e.g. "10.226.168.64/26") to trust when <see cref="TrustAllNetworks"/> is
    /// false. Use this when the app is reachable from a known, stable network - such as a
    /// VNET-integrated Container Apps environment's delegated infrastructure subnet - and you
    /// want to restrict which proxies can set forwarded headers rather than trusting all.
    /// </summary>
    public string[] TrustedNetworks { get; init; } = [];
}
