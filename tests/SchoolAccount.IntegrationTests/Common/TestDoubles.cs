using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Stubs;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Everything a test can change about the running app. The shared host resolves these on every
/// request, so a test configures them instead of building a new host with its own services.
/// </summary>
public sealed class TestDoubles
{
    // No default results: every test sets what these return, and a test that forgets fails with
    // "No result configured".
    public StubQueryHandler<
        GetCensusStatusesQuery,
        List<GetCensusStatusesResponse>
    > CensusStatuses { get; } = new();

    public StubQueryHandler<
        GetCensusJourneyQuery,
        GetCensusJourneyResponse
    > CensusJourney { get; } = new();

    public FakeDateTimeProvider DateTimeProvider { get; } = new();
}

public sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;
}
