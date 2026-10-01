using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using Shouldly;
using static SchoolAccount.Web.Mvc.Hosting.Models.AzureMonitorSettings;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions.ServiceCollection;

public class ServiceCollectionAddConfiguredAzureMonitorExtensionTests
{
    private const string ConnectionString =
        "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://example.in.applicationinsights.azure.com/";

    [Theory]
    [InlineData(ConnectionStringEnvironmentVariable)]
    [InlineData(ConnectionStringKey)]
    public void Registers_the_distro_when_a_connection_string_is_configured(string key)
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration((key, ConnectionString));

        // Act
        services.AddConfiguredAzureMonitor(configuration);

        // Assert
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(TracerProvider));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(LoggerProvider));
    }

    [Fact]
    public void Reports_telemetry_under_the_service_name()
    {
        // Arrange
        using var configuration = BuildConfiguration((ConnectionStringKey, ConnectionString));

        // Act
        var serviceName = GetResourceAttribute(configuration, "service.name");

        // Assert
        serviceName.ShouldBe(ServiceName);
    }

    [Fact]
    public void Reports_the_replica_name_as_the_service_instance()
    {
        // Arrange
        using var configuration = BuildConfiguration(
            (ConnectionStringKey, ConnectionString),
            (ReplicaNameEnvironmentVariable, "schoolaccount-web--0000001-abcde")
        );

        // Act
        var serviceInstance = GetResourceAttribute(configuration, "service.instance.id");

        // Assert
        serviceInstance.ShouldBe("schoolaccount-web--0000001-abcde");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Generates_a_service_instance_when_there_is_no_replica_name(string? replicaName)
    {
        // Arrange
        using var configuration = BuildConfiguration(
            (ConnectionStringKey, ConnectionString),
            (ReplicaNameEnvironmentVariable, replicaName)
        );

        // Act
        var serviceInstance = GetResourceAttribute(configuration, "service.instance.id");

        // Assert
        Guid.TryParse((string?)serviceInstance, out _).ShouldBeTrue();
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
        services.AddConfiguredAzureMonitor(configuration);

        // Assert - nothing registered, because the distro throws on startup without one
        services.ShouldBeEmpty();
    }

    private static object? GetResourceAttribute(IConfiguration configuration, string key)
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddConfiguredAzureMonitor(configuration);

        using var provider = services.BuildServiceProvider();

        return provider
            .GetRequiredService<TracerProvider>()
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
