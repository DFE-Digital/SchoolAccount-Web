using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Hosting.Extensions;

public class AzureAppConfigurationExtensionsTests
{
    private const string _enabledKey = "AzureAppConfiguration:Enabled";

    [Fact]
    public void Does_not_register_the_refresh_services_when_app_configuration_is_disabled()
    {
        // Arrange
        var builder = CreateBuilder(enabled: false);

        // Act
        builder.AddConfiguredAzureAppConfiguration();

        // Assert
        builder.Services.ShouldNotContain(descriptor =>
            descriptor.ServiceType == typeof(IConfigurationRefresherProvider)
        );
    }

    [Fact]
    public void Loads_app_configuration_when_it_is_enabled()
    {
        // Arrange
        var builder = CreateBuilder(enabled: true);

        // Act
        var exception = Should.Throw<InvalidOperationException>(
            builder.AddConfiguredAzureAppConfiguration
        );

        // Assert
        exception.Message.ShouldContain("AzureAppConfiguration:Endpoint");
    }

    private static WebApplicationBuilder CreateBuilder(bool enabled)
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());

        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?> { [_enabledKey] = enabled.ToString() }
        );

        return builder;
    }
}
