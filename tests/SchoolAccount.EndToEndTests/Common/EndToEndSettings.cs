namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// The end-to-end tests start the app and its dependencies in Docker through
/// the Aspire AppHost. They don't run in CI yet, which has no step for them,
/// so they skip when the <c>CI</c> environment variable is set.
/// </summary>
internal static class EndToEndSettings
{
    public const string SkipReason = "End-to-end tests don't run in CI yet.";

    public static bool IsEnabled =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CI"));
}
