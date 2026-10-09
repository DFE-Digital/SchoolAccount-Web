using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using SchoolAccount.Web.Mvc.Hosting.Models;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up OpenTelemetry to send traces, metrics and logs to the destination chosen by
    /// <see cref="TelemetrySettings.Destination"/>: Application Insights through the Azure
    /// Monitor distro, an OTLP endpoint such as Rider's OpenTelemetry tool window, or nowhere.
    /// Settings for the destination that isn't chosen are ignored. Logs arrive via Serilog, which
    /// forwards its events to the OpenTelemetry logger provider registered here (see
    /// <see cref="HostBuilderExtensions.UseConfiguredSerilog"/>).
    /// </summary>
    /// <remarks>
    /// Fails at startup if the chosen destination isn't configured, so a deployed app can't
    /// silently lose its telemetry. The exception is OTLP in development, where running without a
    /// collector, for example outside Rider, just leaves telemetry off.
    /// </remarks>
    public static IServiceCollection AddConfiguredOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        var destination = TelemetrySettings.From(configuration).Destination;

        switch (destination)
        {
            case TelemetryDestination.AzureMonitor:
                if (!AzureMonitorSettings.IsConfigured(configuration))
                {
                    throw new InvalidOperationException(
                        $"{TelemetrySettings.SectionName}:{nameof(TelemetrySettings.Destination)} is "
                            + $"{destination}, but {AzureMonitorSettings.ConnectionStringEnvironmentVariable} "
                            + "is not set."
                    );
                }

                services.AddOpenTelemetry().UseAzureMonitor();
                break;

            case TelemetryDestination.Otlp:
                if (!OpenTelemetrySettings.IsOtlpConfigured(configuration))
                {
                    if (environment.IsDevelopment())
                    {
                        break;
                    }

                    throw new InvalidOperationException(
                        $"{TelemetrySettings.SectionName}:{nameof(TelemetrySettings.Destination)} is "
                            + $"{destination}, but {OpenTelemetrySettings.OtlpEndpointEnvironmentVariable} "
                            + "is not set."
                    );
                }

                services
                    .AddOpenTelemetry()
                    .UseOtlpExporter()
                    .WithLogging(_ => { }, options => options.IncludeFormattedMessage = true)
                    .WithTracing(tracing =>
                        tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation()
                    )
                    .WithMetrics(metrics =>
                        metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation()
                    );
                break;

            case TelemetryDestination.None:
            default:
                break;
        }

        return services;
    }

    /// <summary>
    /// Persists the Data Protection key ring to blob storage, encrypted with a Key Vault key, so
    /// that every instance of the app shares one key ring, and it survives a restart. Without this
    /// the keys are ephemeral and per-instance, which breaks anything encrypted by one instance
    /// and read by another.
    /// </summary>
    /// <remarks>
    /// Falls back to a local key ring when no blob and key are configured, which is
    /// what local development and the integration tests run on.
    /// </remarks>
    public static IServiceCollection AddConfiguredDataProtection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var settings =
            configuration
                .GetSection(DataProtectionSettings.SectionName)
                .Get<DataProtectionSettings>()
            ?? new DataProtectionSettings();

        if (!settings.IsConfigured)
        {
            return services;
        }

        var credentials = new DefaultAzureCredential();

        services
            .AddDataProtection()
            .SetApplicationName(DataProtectionSettings.ApplicationName)
            .PersistKeysToAzureBlobStorage(new Uri(settings.KeyRingBlobUri!), credentials)
            .ProtectKeysWithAzureKeyVault(new Uri(settings.KeyEncryptionKeyUri!), credentials);

        return services;
    }
}
