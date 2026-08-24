using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Authentication.Handlers;
using SchoolAccount.Web.Mvc.Authentication.Models;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

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

                options.Scope.Add(Organisation);
                options.Scope.Add(Email);
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.MapInboundClaims = false;

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = OpenIdConnectEventHandlers.OnTicketReceived,
                };
            });

        services.AddScoped<IUserContext, UserContext>();
    }
}
