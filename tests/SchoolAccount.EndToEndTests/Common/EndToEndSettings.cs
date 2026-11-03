namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// The end-to-end tests need the app running, so they skip unless
/// <c>E2E_BASE_URL</c> is set. That keeps a plain <c>dotnet test</c> green.
/// </summary>
internal static class EndToEndSettings
{
    public const string SkipReason =
        "Set E2E_BASE_URL to the running app, for example http://localhost:5100.";

    public static string? BaseUrl { get; } = Environment.GetEnvironmentVariable("E2E_BASE_URL");

    public static bool IsEnabled => !string.IsNullOrWhiteSpace(BaseUrl);
}
