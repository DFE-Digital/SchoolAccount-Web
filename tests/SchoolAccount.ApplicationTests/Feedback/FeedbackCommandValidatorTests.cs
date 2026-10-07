using FluentValidation.TestHelper;
using SchoolAccount.Application.Features.Feedback;

namespace SchoolAccount.ApplicationTests.Feedback;

public class FeedbackCommandValidatorTests
{
    private readonly FeedbackCommandValidator _validator = new();

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
