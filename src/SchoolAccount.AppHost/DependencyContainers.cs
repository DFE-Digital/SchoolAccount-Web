using Microsoft.Extensions.Configuration;

namespace SchoolAccount.AppHost;

/// <summary>
/// How the dependency containers (SQL Server, Azurite and the Flyway runs) live, and how they're
/// grouped in Docker.
/// </summary>
internal static class DependencyContainers
{
    // Set PersistentContainers=false to have them removed when the AppHost stops, as the E2E
    // tests do so every run starts from clean data.
    private const string PersistentSettingName = "PersistentContainers";

    // Docker Desktop groups containers by the label docker compose uses for its project name.
    private const string GroupLabel = "com.docker.compose.project";
    private const string PersistentGroup = "schoolaccount";
    private const string SessionGroup = "schoolaccount-tests";

    /// <summary>
    /// Persistent by default: SQL Server and Azurite keep running after the AppHost stops and are
    /// reused, data and all, the next time it starts. Remove them with docker rm -f for a clean
    /// start.
    /// </summary>
    public static ContainerLifetime GetDependencyLifetime(
        this IDistributedApplicationBuilder builder
    )
    {
        return IsPersistent(builder) ? ContainerLifetime.Persistent : ContainerLifetime.Session;
    }

    /// <summary>
    /// Puts the container in one group in Docker Desktop: schoolaccount for the persistent ones,
    /// schoolaccount-tests for the ones a test run starts and removes.
    /// </summary>
    public static IResourceBuilder<T> WithDependencyGroup<T>(this IResourceBuilder<T> container)
        where T : ContainerResource
    {
        var group = IsPersistent(container.ApplicationBuilder) ? PersistentGroup : SessionGroup;

        return container.WithContainerRuntimeArgs("--label", $"{GroupLabel}={group}");
    }

    private static bool IsPersistent(IDistributedApplicationBuilder builder)
    {
        return builder.Configuration.GetValue(PersistentSettingName, defaultValue: true);
    }
}
