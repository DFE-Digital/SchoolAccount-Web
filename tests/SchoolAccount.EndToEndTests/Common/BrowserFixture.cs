using Microsoft.Playwright;

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// Starts one Chromium for a test class. Each test gets its own browser
/// context, so cookies and sessions never leak between tests.
/// </summary>
public sealed class BrowserFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public IBrowser Browser =>
        _browser ?? throw new InvalidOperationException(EndToEndSettings.SkipReason);

    public async ValueTask InitializeAsync()
    {
        if (!EndToEndSettings.IsEnabled)
        {
            return;
        }

        // Downloads Chromium on first run; a no-op once it is installed.
        var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
        if (exitCode != 0)
        {
            throw new InvalidOperationException(
                $"Installing Chromium failed with exit code {exitCode}."
            );
        }

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    public Task<IBrowserContext> NewContextAsync(string? storageState = null) =>
        Browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                BaseURL = EndToEndSettings.BaseUrl,
                StorageState = storageState,
                // Authelia serves a self-signed certificate generated on first start.
                IgnoreHTTPSErrors = true,
            }
        );

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
    }
}
