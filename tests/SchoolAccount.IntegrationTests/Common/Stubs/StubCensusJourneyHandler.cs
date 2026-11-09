using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.IntegrationTests.Common.Builders.Features.Collect.GetCensusJourney.Responses;
using static SchoolAccount.IntegrationTests.Common.Builders.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyResponseBuilder;

namespace SchoolAccount.IntegrationTests.Common.Stubs;

public sealed class StubCensusJourneyHandler
    : StubQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>
{
    public static StubCensusJourneyHandler Succeeding(
        GetCensusJourneyResponseBuilder? response = null
    )
    {
        var handler = new StubCensusJourneyHandler();
        handler.Returns((response ?? AGetCensusJourneyResponse()).AsSuccess());

        return handler;
    }

    public static StubCensusJourneyHandler Failing(Exception? exception = null)
    {
        var handler = new StubCensusJourneyHandler();
        handler.Throws(exception ?? new ApplicationException("Bang!"));

        return handler;
    }
}
