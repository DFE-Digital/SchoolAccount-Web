using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common;
using SchoolAccount.E2ETests.Common.Configuration;

namespace SchoolAccount.E2ETests.Common.Fixtures;

public abstract class BrowserTestBase : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;
    private IBrowserContext? _context;
    private IPage? _page;

    protected BrowserTestBase(BrowserFixture fixture)
    {
        _fixture = fixture;
    }

    protected IPage Page =>
        _page
        ?? throw new InvalidOperationException(
            "The page is only available once the test has started."
        );

    protected TestSettings Settings => _fixture.Settings;

    public async ValueTask InitializeAsync()
    {
        var contextOptions = new BrowserNewContextOptions();
        if (_fixture is AuthenticatedBrowserFixture { StorageState: { } storageState })
        {
            contextOptions.StorageState = storageState;
        }

        if (Settings.Video.Enabled)
        {
            contextOptions.RecordVideoDir = TestArtifactPaths.VideoDirectory();
        }

        _context = await _fixture.Browser.NewContextAsync(contextOptions);
        _page = await _context.NewPageAsync();

        await OnInitializedAsync();
    }

    /// <summary>Extension point for derived test classes that need to do something (e.g. navigate, build page objects) once Page is available.</summary>
    protected virtual Task OnInitializedAsync() => Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        // Nothing to tidy up if setup failed before the browser context was created; the setup
        // failure is the one reported.
        if (_context is null)
        {
            return;
        }

        try
        {
            await CaptureScreenshotIfNecessaryAsync();
        }
        finally
        {
            // Playwright only finishes writing the video file once the context closes.
            await _context.CloseAsync();

            await HandleVideoIfNecessaryAsync();
        }
    }

    private async Task CaptureScreenshotIfNecessaryAsync()
    {
        if (_page is null || !Settings.Screenshot.ShouldCapture(CurrentTest.Failed))
        {
            return;
        }

        await _page.ScreenshotAsync(
            new PageScreenshotOptions
            {
                Path = TestArtifactPaths.ScreenshotFilePath(),
                FullPage = true,
            }
        );
    }

    private async Task HandleVideoIfNecessaryAsync()
    {
        if (_page?.Video is not { } video)
        {
            return;
        }

        var recordedVideoPath = await video.PathAsync();

        if (Settings.Video.ShouldCapture(CurrentTest.Failed))
        {
            File.Move(recordedVideoPath, TestArtifactPaths.VideoFilePath());
        }
        else
        {
            File.Delete(recordedVideoPath);
        }
    }
}
