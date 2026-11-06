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

    [Theory]
    [InlineData(-1, true)]
    [InlineData(1, false)]
    public async Task Time_window_filter_enables_a_flag_once_its_start_date_has_passed(
        int startOffsetInDays,
        bool expectedIsEnabled
    )
    {
        // Arrange
        var start = DateTimeOffset.UtcNow.AddDays(startOffsetInDays).ToString("R");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["feature_management:feature_flags:0:id"] =
                        FeatureFlags.MultiSchoolsStatusTableErrorsAndQueries,
                    ["feature_management:feature_flags:0:enabled"] = "true",
                    ["feature_management:feature_flags:0:conditions:client_filters:0:name"] =
                        "Microsoft.TimeWindow",
                    [
                        "feature_management:feature_flags:0:conditions:client_filters:0:parameters:Start"
                    ] = start,
                }
            )
            .Build();

        await using var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddFeatureManagement(configuration)
            .Services.BuildServiceProvider();

        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(
            FeatureFlags.MultiSchoolsStatusTableErrorsAndQueries
        );

        // Assert
        isEnabled.ShouldBe(expectedIsEnabled);
    }
}
