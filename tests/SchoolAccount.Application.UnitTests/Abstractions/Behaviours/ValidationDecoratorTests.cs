using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.Application.UnitTests.Abstractions.Behaviours;

public class ValidationDecoratorTests
{
    private readonly ICommandHandler<TestCommand, int> _handler = Substitute.For<
        ICommandHandler<TestCommand, int>
    >();
    private readonly IValidator<TestCommand> _validator = Substitute.For<IValidator<TestCommand>>();

    [Fact]
    public async Task Invalid_command_with_a_response_does_not_reach_its_handler()
    {
        // Arrange
        var decorated = CreateDecoratedHandler(new TestCommandValidator());

        // Act
        var result = await decorated.Handle(
            new TestCommand { Name = string.Empty },
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeOfType<ValidationError>();
        await _handler.DidNotReceive().Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Valid_command_with_a_response_reaches_its_handler_and_returns_its_value()
    {
        // Arrange
        _handler
            .Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(42));
        var decorated = CreateDecoratedHandler(new TestCommandValidator());

        // Act
        var result = await decorated.Handle(
            new TestCommand { Name = "test-name" },
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
        await _handler.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Validators_receive_the_cancellation_token()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        _handler
            .Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(1));
        _validator
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        var decorated = CreateDecoratedHandler(_validator);

        // Act
        await decorated.Handle(new TestCommand { Name = "test-name" }, cancellation.Token);

        // Assert
        await _validator
            .Received(1)
            .ValidateAsync(Arg.Any<IValidationContext>(), cancellation.Token);
    }

    public sealed record TestCommand : ICommand<int>
    {
        public required string Name { get; init; }
    }

    public sealed class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private ICommandHandler<TestCommand, int> CreateDecoratedHandler(
        IValidator<TestCommand> validator
    )
    {
        var services = new ServiceCollection();

        services.AddScoped(_ => _handler);
        services.AddScoped(_ => validator);
        services.AddLogging();
        services.AddApplication();

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICommandHandler<TestCommand, int>>();
    }
}
