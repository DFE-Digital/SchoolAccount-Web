using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.TestCommon.Builders;

namespace SchoolAccount.TestCommon.Stubs;

public sealed class StubCensusStatusesHandler
    : StubQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
{
    /// <summary>
    /// For tests that only need the journey to render and do not care what it renders.
    /// </summary>
    public static StubCensusStatusesHandler Succeeding()
    {
        var handler = new StubCensusStatusesHandler();
        handler.Returns(CensusStatusesResponseBuilder.ACensusStatusResponse().AsSuccess());

        return handler;
    }
}
