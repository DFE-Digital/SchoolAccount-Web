namespace SchoolAccount.AppHost;

internal static class LocalProjects
{
    /// <summary>
    /// Adds a project that runs either on its own launch profile, exactly as if it
    /// were run directly, or on a free port so it can't clash with one already running.
    /// </summary>
    public static IResourceBuilder<ProjectResource> AddLocalProject(
        this IDistributedApplicationBuilder builder,
        string name,
        string projectPath,
        bool useFreePort
    )
    {
        if (!useFreePort)
        {
            return builder.AddProject(name, projectPath);
        }

        return builder
            .AddProject(name, projectPath, launchProfileName: null)
            .WithHttpEndpoint(name: "http")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");
    }
}
