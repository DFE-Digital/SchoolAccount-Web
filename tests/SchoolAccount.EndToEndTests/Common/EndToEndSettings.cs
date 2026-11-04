using System.Diagnostics;

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// The end-to-end tests need the app running, so they skip unless
/// <c>E2E_BASE_URL</c> is set. That keeps a plain <c>dotnet test</c> green.
/// When a debugger is attached, they default to the local Docker stack instead,
/// so debugging a test from the IDE runs it rather than skipping it.
/// </summary>
internal static class EndToEndSettings
{
    private const string LocalBaseUrl = "http://localhost:5100";

    public const string SkipReason =
        $"Set E2E_BASE_URL to the running app, for example {LocalBaseUrl}, or debug the test.";

    public static string? BaseUrl { get; } =
        Environment.GetEnvironmentVariable("E2E_BASE_URL") switch
        {
            { Length: > 0 } url => url,
            _ when Debugger.IsAttached => LocalBaseUrl,
            _ => null,
        };

    public static bool IsEnabled => !string.IsNullOrWhiteSpace(BaseUrl);
}
