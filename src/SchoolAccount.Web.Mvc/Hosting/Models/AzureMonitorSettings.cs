namespace SchoolAccount.Web.Mvc.Hosting.Models;

public static class AzureMonitorSettings
{
    /// <summary>
    /// The environment variable Azure Monitor documents for the connection string, which is how
    /// the Container App is expected to supply it.
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
