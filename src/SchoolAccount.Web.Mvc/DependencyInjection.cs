using GovUk.Frontend.AspNetCore;
using SchoolAccount.Web.Mvc.Authentication.Extensions;
using SchoolAccount.Web.Mvc.Features.Header;
using SchoolAccount.Web.Mvc.Infrastructure;
using SchoolAccount.Web.Mvc.TagHelpers.Components;

namespace SchoolAccount.Web.Mvc;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IWebHostEnvironment env,
        IConfigurationManager configuration
    )
    {
        services.AddSession();
        services.AddDsiAuthentication(configuration);
        services.AddControllersWithFeatureViews();
        services.AddGovUkFrontend();
        services.AddTagComponents();
        services.AddScoped<IHeaderContentProvider, HeaderContentProvider>();

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

    private static void AddTagComponents(this IServiceCollection services)
    {
        services.Scan(scan =>
            scan.FromAssembliesOf(typeof(DependencyInjection))
                .AddClasses(
                    classes => classes.AssignableTo<IComponentGenerator>(),
                    publicOnly: false
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}
