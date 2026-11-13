using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.IntegrationTests.Common.Stubs;

/// <summary>
///     A query handler that returns whatever the test configures, in place of the real one.
/// </summary>
public class StubQueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private Exception? _exception;
    private Result<TResponse>? _result;

    public Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (_exception is not null)
        {
            throw _exception;
        }

        return Task.FromResult(
            _result
                ?? throw new InvalidOperationException(
                    $"{GetType().Name} was not configured. Call Returns or Throws."
                )
        );
    }

    protected void Returns(Result<TResponse> result)
    {
        _result = result;
    }

    protected void Throws(Exception exception)
    {
        _exception = exception;
    }
}
