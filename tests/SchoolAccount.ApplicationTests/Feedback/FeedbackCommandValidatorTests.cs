using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using SchoolAccount.Application;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Feedback;

public class FeedbackCommandValidatorTests
{
    private readonly FeedbackCommandValidator _validator = new();

    [Fact]
    public void Validator_is_registered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        // Act
        var validator = provider.GetRequiredService<IValidator<FeedbackCommand>>();

        // Assert
        validator.ShouldNotBeNull();
        validator.ShouldBeOfType<FeedbackCommandValidator>();
    }

    [Fact]
    public async Task Invalid_command_does_not_reach_client()
    {
        // Arrange
        var client = Substitute.For<IFeedbackClient>();
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddSingleton(client);
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<FeedbackCommand>>();

        var command = new FeedbackCommand { Message = string.Empty, OrganisationId = "test-id" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        await client
            .DidNotReceive()
            .SendFeedback(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Valid_command_reaches_client()
    {
        // Arrange
        var client = Substitute.For<IFeedbackClient>();
        client
            .SendFeedback(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success);
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddSingleton(client);
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<FeedbackCommand>>();

        var command = new FeedbackCommand
        {
            Message = "test-message",
            OrganisationId = "test-id",
            Ukprn = "test-ukprn",
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await client
            .Received(1)
            .SendFeedback(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public void Validation_error_when_message_is_empty()
    {
        // Arrange
        var command = new FeedbackCommand { Message = string.Empty, OrganisationId = "test-id" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Validation_error_when_message_is_over_maximum_length()
    {
        // Arrange
        var command = new FeedbackCommand
        {
            Message = new string('a', 32001),
            OrganisationId = "test-id",
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Validation_error_when_organisationid_is_empty()
    {
        // Arrange
        var command = new FeedbackCommand { Message = "test-message", OrganisationId = "" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrganisationId);
    }
}
