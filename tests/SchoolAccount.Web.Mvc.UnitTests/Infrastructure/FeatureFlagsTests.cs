using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using SchoolAccount.Web.Mvc.Infrastructure;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Infrastructure;

public class FeatureFlagsTests
{
    [Fact]
    public async Task Feedback_footer_is_off_by_default_in_deployed_environments()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        await using var serviceProvider = new ServiceCollection()
            .AddFeatureManagement(configuration)
            .Services.BuildServiceProvider();

        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(FeatureFlags.FeedbackFooter);

        // Assert
        isEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task Feedback_footer_default_is_overridden_by_app_configuration()
    {
        // Arrange
        // App Configuration loads flags under FeatureManagement, after the appsettings files.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    [$"FeatureManagement:{FeatureFlags.FeedbackFooter}"] = "true",
                }
            )
            .Build();

        await using var serviceProvider = new ServiceCollection()
            .AddFeatureManagement(configuration)
            .Services.BuildServiceProvider();

        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(FeatureFlags.FeedbackFooter);

        // Assert
        isEnabled.ShouldBeTrue();
    }
}
