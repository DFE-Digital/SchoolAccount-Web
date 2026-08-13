using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NSubstitute;
using SchoolAccount.Web.Mvc.Authentication.Extensions;
using SchoolAccount.Web.Mvc.Authentication.Models;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions.ServiceCollection;

public class ServiceCollectionAddDsiAuthenticationExtensionTests
{
    private static ConfigurationManager BuildConfiguration(
        string? callbackPath = "/custom/signin-oidc",
        string? signedOutCallbackPath = "/custom/loggedout"
    )
    {
        var configManager = new ConfigurationManager();
        configManager.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{AuthenticationSettings.SectionName}:Authority"] = "https://idp.example.com",
                [$"{AuthenticationSettings.SectionName}:ClientId"] = "test-client-id",
                [$"{AuthenticationSettings.SectionName}:MetadataAddress"] =
                    "https://idp.example.com/.well-known/openid-configuration",
                [$"{AuthenticationSettings.SectionName}:CallbackPath"] = callbackPath,
                [$"{AuthenticationSettings.SectionName}:SignedOutCallbackPath"] =
                    signedOutCallbackPath,
            }
        );

        return configManager;
    }

    [Fact]
    public void Configures_cookie_options()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration();
        services.AddDsiAuthentication(configuration);

        var provider = services.BuildServiceProvider();
        var cookie = provider
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(CookieAuthenticationDefaults.AuthenticationScheme);

        // Assert
        cookie.LoginPath.Value.ShouldBe("/");
        cookie.LogoutPath.Value.ShouldBe("/account/logout");
        cookie.AccessDeniedPath.Value.ShouldBe("/error/403");
        cookie.Cookie.SecurePolicy.ShouldBe(CookieSecurePolicy.Always);
        cookie.Cookie.Name.ShouldBe("sa-cookie");
    }

    [Fact]
    public void Configures_oidc_options_from_settings()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration();
        services.AddDsiAuthentication(configuration);

        var provider = services.BuildServiceProvider();
        var oidc = provider
            .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert
        oidc.Authority.ShouldBe("https://idp.example.com");
        oidc.ClientId.ShouldBe("test-client-id");
        oidc.CallbackPath.Value.ShouldBe("/custom/signin-oidc");
        oidc.SignedOutCallbackPath.Value.ShouldBe("/custom/loggedout");
        oidc.SignInScheme.ShouldBe(CookieAuthenticationDefaults.AuthenticationScheme);
        oidc.ResponseType.ShouldBe(OpenIdConnectResponseType.IdToken);
        oidc.Scope.ShouldContain("organisation");
        oidc.Scope.ShouldContain("email");
        oidc.SaveTokens.ShouldBeTrue();
        oidc.GetClaimsFromUserInfoEndpoint.ShouldBeTrue();
        oidc.MapInboundClaims.ShouldBeFalse();
    }

    [Fact]
    public void Falls_back_to_default_paths_when_not_configured()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(null, null);
        services.AddDsiAuthentication(configuration);

        var provider = services.BuildServiceProvider();
        var oidc = provider
            .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert
        oidc.CallbackPath.Value.ShouldBe("/signin-oidc");
        oidc.SignedOutCallbackPath.Value.ShouldBe("/account/loggedout");
    }

    [Fact]
    public void Registers_authenticated_user_default_and_fallback_policies()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration();
        services.AddDsiAuthentication(configuration);

        // Act
        var provider = services.BuildServiceProvider();
        var authOptions = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        // Assert
        authOptions.DefaultPolicy.Requirements.ShouldContain(r =>
            r is DenyAnonymousAuthorizationRequirement
        );
        authOptions.FallbackPolicy!.Requirements.ShouldContain(r =>
            r is DenyAnonymousAuthorizationRequirement
        );
    }

    [Fact]
    public async Task Ensure_that_OnRedirectToIdentityProviderForSignOut_clears_session()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        using var configuration = BuildConfiguration();
        services.AddDsiAuthentication(configuration);
        var provider = services.BuildServiceProvider();

        var oidcOptions = provider
            .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        var mockSession = Substitute.For<ISession>();
        var httpContext = new DefaultHttpContext { Session = mockSession };

        var scheme = new AuthenticationScheme(
            OpenIdConnectDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme,
            typeof(OpenIdConnectHandler)
        );

        var redirectContext = new RedirectContext(
            httpContext,
            scheme,
            oidcOptions,
            new AuthenticationProperties()
        );

        // Act
        await oidcOptions.Events.OnRedirectToIdentityProviderForSignOut(redirectContext);

        // Assert
        mockSession.Received(1).Clear();
    }
}
