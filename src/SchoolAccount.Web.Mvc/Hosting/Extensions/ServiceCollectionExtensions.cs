using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using OpenTelemetry.Logs;
using SchoolAccount.Web.Mvc.Hosting.Models;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up OpenTelemetry. Traces, metrics and logs go to Azure Monitor through its distro when
    /// a connection string is configured, and logs also go to an OTLP endpoint when one is, which
    /// is how they reach Rider's OpenTelemetry tool window locally. Logs arrive via Serilog, which
    /// forwards its events to the OpenTelemetry logger provider registered here (see
    /// <see cref="HostBuilderExtensions.UseConfiguredSerilog"/>).
    /// </summary>
    /// <remarks>
    /// Does nothing when neither is configured, because the distro throws on startup without a
    /// connection string. Only logs are sent over OTLP, as there is no trace or metric
    /// instrumentation without the distro.
    /// <para>
    /// The role name and instance are left to the distro's resource detectors. On Container Apps
    /// they are the container app name and the replica name, so environments with differently
    /// named container apps stay apart in Azure Monitor. They are not set here, because anything
    /// set here is overridden by those detectors or has to override them.
    /// </para>
    /// <para>
    /// Leave the OTLP protocol at its default of gRPC, which is what Rider's receiver speaks. With
    /// <c>OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf</c> every log record was exported twice while
    /// an <c>IHttpClientFactory</c> was registered, as it is for the API clients.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddConfiguredOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var useAzureMonitor = AzureMonitorSettings.IsConfigured(configuration);
        var useOtlp = OpenTelemetrySettings.IsOtlpConfigured(configuration);

        if (!useAzureMonitor && !useOtlp)
        {
            return services;
        }

        var builder = services.AddOpenTelemetry();

        if (useAzureMonitor)
        {
            builder.UseAzureMonitor();
        }

        if (useOtlp)
        {
            // Without this the log body is the unrendered template, e.g. "Now listening on: {address}"
            builder.WithLogging(
                logging => logging.AddOtlpExporter(),
                options => options.IncludeFormattedMessage = true
            );
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
