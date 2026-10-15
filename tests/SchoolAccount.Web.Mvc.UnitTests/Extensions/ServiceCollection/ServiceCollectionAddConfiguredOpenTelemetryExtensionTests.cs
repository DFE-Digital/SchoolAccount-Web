using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions.ServiceCollection;

public class ServiceCollectionAddConfiguredOpenTelemetryExtensionTests
{
    [Fact]
    public void Registers_OpenTelemetry_with_an_authenticated_exporter_when_an_endpoint_is_configured()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            "https://example.uksouth-1.ingest.monitor.azure.com/datacollectionRules/dcr-1/streams/Microsoft-OTLP-Traces/otlp/v1/traces"
        );

        services.AddConfiguredOpenTelemetry(configuration);

        using var provider = services.BuildServiceProvider();

        // Act
        var options = provider
            .GetRequiredService<IOptionsMonitor<OtlpExporterOptions>>()
            .Get("otlp");
        using var client = options.HttpClientFactory();

        // Assert
        provider.GetService<TracerProvider>().ShouldNotBeNull();
        client.Timeout.ShouldBe(TimeSpan.FromMilliseconds(options.TimeoutMilliseconds));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Leaves_telemetry_off_when_no_endpoint_is_configured(string? tracesEndpoint)
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(tracesEndpoint);

        // Act
        services.AddConfiguredOpenTelemetry(configuration);

        // Assert
        services.ShouldBeEmpty();
    }

    private static ConfigurationManager BuildConfiguration(string? tracesEndpoint)
    {
        var configManager = new ConfigurationManager();
        configManager.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [Mvc.Hosting.Extensions.ServiceCollectionExtensions.OtlpTracesEndpointKey] =
                    tracesEndpoint,
            }
        );

        return configManager;
    }
}
