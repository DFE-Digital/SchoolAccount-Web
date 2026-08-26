using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.IntegrationTests.Common.Stubs;

namespace SchoolAccount.IntegrationTests.Common.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Replaces the real query handler with <paramref name="handler"/>. The type arguments are
    /// inferred from it, so the closed generic handler type stays out of the test.
    /// </summary>
    public static IServiceCollection StubQueryHandler<TQuery, TResponse>(
        this IServiceCollection services,
        StubQueryHandler<TQuery, TResponse> handler
    )
        where TQuery : IQuery<TResponse>
    {
        services.RemoveAll<IQueryHandler<TQuery, TResponse>>();
        return services.AddScoped<IQueryHandler<TQuery, TResponse>>(_ => handler);
    }
}
