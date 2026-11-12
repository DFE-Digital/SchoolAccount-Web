using Microsoft.Extensions.Configuration;

namespace SchoolAccount.E2ETests.Common.Configuration;

public static class ConfigurationLoader
{
    public static TestSettings LoadSettings()
    {
        // Later sources override earlier ones, so user secrets take priority.
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(ConfigurationLoader).Assembly, optional: true)
            .Build();

        var settings = new TestSettings();
        configuration.Bind(settings);

        return settings;
    }
}
