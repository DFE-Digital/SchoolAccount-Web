namespace SchoolAccount.Web.Mvc.Hosting.Models;

public static class OpenTelemetrySettings
{
    /// <summary>
    /// The standard OpenTelemetry variable naming the OTLP endpoint to send telemetry to. Rider's
    /// OpenTelemetry tool window sets it for anything run from the IDE, so it is not set by hand.
    /// It is left unset in deployed environments, which send to Azure Monitor instead.
    /// </summary>
    public const string OtlpEndpointEnvironmentVariable = "OTEL_EXPORTER_OTLP_ENDPOINT";

    /// <summary>
    /// True when the OTLP endpoint environment variable has a value.
    /// </summary>
    public static bool IsOtlpConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration[OtlpEndpointEnvironmentVariable]);
}
