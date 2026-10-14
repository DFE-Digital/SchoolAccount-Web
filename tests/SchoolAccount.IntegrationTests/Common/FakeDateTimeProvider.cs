using SchoolAccount.SharedKernel;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// The real clock by default. Set <see cref="UtcNow"/> to fix the time for a test.
/// </summary>
public sealed class FakeDateTimeProvider : IDateTimeProvider
{
    private DateTime? _utcNow;

    public DateTime UtcNow
    {
        get => _utcNow ?? DateTime.UtcNow;
        set => _utcNow = value;
    }

    public void UseRealClock() => _utcNow = null;
}
