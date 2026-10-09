namespace SchoolAccount.Web.Mvc.Hosting.Models;

/// <summary>
/// Where the app sends its traces, metrics and logs. Chosen per environment in appsettings, so
/// the destination doesn't depend on which environment variables happen to be present.
/// </summary>
public enum TelemetryDestination
{
    /// <summary>Telemetry is off.</summary>
    None,

    /// <summary>Application Insights, through the Azure Monitor distro.</summary>
    AzureMonitor,

    /// <summary>An OTLP endpoint, such as Rider's OpenTelemetry tool window.</summary>
    Otlp,
}

public sealed class TelemetrySettings
{
    public const string SectionName = "Telemetry";

    public TelemetryDestination Destination { get; init; } = TelemetryDestination.None;

    public static TelemetrySettings From(IConfiguration configuration) =>
        configuration.GetSection(SectionName).Get<TelemetrySettings>() ?? new TelemetrySettings();
}
