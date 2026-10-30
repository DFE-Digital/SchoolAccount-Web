using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SchoolAccount.Application;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Abstractions.Behaviours;

public class ValidationDecoratorTests
{
    [Fact]
    public async Task Invalid_command_with_a_response_does_not_reach_its_handler()
    {
        // Arrange
        var handler = Substitute.For<ICommandHandler<TestCommand, int>>();
        var services = new ServiceCollection();
        services.AddScoped(_ => handler);
        services.AddScoped<IValidator<TestCommand>, TestCommandValidator>();
        services.AddLogging();
        services.AddApplication();
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var decorated = scope.ServiceProvider.GetRequiredService<
            ICommandHandler<TestCommand, int>
        >();

        // Act
        var result = await decorated.Handle(
            new TestCommand { Name = string.Empty },
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeOfType<ValidationError>();
        await handler.DidNotReceive().Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Valid_command_with_a_response_reaches_its_handler_and_returns_its_value()
    {
        // Arrange
        var handler = Substitute.For<ICommandHandler<TestCommand, int>>();
        handler
            .Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(42));
        var services = new ServiceCollection();
        services.AddScoped(_ => handler);
        services.AddScoped<IValidator<TestCommand>, TestCommandValidator>();
        services.AddLogging();
        services.AddApplication();
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var decorated = scope.ServiceProvider.GetRequiredService<
            ICommandHandler<TestCommand, int>
        >();

        // Act
        var result = await decorated.Handle(
            new TestCommand { Name = "test-name" },
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
        await handler.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Validators_receive_the_cancellation_token()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var handler = Substitute.For<ICommandHandler<TestCommand, int>>();
        handler
            .Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(1));
        var validator = Substitute.For<IValidator<TestCommand>>();
        validator
            .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        var services = new ServiceCollection();
        services.AddScoped(_ => handler);
        services.AddScoped(_ => validator);
        services.AddLogging();
        services.AddApplication();
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var decorated = scope.ServiceProvider.GetRequiredService<
            ICommandHandler<TestCommand, int>
        >();

        // Act
        await decorated.Handle(new TestCommand { Name = "test-name" }, cancellation.Token);

        // Assert
        await validator
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
}
