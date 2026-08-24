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
    private static ConfigurationManager BuildConfiguration()
    {
        var configManager = new ConfigurationManager();
        configManager.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{AuthenticationSettings.SectionName}:Authority"] = "https://idp.example.com",
                [$"{AuthenticationSettings.SectionName}:ClientId"] = "test-client-id",
                [$"{AuthenticationSettings.SectionName}:MetadataAddress"] =
                    "https://idp.example.com/.well-known/openid-configuration",
            }
        );

        return configManager;
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

        using var configuration = BuildConfiguration();
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
        )
        {
            ProtocolMessage = new OpenIdConnectMessage
            {
                PostLogoutRedirectUri = "https://test-oidc.signin",
            },
        };

        // Act
        await oidcOptions.Events.OnRedirectToIdentityProviderForSignOut(redirectContext);

        // Assert
        mockSession.Received(1).Clear();
    }

    [Fact]
    public async Task Ensure_that_OnRedirectToIdentityProviderForSignOut_logout_redirect_uri_is_https()
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
        )
        {
            ProtocolMessage = new OpenIdConnectMessage
            {
                PostLogoutRedirectUri = "http://test-oidc.signin",
            },
        };

        // Act
        await oidcOptions.Events.OnRedirectToIdentityProviderForSignOut(redirectContext);

        // Assert
        mockSession.Received(1).Clear();
        redirectContext.ProtocolMessage.PostLogoutRedirectUri.ShouldBe("https://test-oidc.signin");
    }

    [Fact]
    public async Task Ensure_that_OnRedirectToIdentityProvider_redirect_uri_is_https()
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
        )
        {
            ProtocolMessage = new OpenIdConnectMessage { RedirectUri = "http://test-oidc.signin" },
        };

        // Act
        await oidcOptions.Events.OnRedirectToIdentityProvider(redirectContext);

        // Assert
        redirectContext.ProtocolMessage.RedirectUri.ShouldBe("https://test-oidc.signin");
    }
}
