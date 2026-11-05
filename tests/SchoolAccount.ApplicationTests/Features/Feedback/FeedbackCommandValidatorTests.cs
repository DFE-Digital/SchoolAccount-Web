using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SchoolAccount.Application;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Features.Feedback;

public class FeedbackCommandValidatorTests
{
    private readonly FeedbackCommandValidator _validator = new();

    [Fact]
    public void An_empty_message_is_invalid()
    {
        // Arrange
        var command = new FeedbackCommand { Message = string.Empty, OrganisationId = "test-id" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void A_message_over_the_maximum_character_limit_is_invalid()
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
    public void Feedback_without_an_organisation_is_invalid()
    {
        // Arrange
        var command = new FeedbackCommand { Message = "test-message", OrganisationId = "" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrganisationId);
    }
}
