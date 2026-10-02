using Serilog;

namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class HostBuilderExtensions
{
    /// <summary>
    /// Takes Serilog's sinks and levels from configuration rather than code, so they can be set
    /// per environment, and reads enrichers registered in the container.
    /// </summary>
    /// <remarks>
    /// Also writes to the other registered logger providers, which is how events reach the
    /// OpenTelemetry provider added by <c>AddConfiguredOpenTelemetry</c>. Without it Serilog
    /// replaces the providers and nothing is exported.
    /// </remarks>
    public static IHostBuilder UseConfiguredSerilog(this IHostBuilder host) =>
        host.UseSerilog(
            (context, services, loggerConfiguration) =>
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext(),
            writeToProviders: true
        );
}
