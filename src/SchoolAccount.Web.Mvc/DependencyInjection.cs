using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using SchoolAccount.Web.Mvc.Authentication;
using SchoolAccount.Web.Mvc.Authentication.Extensions;
using SchoolAccount.Web.Mvc.Features.Header;
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
        services.AddSession();
        services.AddDsiAuthentication(configuration);
        services.AddAuthorisation();
        services.AddControllersWithFeatureViews();
        services.AddGovUkFrontend();
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

    private static void AddAuthorisation(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimConstants.OrganisationId)
                    .Build()
            );
    }
}
