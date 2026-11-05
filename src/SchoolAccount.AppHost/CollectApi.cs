using System.Reflection;

namespace SchoolAccount.AppHost;

internal static class CollectApi
{
    public static IResourceBuilder<ProjectResource> AddCollectApi(
        this IDistributedApplicationBuilder builder,
        bool useFreePort
    )
    {
        return builder
            .AddLocalProject(ResourceNames.CollectApi, FindProject(), useFreePort)
            .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> WithCollectApi(
        this IResourceBuilder<ProjectResource> web,
        IResourceBuilder<ProjectResource> collectApi
    )
    {
        return web.WithEnvironment(
                "CommonApiSettings__CollectApiUrl",
                collectApi.GetEndpoint("http")
            )
            .WaitFor(collectApi);
    }

    // The path is set once, in the AppHost's project file, which also builds the project.
    private static string FindProject()
    {
        var projectPath = typeof(CollectApi)
            .Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(attribute => attribute.Key == "CollectApiProjectPath")
            .Value!;

        if (!File.Exists(projectPath))
        {
            throw new InvalidOperationException(
                $"The Collect API wasn't found at {Path.GetFullPath(projectPath)}. "
                    + "Clone SchoolAccount-CollectAPI next to this repository."
            );
        }

        return projectPath;
    }
}
