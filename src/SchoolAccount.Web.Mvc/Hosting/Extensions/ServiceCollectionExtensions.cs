using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SchoolAccount.Web.Mvc.Hosting.Models;
using SchoolAccount.Web.Mvc.Hosting.Telemetry;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    public const string OtlpTracesEndpointKey = "OTEL_EXPORTER_OTLP_TRACES_ENDPOINT";

    /// <summary>
    /// Shown as the cloud role name in Application Insights.
    /// </summary>
    public const string ServiceName = "schoolaccount-web-app";

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

    /// <summary>
    /// Exports traces, metrics and logs over OTLP/HTTP to Azure Monitor. Endpoints, protocol and
    /// metric temporality come from the standard OTEL_EXPORTER_OTLP_* environment variables set by
    /// the infrastructure, and requests are authenticated with the app's managed identity.
    /// </summary>
    /// <remarks>
    /// Does nothing when no OTLP traces endpoint is configured, which is what local development
    /// and the integration tests run on.
    /// </remarks>
    public static IServiceCollection AddConfiguredOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (string.IsNullOrWhiteSpace(configuration[OtlpTracesEndpointKey]))
        {
            return services;
        }

        var credentials = new DefaultAzureCredential();

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(ServiceName))
            .WithTracing(tracing =>
                tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation()
            )
            .WithMetrics(metrics =>
                metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation()
            )
            .UseOtlpExporter();

        // UseOtlpExporter reads its defaults from the options named "otlp", and every signal's
        // exporter inherits this HttpClient factory from them.
        services.Configure<OtlpExporterOptions>(
            "otlp",
            options =>
                options.HttpClientFactory = () =>
                    new HttpClient(
                        new AzureMonitorAuthenticationHandler(credentials)
                        {
                            InnerHandler = new HttpClientHandler(),
                        }
                    )
                    {
                        Timeout = TimeSpan.FromMilliseconds(options.TimeoutMilliseconds),
                    }
        );

        return services;
    }
}
