namespace SchoolAccount.Web.Mvc.Hosting.Models;

public class ForwardedHeadersSettings
{
    public const string SectionName = "ForwardedHeadersSettings";

    /// <summary>
    /// Accept forwarded headers from any source, for environments where the proxy address is
    /// not known in advance.
    /// </summary>
    public bool TrustAllNetworks { get; init; } = true;

    /// <summary>
    /// CIDR ranges to accept forwarded headers from, e.g. "10.226.168.64/26". Ignored when
    /// <see cref="TrustAllNetworks"/> is true.
    /// </summary>
    public string[] TrustedNetworks { get; init; } = [];
}
