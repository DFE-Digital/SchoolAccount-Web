using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions;

public class WebApplicationExtensionsTests
{
    private readonly FakeLogger _logger = new();

    [Fact]
    public void Logs_each_otel_setting_with_its_value()
    {
        // Arrange
        var configuration = BuildConfiguration(
            ("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317"),
            ("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
        );

        // Act
        WebApplicationExtensions.LogTelemetryConfiguration(configuration, _logger);

        // Assert
        Messages()
            .ShouldContain("Telemetry setting OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317");
        Messages().ShouldContain("Telemetry setting OTEL_EXPORTER_OTLP_PROTOCOL=grpc");
    }

    [Fact]
    public void Hides_otel_header_values()
    {
        // Arrange
        var configuration = BuildConfiguration(("OTEL_EXPORTER_OTLP_HEADERS", "api-key=secret"));

        // Act
        WebApplicationExtensions.LogTelemetryConfiguration(configuration, _logger);

        // Assert
        Messages().ShouldContain("Telemetry setting OTEL_EXPORTER_OTLP_HEADERS=(hidden)");
        Messages().ShouldAllBe(message => !message.Contains("secret"));
    }

    [Fact]
    public void Logs_the_connection_string_ingestion_endpoint_but_not_its_key()
    {
        // Arrange
        var configuration = BuildConfiguration(
            (
                "APPLICATIONINSIGHTS_CONNECTION_STRING",
                "InstrumentationKey=12345678-0000-0000-0000-009876543210;IngestionEndpoint=http://localhost:63907/"
            )
        );

        // Act
        WebApplicationExtensions.LogTelemetryConfiguration(configuration, _logger);

        // Assert
        Messages()
            .ShouldContain(
                "Telemetry setting APPLICATIONINSIGHTS_CONNECTION_STRING is set, with ingestion endpoint http://localhost:63907/"
            );
        Messages().ShouldAllBe(message => !message.Contains("12345678"));
    }

    [Fact]
    public void Logs_when_no_connection_string_is_set()
    {
        // Arrange
        var configuration = BuildConfiguration();

        // Act
        WebApplicationExtensions.LogTelemetryConfiguration(configuration, _logger);

        // Assert
        Messages()
            .ShouldBe([
                "Telemetry destination is None",
                "Telemetry setting APPLICATIONINSIGHTS_CONNECTION_STRING is not set",
            ]);
    }

    [Fact]
    public void Logs_the_destination()
    {
        // Arrange
        var configuration = BuildConfiguration(("Telemetry:Destination", "AzureMonitor"));

        // Act
        WebApplicationExtensions.LogTelemetryConfiguration(configuration, _logger);

        // Assert
        Messages().ShouldContain("Telemetry destination is AzureMonitor");
    }

    [Fact]
    public void Warns_when_the_destination_is_otlp_without_an_endpoint()
    {
        // Arrange
        var configuration = BuildConfiguration(("Telemetry:Destination", "Otlp"));

        // Act
        WebApplicationExtensions.LogTelemetryConfiguration(configuration, _logger);

        // Assert
        _logger
            .Collector.GetSnapshot()
            .Where(record => record.Level == LogLevel.Warning)
            .ShouldHaveSingleItem()
            .Message.ShouldBe(
                "Telemetry destination is Otlp but OTEL_EXPORTER_OTLP_ENDPOINT is not set, so telemetry is off"
            );
    }

    private string[] Messages() =>
        [.. _logger.Collector.GetSnapshot().Select(record => record.Message)];

    private static IConfiguration BuildConfiguration(
        params (string Key, string? Value)[] settings
    ) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(
                settings.ToDictionary(setting => setting.Key, setting => setting.Value)
            )
            .Build();
}
