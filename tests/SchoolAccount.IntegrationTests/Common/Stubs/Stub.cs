using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;

namespace SchoolAccount.IntegrationTests.Common.Stubs;

/// <summary>
/// Creates NSubstitute handler substitutes with no default result. Until a test sets what a
/// handler returns, calling it fails with "No result configured", so a test can't pass on data it
/// never mentions.
/// </summary>
public static class Stub
{
    public static CensusJourneyHandler CensusJourneyHandler()
    {
        return QueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>();
    }

    private static IQueryHandler<TQuery, TResponse> QueryHandler<TQuery, TResponse>()
        where TQuery : IQuery<TResponse>
    {
        var handler = Substitute.For<IQueryHandler<TQuery, TResponse>>();

        // A faulted task rather than a thrown exception, so a test can still set its own result
        // with Handle(...).Returns(...) without triggering this.
        handler.Handle(default!, default).ThrowsAsyncForAnyArgs(NoResultConfigured<TQuery>());

        return handler;
    }

    private static InvalidOperationException NoResultConfigured<TMessage>()
    {
        return new InvalidOperationException(
            $"No result configured for {typeof(TMessage).Name}. Set what Handle returns before acting."
        );
    }
}
