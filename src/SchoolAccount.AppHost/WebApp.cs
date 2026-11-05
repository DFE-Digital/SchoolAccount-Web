namespace SchoolAccount.AppHost;

internal static class WebApp
{
    public static IResourceBuilder<ProjectResource> AddWebApp(
        this IDistributedApplicationBuilder builder,
        bool useFreePort
    )
    {
        var projectPath = new Projects.SchoolAccount_Web_Mvc().ProjectPath;

        return builder
            .AddLocalProject(ResourceNames.Web, projectPath, useFreePort)
            .WithHttpHealthCheck("/health");
    }
}
