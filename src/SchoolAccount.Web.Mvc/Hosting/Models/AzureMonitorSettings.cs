namespace SchoolAccount.Web.Mvc.Hosting.Models;

public static class AzureMonitorSettings
{
    /// <summary>
    /// The environment variable Azure Monitor documents for the connection string. Deployed, it
    /// comes from Azure App Configuration. Only used when <see cref="TelemetrySettings.Destination"/>
    /// is <see cref="TelemetryDestination.AzureMonitor"/>.
    /// </summary>
    public const string ConnectionStringEnvironmentVariable =
        "APPLICATIONINSIGHTS_CONNECTION_STRING";

    /// <summary>
    /// True when the connection string environment variable has a value. Only the environment
    /// variable is checked, so a connection string set under <c>AzureMonitor:ConnectionString</c>
    /// on its own does not turn the distro on.
    /// </summary>
    public static bool IsConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration[ConnectionStringEnvironmentVariable]);
}
