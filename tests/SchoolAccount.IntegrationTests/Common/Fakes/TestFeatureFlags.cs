using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace SchoolAccount.IntegrationTests.Common.Fakes;

/// <summary>
/// Feature flags a test can switch off. Every other flag keeps its value from
/// appsettings.IntegrationTests.json. Switch flags off before the first request in a test.
/// </summary>
public sealed class TestFeatureFlags
{
    private readonly HashSet<string> _disabled = [];

    public void Disable(string featureName)
    {
        _disabled.Add(featureName);
    }

    internal bool IsDisabled(string featureName)
    {
        return _disabled.Contains(featureName);
    }
}

/// <summary>
/// Reads flags from configuration the same way the app does, then applies the test's overrides.
/// </summary>
internal sealed class OverridableFeatureDefinitionProvider(
    IConfiguration configuration,
    IOptions<ConfigurationFeatureDefinitionProviderOptions> options,
    TestFeatureFlags overrides
) : IFeatureDefinitionProvider, IDisposable
{
    private readonly ConfigurationFeatureDefinitionProvider _inner = new(
        configuration,
        options.Value
    );

    public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        var definition = await _inner.GetFeatureDefinitionAsync(featureName);

        return ApplyOverride(featureName, definition);
    }

    public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        await foreach (var definition in _inner.GetAllFeatureDefinitionsAsync())
        {
            yield return ApplyOverride(definition.Name, definition);
        }
    }

    public void Dispose()
    {
        _inner.Dispose();
    }

    private FeatureDefinition ApplyOverride(string featureName, FeatureDefinition definition)
    {
        if (!overrides.IsDisabled(featureName))
        {
            return definition;
        }

        return new FeatureDefinition { Name = featureName, Status = FeatureStatus.Disabled };
    }
}
