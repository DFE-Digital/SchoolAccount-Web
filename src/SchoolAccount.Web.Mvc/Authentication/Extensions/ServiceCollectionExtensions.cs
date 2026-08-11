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
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
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
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/";
                options.LogoutPath = "/account/signout";
                options.AccessDeniedPath = "/error/403";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "sa-cookie";
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = settings.Authority;
                options.ClientId = settings.ClientId;
                options.MetadataAddress = settings.MetadataAddress;

                options.CallbackPath = !string.IsNullOrEmpty(settings.CallbackPath)
                    ? settings.CallbackPath
                    : "/signin-oidc";

                options.SignedOutCallbackPath = !string.IsNullOrEmpty(
                    settings.SignedOutCallbackPath
                )
                    ? settings.SignedOutCallbackPath
                    : "/account/signedout";

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.IdToken;

                options.Scope.Add("organisation");
                options.Scope.Add("email");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SkipUnrecognizedRequests = true;

                options.MapInboundClaims = false;
            });

        services.AddScoped<IUserContext, UserContext>();

        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

        services.AddAuthorizationBuilder().SetDefaultPolicy(policy).SetFallbackPolicy(policy);
    }
}
