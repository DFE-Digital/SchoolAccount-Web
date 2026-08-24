using Microsoft.AspNetCore.HttpOverrides;
using SchoolAccount.Web.Mvc.Hosting.Models;
using static SchoolAccount.Web.Mvc.Hosting.Models.ForwardedHeadersSettings;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures ForwardedHeadersMiddleware so ASP.NET Core trusts the X-Forwarded-For/
    /// X-Forwarded-Proto headers set by the reverse proxy in front of the app.
    /// Without this, the app sees every request as plain HTTP, since the proxy terminates TLS
    /// and forwards over HTTP internally - which breaks anything that builds an absolute URL
    /// from the request scheme (e.g. login redirects, OIDC redirect URIs).
    ///
    /// Controlled via <see cref="ForwardedHeadersSettings"/> so trust can be scoped per
    /// environment: trust-all where the proxy's address isn't known ahead of time and the
    /// network topology already blocks direct access (e.g. a non-VNET Container Apps
    /// environment), or restricted to specific CIDR ranges where it is known (e.g. a
    /// VNET-integrated environment's delegated infrastructure subnet).
    /// </summary>
    public static void UseConfiguredForwardedHeaders(
        this WebApplication app,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection(SectionName);
        var settings = section.Get<ForwardedHeadersSettings>() ?? new ForwardedHeadersSettings();

        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        };

        if (settings.TrustAllNetworks)
        {
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        }
        else
        {
            foreach (var network in settings.TrustedNetworks)
            {
                options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
            }
        }

        app.UseForwardedHeaders(options);
    }
}
