using System.Globalization;

namespace SchoolAccount.E2ETests.Common;

public static class TestArtifactPaths
{
    public static string VideoDirectory()
    {
        var directory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "videos",
            CurrentTest.ClassName
        );
        Directory.CreateDirectory(directory);
        return directory;
    }

    public static string ScreenshotFilePath()
    {
        var directory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "screenshots",
            CurrentTest.ClassName
        );
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"{CurrentTest.MethodName}_{Timestamp()}.png");
    }

    public static string VideoFilePath() =>
        Path.Combine(VideoDirectory(), $"{CurrentTest.MethodName}_{Timestamp()}.webm");

    private static string Timestamp() =>
        DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
}
