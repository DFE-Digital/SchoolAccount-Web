using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Configuration;

namespace SchoolAccount.E2ETests.Common.Fixtures;

public class BrowserFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public TestSettings Settings { get; private set; } = null!;

    public IBrowser Browser =>
        _browser
        ?? throw new InvalidOperationException(
            "The browser is only available once the fixture has started."
        );

    public virtual async ValueTask InitializeAsync()
    {
        Settings = ConfigurationLoader.LoadSettings();

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = Settings.Headless }
        );
    }

    public async ValueTask DisposeAsync()
    {
        // Either may be null if setup failed part way; the setup failure is the one reported.
        try
        {
            if (_browser is not null)
            {
                await _browser.CloseAsync();
            }
        }
        finally
        {
            _playwright?.Dispose();
        }
    }
}
