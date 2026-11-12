using Microsoft.Playwright;

namespace SchoolAccount.E2ETests.Common.Fixtures;

public abstract class AuthenticatedBrowserFixture : BrowserFixture
{
    private const int MaximumSignInAttempts = 3;

    public string? StorageState { get; private set; }

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        StorageState = await SignInAsync();
    }

    /// <summary>
    /// Signs in and returns the signed-in browser state. Each attempt starts from scratch in a new
    /// browser context. Only Playwright failures (timeouts, pages that didn't load) are retried;
    /// anything else, such as credentials being rejected, fails straight away so repeated attempts
    /// can't lock the account.
    /// </summary>
    private async Task<string> SignInAsync()
    {
        var failures = new List<PlaywrightException>();

        for (var attempt = 1; attempt <= MaximumSignInAttempts; attempt++)
        {
            var context = await Browser.NewContextAsync();
            try
            {
                var page = await context.NewPageAsync();
                await LoginAsync(page);
                return await context.StorageStateAsync();
            }
            catch (PlaywrightException failure)
            {
                failures.Add(failure);
            }
            finally
            {
                await context.CloseAsync();
            }
        }

        throw new AggregateException(
            $"Sign-in failed after {MaximumSignInAttempts} attempts.",
            failures
        );
    }

    /// <summary>
    /// Signs in on <paramref name="page"/>, returning only once sign-in has visibly succeeded.
    /// Throw a <see cref="PlaywrightException"/> for a failure worth retrying.
    /// </summary>
    protected abstract Task LoginAsync(IPage page);
}
