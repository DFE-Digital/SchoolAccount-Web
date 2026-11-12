// Short names for the handler types that tests substitute, so test fields stay readable.
global using CensusJourneyHandler = SchoolAccount.Application.Abstractions.Messaging.IQueryHandler<
    SchoolAccount.Application.Features.Collect.GetCensusJourney.GetCensusJourneyQuery,
    SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyResponse
>;
