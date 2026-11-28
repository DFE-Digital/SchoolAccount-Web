using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace SchoolAccount.E2ETests.Common.Configuration;

public static class ConfigurationLoader
{
    // Values the run sets for itself, such as the addresses of the dependencies the Aspire
    // AppHost started. They go on top of everything else.
    private static readonly ConcurrentDictionary<string, string?> _overrides = new(
        StringComparer.OrdinalIgnoreCase
    );

    public static TestSettings LoadSettings()
    {
        // Later sources override earlier ones, so user secrets take priority over the files and
        // environment, and the run's own overrides take priority over everything.
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(ConfigurationLoader).Assembly, optional: true)
            .AddInMemoryCollection(_overrides)
            .Build();

        var settings = new TestSettings();
        configuration.Bind(settings);

        return settings;
    }

    /// <summary>Sets values that win over every other source for the rest of the run.</summary>
    public static void Override(IReadOnlyDictionary<string, string?> values)
    {
        foreach (var (key, value) in values)
        {
            _overrides[key] = value;
        }
    }
}
