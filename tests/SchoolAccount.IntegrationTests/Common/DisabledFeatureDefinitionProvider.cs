using Microsoft.FeatureManagement;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Wraps the app's feature definitions and turns one feature off, leaving every other feature
/// as the app configures it.
/// </summary>
internal sealed class DisabledFeatureDefinitionProvider(
    IFeatureDefinitionProvider inner,
    string disabledFeatureName
) : IFeatureDefinitionProvider, IDisposable
{
    public Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName) =>
        featureName == disabledFeatureName
            ? Task.FromResult(Disabled(featureName))
            : inner.GetFeatureDefinitionAsync(featureName);

    public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        await foreach (var definition in inner.GetAllFeatureDefinitionsAsync())
        {
            yield return definition.Name == disabledFeatureName
                ? Disabled(definition.Name)
                : definition;
        }
    }

    public void Dispose() => (inner as IDisposable)?.Dispose();

    private static FeatureDefinition Disabled(string featureName) =>
        new() { Name = featureName, Status = FeatureStatus.Disabled };
}
