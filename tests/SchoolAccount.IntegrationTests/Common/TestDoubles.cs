using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Stubs;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Everything a test can change about the running app. The shared host resolves these on every
/// request, so a test configures them instead of building a new host with its own services.
/// </summary>
public sealed class TestDoubles
{
    public StubCensusStatusesHandler CensusStatuses { get; } =
        StubCensusStatusesHandler.Succeeding();

    public StubCensusJourneyHandler CensusJourney { get; } = StubCensusJourneyHandler.Succeeding();

    public FakeDateTimeProvider DateTimeProvider { get; } = new();
}

public sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;
}
