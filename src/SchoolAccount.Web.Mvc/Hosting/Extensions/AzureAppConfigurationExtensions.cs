namespace SchoolAccount.Web.Mvc.Hosting.Extensions;

public static class AzureAppConfigurationExtensions
{
    private const string _enabledKey = "AzureAppConfiguration:Enabled";

    /// <summary>
    /// Loads settings and feature flags from Azure App Configuration and registers the services
    /// that refresh them, when <c>AzureAppConfiguration:Enabled</c> is true.
    /// </summary>
    public static WebApplicationBuilder AddConfiguredAzureAppConfiguration(
        this WebApplicationBuilder builder
    )
    {
        if (builder.Configuration.GetValue<bool>(_enabledKey))
        {
            builder.Configuration.AddAzureAppConfiguration();
            builder.Services.AddAzureAppConfiguration();
        }

        return builder;
    }

    /// <summary>
    /// Refreshes App Configuration values, including feature flags, as requests come in, so
    /// changes apply without a restart.
    /// </summary>
    public static WebApplication UseConfiguredAzureAppConfiguration(this WebApplication app)
    {
        if (app.Configuration.GetValue<bool>(_enabledKey))
        {
            app.UseAzureAppConfiguration();
        }

        return app;
    }
}
