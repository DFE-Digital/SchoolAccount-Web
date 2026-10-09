using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

namespace SchoolAccount.Web.Mvc.UnitTests.Hosting.Extensions;

public class ServiceCollectionExtensionsTests
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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

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
        var services = new ServiceCollection();

        using var configuration = BuildConfiguration(
            (_destination, nameof(TelemetryDestination.Otlp)),
            (OtlpEndpointEnvironmentVariable, endpoint)
        );

        // Act
        services.AddConfiguredOpenTelemetry(configuration, HostEnvironment("Development"));

        // Assert
        services.ShouldBeEmpty();
    }

    [Fact]
    public async Task Serilog_writes_each_log_to_each_provider_once_when_exporting_over_otlp_http()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = "Development" }
        );

        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [_destination] = nameof(TelemetryDestination.Otlp),
                [OtlpEndpointEnvironmentVariable] = "http://localhost:4318",
                ["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf",
            }
        );

        builder.Logging.ClearProviders();
        builder.Host.UseConfiguredSerilog();

        using var countingProvider = new CountingLoggerProvider();
        builder.Services.AddSingleton<ILoggerProvider>(countingProvider);
        builder.Services.AddHttpClient();
        builder.Services.AddConfiguredOpenTelemetry(builder.Configuration, builder.Environment);

        await using var app = builder.Build();

        // Act
        app.Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Test")
            .LogInformation("Logged once");

        // Assert
        countingProvider.Count("Logged once").ShouldBe(1);
    }

    [Fact]
    public void Persists_the_key_ring_to_blob_storage_when_configured()
    {
        // Arrange
        var services = new ServiceCollection();

        using var configuration = BuildDataProtectionConfiguration(
            "https://example.blob.core.windows.net/keys/schoolaccount.xml",
            "https://example.vault.azure.net/keys/data-protection"
        );

        services.AddConfiguredDataProtection(configuration);

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>().Value;

        // Assert
        options.XmlRepository.ShouldNotBeNull();
        options.XmlRepository.GetType().Name.ShouldBe("AzureBlobXmlRepository");
        options.XmlEncryptor.ShouldNotBeNull();
        options.XmlEncryptor.GetType().Name.ShouldBe("AzureKeyVaultXmlEncryptor");
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("https://example.blob.core.windows.net/keys/schoolaccount.xml", null)]
    [InlineData(null, "https://example.vault.azure.net/keys/data-protection")]
    [InlineData("", "")]
    public void Leaves_the_key_ring_alone_when_the_blob_or_key_is_missing(
        string? keyRingBlobUri,
        string? keyEncryptionKeyUri
    )
    {
        // Arrange
        var services = new ServiceCollection();

        using var configuration = BuildDataProtectionConfiguration(keyRingBlobUri, keyEncryptionKeyUri);

        // Act
        services.AddConfiguredDataProtection(configuration);

        // Assert - nothing registered, so the framework's own defaults apply
        services.ShouldBeEmpty();
    }

    private static object? GetResourceAttribute(IConfiguration configuration, string key)
    {
        var services = new ServiceCollection();

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

    private static ConfigurationManager BuildDataProtectionConfiguration(
        string? keyRingBlobUri,
        string? keyEncryptionKeyUri
    ) =>
        BuildConfiguration(
            ($"{DataProtectionSettings.SectionName}:KeyRingBlobUri", keyRingBlobUri),
            ($"{DataProtectionSettings.SectionName}:KeyEncryptionKeyUri", keyEncryptionKeyUri)
        );

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

    private sealed class CountingLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _messages = [];

        public int Count(string message)
        {
            lock (_messages)
            {
                return _messages.Count(logged => logged == message);
            }
        }

        public ILogger CreateLogger(string categoryName) => new CountingLogger(_messages);

        public void Dispose() { }

        private sealed class CountingLogger(List<string> messages) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state)
                where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter
            )
            {
                lock (messages)
                {
                    messages.Add(formatter(state, exception));
                }
            }
        }
    }
}
