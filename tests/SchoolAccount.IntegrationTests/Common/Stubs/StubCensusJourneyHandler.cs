using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using static SchoolAccount.TestCommon.Builders.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyResponseBuilder;

namespace SchoolAccount.IntegrationTests.Common.Stubs;

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
