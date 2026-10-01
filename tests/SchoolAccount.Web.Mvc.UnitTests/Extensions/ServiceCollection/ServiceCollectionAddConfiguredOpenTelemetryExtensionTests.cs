using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using Shouldly;
using static SchoolAccount.Web.Mvc.Hosting.Models.AzureMonitorSettings;
using static SchoolAccount.Web.Mvc.Hosting.Models.OpenTelemetrySettings;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions.ServiceCollection;

public class ServiceCollectionAddConfiguredOpenTelemetryExtensionTests
{
    private const string OtlpEndpoint = "http://localhost:4317";

    private const string ConnectionString =
        "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://example.in.applicationinsights.azure.com/";

    [Fact]
    public void Registers_the_distro_when_a_connection_string_is_configured()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (ConnectionStringEnvironmentVariable, ConnectionString)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration);

        // Assert
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(TracerProvider));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(LoggerProvider));
    }

    [Fact]
    public void Exports_logs_only_when_an_otlp_endpoint_is_configured()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (OtlpEndpointEnvironmentVariable, OtlpEndpoint)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration);

        // Assert - there is no trace or metric instrumentation without the distro
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(LoggerProvider));
        services.ShouldNotContain(descriptor => descriptor.ServiceType == typeof(TracerProvider));
    }

    [Fact]
    public void Reports_the_container_app_name_as_the_service_name_when_deployed()
    {
        // Arrange - the distro's detector reads this from the environment, as it does when deployed
        Environment.SetEnvironmentVariable("CONTAINER_APP_NAME", "detected-container-app");

        try
        {
            using var configuration = BuildConfiguration(
                (ConnectionStringEnvironmentVariable, ConnectionString)
            );

            // Act
            var serviceName = GetResourceAttribute(configuration, "service.name");

            // Assert - nothing here overrides it
            serviceName.ShouldBe("detected-container-app");
        }
        finally
        {
            Environment.SetEnvironmentVariable("CONTAINER_APP_NAME", null);
        }
    }

    [Fact]
    public void Includes_the_rendered_message_in_logs_sent_over_otlp()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (OtlpEndpointEnvironmentVariable, OtlpEndpoint)
        );

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddConfiguredOpenTelemetry(configuration);

        using var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<OpenTelemetryLoggerOptions>>().Value;

        // Assert - otherwise the body is the template, e.g. "Now listening on: {address}"
        options.IncludeFormattedMessage.ShouldBeTrue();
    }

    [Fact]
    public void Registers_a_single_logger_provider_when_both_destinations_are_configured()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (ConnectionStringEnvironmentVariable, ConnectionString),
            (OtlpEndpointEnvironmentVariable, OtlpEndpoint)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration);

        // Assert - a second one would send every log twice
        services.Count(descriptor => descriptor.ServiceType == typeof(LoggerProvider)).ShouldBe(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Leaves_telemetry_alone_when_the_otlp_endpoint_is_blank(string endpoint)
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration((OtlpEndpointEnvironmentVariable, endpoint));

        // Act
        services.AddConfiguredOpenTelemetry(configuration);

        // Assert
        services.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Leaves_telemetry_alone_when_no_connection_string_is_configured(
        string? connectionString
    )
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (ConnectionStringEnvironmentVariable, connectionString)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration);

        // Assert - nothing registered, because the distro throws on startup without one
        services.ShouldBeEmpty();
    }

    [Fact]
    public void Leaves_telemetry_alone_when_only_the_configuration_section_has_a_connection_string()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            ("AzureMonitor:ConnectionString", ConnectionString)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration);

        // Assert - only the environment variable turns the distro on
        services.ShouldBeEmpty();
    }

    private static object? GetResourceAttribute(IConfiguration configuration, string key)
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddConfiguredOpenTelemetry(configuration);

        using var provider = services.BuildServiceProvider();

        return provider
            .GetRequiredService<LoggerProvider>()
            .GetResource()
            .Attributes.Single(attribute => attribute.Key == key)
            .Value;
    }

    private static ConfigurationManager BuildConfiguration(
        params (string Key, string? Value)[] entries
    )
    {
        var configManager = new ConfigurationManager();
        configManager.AddInMemoryCollection(
            entries.ToDictionary(entry => entry.Key, entry => entry.Value)
        );

        return configManager;
    }
}
