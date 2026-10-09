using Microsoft.AspNetCore.HttpOverrides;
using SchoolAccount.Web.Mvc.Hosting.Models;
using static SchoolAccount.Web.Mvc.Hosting.Models.ForwardedHeadersSettings;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class WebApplicationExtensions
{
    private const string _diagnosticsLoggerCategory = "ForwardedHeaders.Diagnostics";
    private const string _telemetryLoggerCategory = "OpenTelemetry.Diagnostics";

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

    /// <summary>
    /// Logs the OpenTelemetry settings the app starts with, so it's clear where telemetry is
    /// going when running locally. Tools such as Rider's plugins add these settings when they
    /// launch the app, so they don't appear in the run configuration. Development only.
    /// </summary>
    public static void LogTelemetryConfiguration(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        var logger = app
            .Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger(_telemetryLoggerCategory);

        LogTelemetryConfiguration(app.Configuration, logger);
    }

    /// <summary>
    /// Logs every <c>OTEL_</c> setting, hiding header values as they can hold API keys, and
    /// whether an Application Insights connection string is set, with its ingestion endpoint but
    /// never its key.
    /// </summary>
    public static void LogTelemetryConfiguration(IConfiguration configuration, ILogger logger)
    {
        if (!logger.IsEnabled(LogLevel.Information))
        {
            return;
        }

        var otelSettings = configuration
            .AsEnumerable()
            .Where(setting =>
                setting.Key.StartsWith("OTEL_", StringComparison.OrdinalIgnoreCase)
                && setting.Value is not null
            )
            .OrderBy(setting => setting.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var (name, value) in otelSettings)
        {
            logger.LogInformation(
                "Telemetry setting {Name}={Value}",
                name,
                name.EndsWith("_HEADERS", StringComparison.OrdinalIgnoreCase) ? "(hidden)" : value
            );
        }

        var connectionString = configuration[
            AzureMonitorSettings.ConnectionStringEnvironmentVariable
        ];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogInformation(
                "Telemetry setting {Name} is not set",
                AzureMonitorSettings.ConnectionStringEnvironmentVariable
            );
            return;
        }

        var ingestionEndpoint = connectionString
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault(part =>
                part.StartsWith("IngestionEndpoint=", StringComparison.OrdinalIgnoreCase)
            )
            ?["IngestionEndpoint=".Length..];

        logger.LogInformation(
            "Telemetry setting {Name} is set, with ingestion endpoint {IngestionEndpoint}",
            AzureMonitorSettings.ConnectionStringEnvironmentVariable,
            ingestionEndpoint ?? "(none)"
        );
    }
}
