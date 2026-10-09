using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using SchoolAccount.Web.Mvc.Hosting.Models;
using Shouldly;
using static SchoolAccount.Web.Mvc.Hosting.Models.AzureMonitorSettings;
using static SchoolAccount.Web.Mvc.Hosting.Models.OpenTelemetrySettings;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions.ServiceCollection;

public class ServiceCollectionAddConfiguredOpenTelemetryExtensionTests
{
    private const string _destination = "Telemetry:Destination";

    private const string _otlpEndpoint = "http://localhost:4317";

    private const string _connectionString =
        "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://example."
        + "in.applicationinsights.azure.com/";

    [Fact]
    public void Registers_the_distro_when_the_destination_is_azure_monitor()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.AzureMonitor)),
            (ConnectionStringEnvironmentVariable, _connectionString)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Prod"));

        // Assert
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(LoggerProvider));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(TracerProvider));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(MeterProvider));
        services.ShouldNotContain(descriptor =>
            descriptor.ServiceType == typeof(IOptionsFactory<OtlpExporterOptions>)
        );
    }

    [Fact]
    public void Exports_logs_traces_and_metrics_over_otlp_when_the_destination_is_otlp()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.Otlp)),
            (OtlpEndpointEnvironmentVariable, _otlpEndpoint)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Development"));

        // Assert
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(LoggerProvider));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(TracerProvider));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(MeterProvider));
        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IOptionsFactory<OtlpExporterOptions>)
        );
    }

    [Fact]
    public void Ignores_a_connection_string_when_the_destination_is_otlp()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.Otlp)),
            (OtlpEndpointEnvironmentVariable, _otlpEndpoint),
            (ConnectionStringEnvironmentVariable, _connectionString)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Development"));

        // Assert
        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IOptionsFactory<OtlpExporterOptions>)
        );
        services.Count(descriptor => descriptor.ServiceType == typeof(LoggerProvider)).ShouldBe(1);
        services.Count(descriptor => descriptor.ServiceType == typeof(TracerProvider)).ShouldBe(1);
        services.Count(descriptor => descriptor.ServiceType == typeof(MeterProvider)).ShouldBe(1);
    }

    [Fact]
    public void Ignores_an_otlp_endpoint_when_the_destination_is_azure_monitor()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.AzureMonitor)),
            (ConnectionStringEnvironmentVariable, _connectionString),
            (OtlpEndpointEnvironmentVariable, _otlpEndpoint)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Prod"));

        // Assert
        services.ShouldNotContain(descriptor =>
            descriptor.ServiceType == typeof(IOptionsFactory<OtlpExporterOptions>)
        );
    }

    [Fact]
    public void Reports_the_container_app_name_as_the_service_name_when_deployed()
    {
        // Arrange
        Environment.SetEnvironmentVariable("CONTAINER_APP_NAME", "detected-container-app");

        try
        {
            using var configuration = BuildConfiguration(
                (_destination, nameof(TelemetryDestination.AzureMonitor)),
                (ConnectionStringEnvironmentVariable, _connectionString)
            );

            // Act
            var serviceName = GetResourceAttribute(configuration, "service.name");

            // Assert
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
            (_destination, nameof(TelemetryDestination.Otlp)),
            (OtlpEndpointEnvironmentVariable, _otlpEndpoint)
        );

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Development"));

        using var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<OpenTelemetryLoggerOptions>>().Value;

        // Assert
        options.IncludeFormattedMessage.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(nameof(TelemetryDestination.None))]
    public void Leaves_telemetry_off_when_the_destination_is_none_or_not_set(string? destination)
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, destination),
            (ConnectionStringEnvironmentVariable, _connectionString),
            (OtlpEndpointEnvironmentVariable, _otlpEndpoint)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Prod"));

        // Assert
        services.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Fails_when_the_destination_is_azure_monitor_without_a_connection_string(
        string? connectionString
    )
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.AzureMonitor)),
            (ConnectionStringEnvironmentVariable, connectionString)
        );

        // Act & Assert
        Should
            .Throw<InvalidOperationException>(() =>
                services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Development"))
            )
            .Message.ShouldContain(ConnectionStringEnvironmentVariable);
    }

    [Fact]
    public void Fails_when_the_destination_is_otlp_without_an_endpoint_outside_development()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.Otlp))
        );

        // Act & Assert
        Should
            .Throw<InvalidOperationException>(() =>
                services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Prod"))
            )
            .Message.ShouldContain(OtlpEndpointEnvironmentVariable);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Leaves_telemetry_off_when_the_destination_is_otlp_without_an_endpoint_in_development(
        string? endpoint
    )
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.Otlp)),
            (OtlpEndpointEnvironmentVariable, endpoint)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Development"));

        // Assert
        services.ShouldBeEmpty();
    }

    private static object? GetResourceAttribute(IConfiguration configuration, string key)
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Prod"));

        using var provider = services.BuildServiceProvider();

        return provider
            .GetRequiredService<LoggerProvider>()
            .GetResource()
            .Attributes.Single(attribute => attribute.Key == key)
            .Value;
    }

    private static IHostEnvironment HostEnvironment(string name)
    {
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(name);

        return environment;
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
