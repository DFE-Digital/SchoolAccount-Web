using System.Reflection;
using Aspire.Hosting.Dotnet;

namespace SchoolAccount.AppHost;

internal static class CollectApi
{
    public static IResourceBuilder<DotnetProjectResource> AddCollectApi(
        this IDistributedApplicationBuilder builder
    )
    {
        // Any free port, not the launch profile's 5101: a Collect API started by hand often
        // already has that, and the web app would then be sent to it instead. The web app is
        // given this one's address, so nothing needs a fixed port.
        return builder
            .AddDotnetProject(ResourceNames.CollectApi, FindProject())
            .WithEndpoint("http", endpoint => endpoint.Port = null)
            .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<DotnetProjectResource> WithCollectApi(
        this IResourceBuilder<DotnetProjectResource> web,
        IResourceBuilder<DotnetProjectResource> collectApi
    )
    {
        return web.WithEnvironment(
                "CommonApiSettings__CollectApiUrl",
                collectApi.GetEndpoint("http")
            )
            .WaitFor(collectApi);
    }

    // The path is set once, in the AppHost's project file.
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
