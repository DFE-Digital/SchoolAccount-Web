using System.Diagnostics.CodeAnalysis;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Abstractions.Messaging;

public delegate Task<Result<TResponse>> QueryHandlerDelegate<TResponse>();
public delegate Task<Result<TResponse>> CommandHandlerDelegate<TResponse>();
public delegate Task<Result> CommandHandlerDelegate();

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
public interface IQueryPipelineBehavior<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    );
}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
public interface ICommandPipelineBehavior<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    );
}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
public interface ICommandPipelineBehavior<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(
        TCommand command,
        CommandHandlerDelegate next,
        CancellationToken cancellationToken
    );
}
