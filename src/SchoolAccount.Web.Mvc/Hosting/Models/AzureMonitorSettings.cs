namespace SchoolAccount.Web.Mvc.Hosting.Models;

public static class AzureMonitorSettings
{
    /// <summary>
    /// The cloud role name telemetry is reported under, which is how this app is told apart from
    /// other components in the Application Map and in queries. It names the component and stays
    /// the same in every environment and across deployments, so the environment and version are
    /// not part of it.
    /// </summary>
    public const string ServiceName = "SchoolAccount.Web";

    /// <summary>
    /// The environment variable Azure Container Apps sets to the name of the replica the app is
    /// running on. It is reported as the role instance, so telemetry can be traced to a replica
    /// rather than to a process id that changes on every restart.
    /// </summary>
    public const string ReplicaNameEnvironmentVariable = "CONTAINER_APP_REPLICA_NAME";

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
