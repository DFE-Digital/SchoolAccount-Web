namespace SchoolAccount.Web.Mvc.Hosting.Models;

public static class OpenTelemetrySettings
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
}
