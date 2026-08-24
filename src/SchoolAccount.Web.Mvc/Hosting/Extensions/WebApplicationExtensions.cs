using Microsoft.AspNetCore.HttpOverrides;
using SchoolAccount.Web.Mvc.Hosting.Models;
using static SchoolAccount.Web.Mvc.Hosting.Models.ForwardedHeadersSettings;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class WebApplicationExtensions
{
    private const string _diagnosticsLoggerCategory = "ForwardedHeaders.Diagnostics";

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

    /// <summary>
    /// Logs the request scheme and remote IP either side of
    /// <see cref="UseConfiguredForwardedHeaders" />, so it is possible to see both what the
    /// reverse proxy sent and what ForwardedHeadersMiddleware made of it. Register this
    /// immediately BEFORE <see cref="UseConfiguredForwardedHeaders" />: the raw values are
    /// logged on the way in, and the rewritten values on the way back out, once the rest of
    /// the pipeline has run against the same HttpContext.
    ///
    /// Note that ForwardedHeadersMiddleware consumes the headers it applies - it pops the
    /// entry it used off X-Forwarded-For and moves the connection's original values to
    /// X-Original-For/X-Original-Proto - which is why the two stages log different headers.
    ///
    /// Diagnostics only, and skipped entirely in production: this logs the client IP of every
    /// request, which is personal data and has no place in production logs. Pair it with
    /// "Microsoft.AspNetCore.HttpOverrides": "Debug" to see why a proxy was not trusted.
    /// </summary>
    public static void UseForwardedHeadersDiagnostics(
        this WebApplication app,
        IWebHostEnvironment environment
    )
    {
        if (environment.IsProduction())
        {
            return;
        }

        app.Use(
            async (context, next) =>
            {
                var logger = context
                    .RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(_diagnosticsLoggerCategory);

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug(
                        "Forwarded headers (before): Scheme={Scheme} RemoteIp={RemoteIp} "
                            + "XForwardedFor={XForwardedFor} XForwardedProto={XForwardedProto}",
                        context.Request.Scheme,
                        context.Connection.RemoteIpAddress,
                        context.Request.Headers["X-Forwarded-For"].ToString(),
                        context.Request.Headers["X-Forwarded-Proto"].ToString()
                    );
                }

                try
                {
                    await next();
                }
                finally
                {
                    if (logger.IsEnabled(LogLevel.Debug))
                    {
                        logger.LogDebug(
                            "Forwarded headers (after): Scheme={Scheme} RemoteIp={RemoteIp} "
                                + "XOriginalFor={XOriginalFor} XOriginalProto={XOriginalProto}",
                            context.Request.Scheme,
                            context.Connection.RemoteIpAddress,
                            context.Request.Headers["X-Original-For"].ToString(),
                            context.Request.Headers["X-Original-Proto"].ToString()
                        );
                    }
                }
            }
        );
    }
}
