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
    /// The configuration key the distro binds its own options from, so the connection string can
    /// also be set in appsettings, user secrets or Azure App Configuration.
    /// </summary>
    public const string ConnectionStringKey = "AzureMonitor:ConnectionString";

    /// <summary>
    /// True when either of the places the distro reads the connection string from has a value.
    /// </summary>
    public static bool IsConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration[ConnectionStringEnvironmentVariable])
        || !string.IsNullOrWhiteSpace(configuration[ConnectionStringKey]);
}
