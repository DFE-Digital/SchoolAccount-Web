using Aspire.Hosting.Dotnet;

namespace SchoolAccount.AppHost;

internal static class WebApp
{
    // Relative to the AppHost's folder.
    private const string ProjectPath = "../SchoolAccount.Web.Mvc/SchoolAccount.Web.Mvc.csproj";

    // Named rather than left to Aspire's default choice of profile. It keeps the app on
    // https://127.0.0.1:7041, the address registered with DfE Sign-in.
    private const string LaunchProfile = "https";

    public static IResourceBuilder<DotnetProjectResource> AddWebApp(
        this IDistributedApplicationBuilder builder
    )
    {
        return builder
            .AddDotnetProject(
                ResourceNames.Web,
                ProjectPath,
                options => options.LaunchProfileName = LaunchProfile
            )
            .WithHttpHealthCheck("/health");
    }
}
