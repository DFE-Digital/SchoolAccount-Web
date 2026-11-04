using System.Collections.Concurrent;
using Microsoft.Playwright;
using SchoolAccount.EndToEndTests.Common;

[assembly: AssemblyFixture(typeof(SignedInSessions))]

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// Shared by every test in the run. Signs in once per test user, the first
/// time a test asks for them, saves that session's cookies, and gives every
/// later test a browser that starts already signed in.
/// </summary>
public sealed class SignedInSessions : IAsyncLifetime
{
    private readonly BrowserFixture _browser = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<string>>> _sessions = new();

    public ValueTask InitializeAsync() => _browser.InitializeAsync();

    /// <summary>
    /// A fresh browser context already signed in as <paramref name="username"/>.
    /// Each test gets its own context, so tests can't affect each other's pages.
    /// </summary>
    public async Task<IBrowserContext> NewContextAsync(string username)
    {
        var storageState = await _sessions
            .GetOrAdd(username, user => new Lazy<Task<string>>(() => SignInAsync(user)))
            .Value;

        return await _browser.NewContextAsync(storageState);
    }

    private async Task<string> SignInAsync(string username)
    {
        await using var context = await _browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await MockSignIn.SignInAsAsync(page, username);

        // The cookies and local storage that make up the signed-in session.
        return await context.StorageStateAsync();
    }

    public ValueTask DisposeAsync() => _browser.DisposeAsync();
}
