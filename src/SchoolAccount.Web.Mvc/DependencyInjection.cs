using GovUk.Frontend.AspNetCore;
using SchoolAccount.Web.Mvc.Authentication.Extensions;
using SchoolAccount.Web.Mvc.Infrastructure;

namespace SchoolAccount.Web.Mvc;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IWebHostEnvironment env,
        IConfigurationManager configuration
    )
    {
        services.AddDsiAuthentication(configuration);
        services.AddControllersWithFeatureViews();
        services.AddGovUkFrontend();

        if (env.IsDevelopment())
        {
            services.AddSassCompiler();
        }

        return services;
    }

    private static void AddControllersWithFeatureViews(this IServiceCollection services)
    {
        services
            .AddControllersWithViews(options => options.Conventions.Add(new FeatureConvention()))
            .AddRazorOptions(options =>
            {
                // {0} = view name, {3} = feature folder path (see FeatureViewLocationExpander)
                options.ViewLocationFormats.Insert(0, "/Features/{3}/{0}.cshtml");
                options.ViewLocationFormats.Insert(1, "/Features/Shared/Layout/{0}.cshtml");
                options.ViewLocationFormats.Insert(2, "/Features/Shared/{0}.cshtml");
                options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
            });
    }
}
