using Microsoft.AspNetCore.HttpOverrides;
using SchoolAccount.Web.Mvc.Hosting.Models;
using static SchoolAccount.Web.Mvc.Hosting.Models.ForwardedHeadersSettings;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class WebApplicationExtensions
{
    private const string _diagnosticsLoggerCategory = "ForwardedHeaders.Diagnostics";

    /// <summary>
    /// Trusts the reverse proxy's X-Forwarded-For/X-Forwarded-Proto headers, so the app sees
    /// the original scheme and client IP rather than the proxy's internal HTTP hop. Which
    /// proxies are trusted is scoped per environment by <see cref="ForwardedHeadersSettings"/>.
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
    /// <see cref="UseConfiguredForwardedHeaders" />, which it must be registered before.
    /// Skipped in production, as it logs the client IP of every request.
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
