using FluentValidation;
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
        services.AddApplication();
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var decorated = scope.ServiceProvider.GetRequiredService<
            ICommandHandler<TestCommand, int>
        >();

        // Act
        var result = await decorated.Handle(
            new TestCommand { Name = string.Empty },
            CancellationToken.None
        );

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeOfType<ValidationError>();
        await handler
            .DidNotReceive()
            .Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
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
