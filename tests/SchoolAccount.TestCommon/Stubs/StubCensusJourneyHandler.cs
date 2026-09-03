using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.TestCommon.Builders;
using SchoolAccount.TestCommon.Builders.GetCensusJourney;

namespace SchoolAccount.TestCommon.Stubs;

public sealed class StubCensusJourneyHandler
    : StubQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>
{
    public static StubCensusJourneyHandler Succeeding()
    {
        var handler = new StubCensusJourneyHandler();
        handler.Returns(CensusJourneyResponseBuilder.Create().AsSuccess());

        return handler;
    }
}
