using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SchoolAccount.TestCommon.Constants;
using SchoolAccount.Web.Mvc.Authentication.Extensions;
using SchoolAccount.Web.Mvc.Authentication.Models;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Authentication.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void Configures_oidc_options_from_settings()
    {
        // Arrange
        var services = new ServiceCollection();

        using var configuration = BuildConfiguration();
        services.AddDsiAuthentication(configuration);

        // Act
        var provider = services.BuildServiceProvider();
        var oidc = provider
            .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert
        oidc.ShouldSatisfyAllConditions(
            claim => claim.Authority.ShouldBe(UrlConstants.OidcAuthorityUrl),
            claim => claim.ClientId.ShouldBe("test-client-id"),
            claim => claim.SignInScheme.ShouldBe(CookieAuthenticationDefaults.AuthenticationScheme),
            claim => claim.ResponseType.ShouldBe(OpenIdConnectResponseType.IdToken),
            claim => claim.Scope.ShouldContain("organisation"),
            claim => claim.Scope.ShouldContain("email"),
            claim => claim.SaveTokens.ShouldBeTrue(),
            claim => claim.GetClaimsFromUserInfoEndpoint.ShouldBeTrue(),
            claim => claim.MapInboundClaims.ShouldBeFalse()
        );
    }

    [Fact]
    public void Falls_back_to_default_paths_when_not_configured()
    {
        // Arrange
        var services = new ServiceCollection();

        using var configuration = BuildConfiguration();
        services.AddDsiAuthentication(configuration);

        // Act
        var provider = services.BuildServiceProvider();
        var oidc = provider
            .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert
        oidc.CallbackPath.Value.ShouldBe(UrlConstants.OidcSignInUrl);
    }

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
}
