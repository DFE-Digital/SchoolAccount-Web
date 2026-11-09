using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.IntegrationTests.Common.Stubs;

/// <summary>
/// A command handler that returns whatever the test configures, in place of the real one.
/// </summary>
public class StubCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private Result? _result;
    private Exception? _exception;

    public void Returns(Result result) => _result = result;

    protected void Throws(Exception exception) => _exception = exception;

    public Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
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
}
