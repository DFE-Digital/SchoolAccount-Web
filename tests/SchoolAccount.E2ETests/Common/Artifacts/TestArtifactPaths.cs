using System.Globalization;

namespace SchoolAccount.E2ETests.Common.Artifacts;

/// <summary>
/// Where screenshots and videos are saved: a folder per test class under the output directory,
/// with each file named after the test and the time it ran.
/// </summary>
public static class TestArtifactPaths
{
    private const string _screenshotsFolder = "screenshots";
    private const string _videosFolder = "videos";
    private const string _recordingFolder = "recording";

    public static string ScreenshotFilePath()
    {
        return BuildFilePath(_screenshotsFolder, ".png");
    }

    public static string VideoDirectory()
    {
        return Path.Combine(AppContext.BaseDirectory, _videosFolder, _recordingFolder);
    }

    public static string VideoFilePath()
    {
        return BuildFilePath(_videosFolder, ".webm");
    }

    private static string BuildFilePath(string folder, string extension)
    {
        var classFolder = Path.Combine(AppContext.BaseDirectory, folder, CurrentTest.ClassName);
        Directory.CreateDirectory(classFolder);

        var timestamp = DateTimeOffset.UtcNow.ToString(
            "yyyyMMdd-HHmmss",
            CultureInfo.InvariantCulture
        );

        return Path.Combine(classFolder, $"{CurrentTest.MethodName}_{timestamp}{extension}");
    }
}
