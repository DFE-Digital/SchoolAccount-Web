using System.Collections.Concurrent;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Playwright;
using SchoolAccount.EndToEndTests.Common;

[assembly: AssemblyFixture(typeof(EndToEndApp))]

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// Shared by every test in the run. Starts the app and the mock identity
/// provider through the Aspire AppHost, launches Chromium, and signs in once
/// per test user, the first time a test asks for them, so later tests start
/// already signed in. Everything is stopped when the run ends.
/// </summary>
public sealed class EndToEndApp : IAsyncLifetime
{
    private static readonly TimeSpan _startTimeout = TimeSpan.FromMinutes(5);

    private readonly ConcurrentDictionary<string, Lazy<Task<string>>> _sessions = new();
    private DistributedApplication? _app;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    /// <summary>The web app's address, for example http://localhost:51234.</summary>
    public string BaseUrl { get; private set; } = string.Empty;

    public async ValueTask InitializeAsync()
    {
        if (!EndToEndSettings.IsEnabled)
        {
            return;
        }

        using var timeout = new CancellationTokenSource(_startTimeout);

        var appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<Projects.SchoolAccount_AppHost>(
                timeout.Token
            );
        _app = await appHost.BuildAsync(timeout.Token);
        await _app.StartAsync(timeout.Token);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("web", timeout.Token);
        BaseUrl = _app.GetEndpoint("web", "http").ToString().TrimEnd('/');

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

    /// <summary>A fresh browser context that isn't signed in.</summary>
    public Task<IBrowserContext> NewContextAsync(string? storageState = null)
    {
        var browser = _browser ?? throw new InvalidOperationException(EndToEndSettings.SkipReason);
        return browser.NewContextAsync(
            new BrowserNewContextOptions { BaseURL = BaseUrl, StorageState = storageState }
        );
    }

    /// <summary>
    /// A fresh browser context already signed in as <paramref name="username"/>.
    /// Each test gets its own context, so tests can't affect each other's pages.
    /// </summary>
    public async Task<IBrowserContext> NewSignedInContextAsync(string username)
    {
        var storageState = await _sessions
            .GetOrAdd(username, user => new Lazy<Task<string>>(() => SignInAsync(user)))
            .Value;

        return await NewContextAsync(storageState);
    }

    private async Task<string> SignInAsync(string username)
    {
        await using var context = await NewContextAsync();
        var page = await context.NewPageAsync();
        await MockSignIn.SignInAsAsync(page, username);

        // The cookies and local storage that make up the signed-in session.
        return await context.StorageStateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}
