using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
                    OnTokenValidated = context =>
                    {
                        GetOrganisationNameFromClaim(context.Principal);
                        return Task.CompletedTask;
                    },

                    OnRedirectToIdentityProviderForSignOut = async context =>
                    {
                        context.HttpContext.Session.Clear();
                        await Task.CompletedTask;
                    },
                };
            });

        services.AddScoped<IUserContext, UserContext>();
    }

    private static void GetOrganisationNameFromClaim(ClaimsPrincipal? principal)
    {
        var organisationClaim = principal?.FindFirst("organisation")?.Value;
        if (
            string.IsNullOrEmpty(organisationClaim)
            || principal?.Identity is not ClaimsIdentity identity
        )
        {
            return;
        }

        var organisationName = JsonDocument
            .Parse(organisationClaim)
            .RootElement.TryGetProperty("name", out var n)
            ? n.GetString()
            : null;

        if (organisationName is not null)
        {
            identity.AddClaim(new Claim("org_name", organisationName));
        }
    }
}
