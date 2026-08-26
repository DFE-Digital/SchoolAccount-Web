using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
}
