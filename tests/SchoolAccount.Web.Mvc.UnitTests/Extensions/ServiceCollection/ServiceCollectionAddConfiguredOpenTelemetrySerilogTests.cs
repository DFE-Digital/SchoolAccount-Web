using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using SchoolAccount.Web.Mvc.Hosting.Models;
using Shouldly;
using static SchoolAccount.Web.Mvc.Hosting.Models.OpenTelemetrySettings;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions.ServiceCollection;

public class ServiceCollectionAddConfiguredOpenTelemetrySerilogTests
{
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
                ["Telemetry:Destination"] = nameof(TelemetryDestination.Otlp),
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
