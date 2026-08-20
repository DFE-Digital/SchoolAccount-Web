using Microsoft.AspNetCore.Authentication;
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
                    OnRedirectToIdentityProviderForSignOut = async context =>
                    {
                        context.HttpContext.Session.Clear();
                        await Task.CompletedTask;
                    },

                    OnTicketReceived = OpenIdConnectEventHandlers.OnTicketReceived,

                    // within ACA a container runs on http, though available as https publicly
                    // this causes the OIDC redirect_url to have the http protocol, rather than https
                    // DSI does not allow http redirect URLS. The following corrects the URL
                    OnRedirectToIdentityProvider = async n =>
                    {
                        n.ProtocolMessage.RedirectUri = n.ProtocolMessage.RedirectUri.Replace(
                            "http://",
                            "https://"
                        );
                        await Task.CompletedTask;
                    },
                };
            });

        services.AddScoped<IUserContext, UserContext>();
    }
}
