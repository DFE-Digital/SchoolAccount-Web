using System.Collections.Concurrent;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Playwright;
using static Aspire.Hosting.Testing.DistributedApplicationTestingBuilder;

[assembly: AssemblyFixture(typeof(SchoolAccount.EndToEndTests.Common.EndToEndApp))]

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// Shared by every test in the run. Starts the app and the mock identity
/// provider through the Aspire AppHost, launches Chromium, and signs in once
/// per test user, the first time a test asks for them, so later tests start
/// already signed in. Everything is stopped when the run ends.
/// </summary>
public sealed class EndToEndApp : IAsyncLifetime
{
    private const string NotRunningMessage =
        "The end-to-end app isn't running. Skip the test when EndToEndSettings.IsEnabled is false.";

    private static readonly TimeSpan _startTimeout = TimeSpan.FromMinutes(5);

    // In start-up order. Waiting on each in turn means a failure names the
    // resource at fault, rather than the web app waiting forever.
    private static readonly string[] _resources = ["mock-oauth2-server", "web"];

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

        // Stopping the run, for example from the IDE, also stops start-up.
        var runCancelled = TestContext.Current.CancellationToken;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(runCancelled);

        timeout.CancelAfter(_startTimeout);

        // Installed before the app starts, so a failure here leaves nothing running.
        InstallChromium();

        try
        {
            await StartAppAsync(timeout.Token);

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync();
        }
        catch (OperationCanceledException exception)
            when (timeout.IsCancellationRequested && !runCancelled.IsCancellationRequested)
        {
            await DisposeAsync();
            throw new TimeoutException(
                $"The end-to-end stack didn't start within {_startTimeout.TotalMinutes} minutes. "
                    + "Check Docker is running.",
                exception
            );
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    private async Task StartAppAsync(CancellationToken cancellationToken)
    {
        // Asked for explicitly, so the tests don't depend on the AppHost's default.
        var appHost = await CreateAsync<Projects.SchoolAccount_AppHost>(
            ["UseMockSignIn=true"],
            cancellationToken
        );

        _app = await appHost.BuildAsync(cancellationToken);

        await _app.StartAsync(cancellationToken);

        foreach (var resource in _resources)
        {
            // Throws straight away if the resource fails to start, for example
            // when Docker isn't running, instead of waiting for the timeout.
            await _app.ResourceNotifications.WaitForResourceHealthyAsync(
                resource,
                WaitBehavior.StopOnResourceUnavailable,
                cancellationToken
            );
        }

        BaseUrl = _app.GetEndpoint("web", "http").ToString().TrimEnd('/');
    }

    // Downloads Chromium on first run; a no-op once it is installed.
    private static void InstallChromium()
    {
        var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);

        if (exitCode != 0)
        {
            throw new InvalidOperationException(
                $"Installing Chromium failed with exit code {exitCode}."
            );
        }
    }

    /// <summary>A fresh browser context that isn't signed in.</summary>
    public Task<IBrowserContext> NewContextAsync(string? storageState = null)
    {
        if (_browser is null)
        {
            throw new InvalidOperationException(NotRunningMessage);
        }

        var options = new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            StorageState = storageState,
        };

        return _browser.NewContextAsync(options);
    }

    /// <summary>
    /// A fresh browser context already signed in as <paramref name="username"/>.
    /// Each test gets its own context, so tests can't affect each other's pages.
    /// </summary>
    public async Task<IBrowserContext> NewSignedInContextAsync(string username)
    {
        var session = _sessions.GetOrAdd(
            username,
            user => new Lazy<Task<string>>(() => SignInAsync(user))
        );

        string storageState;

        try
        {
            storageState = await session.Value;
        }
        catch
        {
            // Forget the failed sign-in, so the next test for this user tries
            // again. Only this attempt is removed, never a newer one.
            _sessions.TryRemove(KeyValuePair.Create(username, session));
            throw;
        }

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

    // Safe to call more than once: start-up failures dispose straight away, and
    // xUnit may dispose again at the end of the run.
    public async ValueTask DisposeAsync()
    {
        var browser = Interlocked.Exchange(ref _browser, null);
        var playwright = Interlocked.Exchange(ref _playwright, null);
        var app = Interlocked.Exchange(ref _app, null);

        // Stop the app even if closing the browser fails, so no containers are left running.
        try
        {
            if (browser is not null)
            {
                await browser.DisposeAsync();
            }

            playwright?.Dispose();
        }
        finally
        {
            if (app is not null)
            {
                await app.DisposeAsync();
            }
        }
    }
}
