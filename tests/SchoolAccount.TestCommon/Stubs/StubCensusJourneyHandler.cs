using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseBuilder;

namespace SchoolAccount.TestCommon.Stubs;

public sealed class StubCensusJourneyHandler
    : StubQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>
{
    public static StubCensusJourneyHandler Succeeding()
    {
        var handler = new StubCensusJourneyHandler();
        handler.Returns(AGetCensusJourneyResponse().AsSuccess());

        return handler;
    }
}
