namespace SchoolAccount.E2ETests.Common.Configuration;

public sealed class CaptureSettings
{
    public bool Enabled { get; set; } = true;

    public bool CaptureOnSuccess { get; set; } = true;

    public bool CaptureOnFailure { get; set; } = true;

    public bool ShouldCapture(bool testFailed) =>
        Enabled && (testFailed ? CaptureOnFailure : CaptureOnSuccess);
}
