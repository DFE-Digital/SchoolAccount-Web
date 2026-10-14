using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Stubs;

/// <summary>
/// A query handler that returns whatever the test configures, in place of the real one.
/// </summary>
public class StubQueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private Result<TResponse>? _result;
    private Exception? _exception;

    public void Returns(Result<TResponse> result)
    {
        _result = result;
        _exception = null;
    }

    public void Throws(Exception exception)
    {
        _exception = exception;
        _result = null;
    }

    public Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (_exception is not null)
        {
            throw _exception;
        }

        return Task.FromResult(
            _result
                ?? throw new InvalidOperationException(
                    $"No result configured for {typeof(TQuery).Name}. Call Returns before acting."
                )
        );
    }
}
