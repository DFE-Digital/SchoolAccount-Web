using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using SchoolAccount.Web.Mvc.Hosting.Models;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Exports traces, metrics and logs to Azure Monitor through the OpenTelemetry distro. Logs
    /// arrive via Serilog, which forwards its events to the OpenTelemetry logger provider
    /// registered here (see <see cref="HostBuilderExtensions.UseConfiguredSerilog"/>).
    /// </summary>
    /// <remarks>
    /// Does nothing when no connection string is configured, because the distro throws on
    /// startup without one. That is what local development and the integration tests run on.
    /// </remarks>
    public static IServiceCollection AddConfiguredAzureMonitor(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (!AzureMonitorSettings.IsConfigured(configuration))
        {
            return services;
        }

        services.AddOpenTelemetry().UseAzureMonitor();

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
