using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Replaces the app's DfE Sign-in authentication with a test handler, which decides whether
/// requests are signed in.
/// </summary>
public static class TestAuthentication
{
    public static void Use<THandler>(IServiceCollection services)
        where THandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        const string schemeName = CookieAuthenticationDefaults.AuthenticationScheme;

        services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
        services.RemoveAll<IAuthenticationSchemeProvider>();
        services.RemoveAll<IConfigureOptions<OpenIdConnectOptions>>();
        services.RemoveAll<IPostConfigureOptions<OpenIdConnectOptions>>();
        services.RemoveAll<IConfigureOptions<CookieAuthenticationOptions>>();

        services
            .AddAuthentication(authenticationOptions =>
            {
                authenticationOptions.DefaultScheme = schemeName;
                authenticationOptions.DefaultAuthenticateScheme = schemeName;
                authenticationOptions.DefaultChallengeScheme = schemeName;
                authenticationOptions.DefaultSignInScheme = schemeName;
                authenticationOptions.DefaultSignOutScheme = schemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, THandler>(schemeName, _ => { })
            .AddScheme<AuthenticationSchemeOptions, THandler>(
                OpenIdConnectDefaults.AuthenticationScheme,
                _ => { }
            );
    }
}
