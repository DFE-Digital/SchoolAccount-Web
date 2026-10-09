using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Results;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Abstractions.Behaviours;

internal static class ValidationDecorator
{
    internal sealed class Command<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> next,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(
            TCommand command,
            CancellationToken cancellationToken
        )
        {
            var failures = await ValidateAsync(command, validators);

            if (failures.Length > 0)
            {
                return Result.Failure<TResponse>(CreateValidationError(failures));
            }

            return await next.Handle(command, cancellationToken);
        }
    }

    internal sealed class CommandHandler<TCommand>(
        ICommandHandler<TCommand> next,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var failures = await ValidateAsync(command, validators);

            if (failures.Length > 0)
            {
                return Result.Failure(CreateValidationError(failures));
            }

            return await next.Handle(command, cancellationToken);
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators
    )
    {
        var validatorsArray = validators as IValidator<TCommand>[] ?? validators.ToArray();

        if (validatorsArray.Length == 0)
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        var validationResults = await Task.WhenAll(
            validatorsArray.Select(v => v.ValidateAsync(context))
        );

        return validationResults.Where(r => !r.IsValid).SelectMany(r => r.Errors).ToArray();
    }

    private static ValidationError CreateValidationError(ValidationFailure[] failures) =>
        new([
            .. failures.Select(f => Error.Validation(f.ErrorCode, f.ErrorMessage, f.PropertyName)),
        ]);
}
