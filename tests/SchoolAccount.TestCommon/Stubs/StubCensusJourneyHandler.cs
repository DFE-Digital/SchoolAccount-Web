using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.TestCommon.Builders;
using SchoolAccount.TestCommon.Builders.GetCensusJourney;

namespace SchoolAccount.TestCommon.Stubs;

public sealed class StubCensusJourneyHandler
    : StubQueryHandler<GetCensusJourneyQuery, GetCensusJourneyContentResponse>
{
    public static StubCensusJourneyHandler Succeeding()
    {
        var handler = new StubCensusJourneyHandler();
        handler.Returns(CensusJourneyResponseBuilder.ACensusJourneyResponse().AsSuccess());

        return handler;
    }
}
