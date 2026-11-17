using Microsoft.FeatureManagement;

namespace SchoolAccount.IntegrationTests.Common.Fakes;

/// <summary>
/// Reports every feature flag as on unless the test switches it off. It does not read flags
/// from configuration. Switch flags off before the first request in a test.
/// </summary>
public sealed class TestFeatureFlags : IFeatureDefinitionProvider
{
    private readonly HashSet<string> _disabled = [];

    public void Disable(string featureName)
    {
        _disabled.Add(featureName);
    }

    public Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        var definition = new FeatureDefinition
        {
            Name = featureName,
            EnabledFor = [new FeatureFilterConfiguration { Name = "AlwaysOn" }],
            Status = _disabled.Contains(featureName)
                ? FeatureStatus.Disabled
                : FeatureStatus.Conditional,
        };

        return Task.FromResult(definition);
    }

    public IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        throw new NotSupportedException("Tests check flags by name and never list them.");
    }
}
