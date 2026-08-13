using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Authentication.Models;

namespace SchoolAccount.Web.Mvc.Authentication.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDsiAuthentication(
        this IServiceCollection services,
        IConfigurationManager configuration
    )
    {
        var settings =
            configuration
                .GetRequiredSection(AuthenticationSettings.SectionName)
                .Get<AuthenticationSettings>()
            ?? throw new ArgumentException("Authentication settings not found in configuration.");

        services.AddHttpContextAccessor();

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = settings.Authority;
                options.ClientId = settings.ClientId;

                options.SignedOutCallbackPath = "/account/loggedout";

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.IdToken;

                options.Scope.Add("organisation");
                options.Scope.Add("email");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.MapInboundClaims = false;

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProviderForSignOut = async context =>
                    {
                        context.HttpContext.Session.Clear();
                        await Task.CompletedTask;
                    },
                };
            });

        services.AddScoped<IUserContext, UserContext>();
    }
}
