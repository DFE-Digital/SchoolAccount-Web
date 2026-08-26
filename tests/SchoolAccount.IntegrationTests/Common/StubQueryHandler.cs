using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// A query handler that returns whatever the test configures, in place of the real one.
/// </summary>
public class StubQueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private Result<TResponse>? _result;

    public void Returns(Result<TResponse> result) => _result = result;

    public Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken) =>
        Task.FromResult(
            _result
                ?? throw new InvalidOperationException(
                    $"No result configured for {typeof(TQuery).Name}. Call Returns before acting."
                )
        );
}
