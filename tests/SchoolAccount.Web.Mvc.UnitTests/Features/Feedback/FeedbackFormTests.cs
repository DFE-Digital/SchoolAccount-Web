using System.ComponentModel.DataAnnotations;
using SchoolAccount.Web.Mvc.Features.Feedback;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Features.Feedback;

public class FeedbackFormTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void A_missing_message_is_invalid(string? message)
    {
        // Arrange
        var form = new FeedbackForm { FeedbackMessage = message };

        // Act
        var errors = Validate(form);

        // Assert
        errors.ShouldBe(["Enter your feedback or describe the problem"]);
    }

    [Fact]
    public void A_message_over_the_maximum_length_is_invalid()
    {
        // Arrange
        var form = new FeedbackForm
        {
            FeedbackMessage = new string('a', FeedbackForm.MaximumMessageLength + 1),
        };

        // Act
        var errors = Validate(form);

        // Assert
        errors.ShouldBe(["Your feedback must be 32,000 characters or fewer"]);
    }

    [Fact]
    public void A_message_at_the_maximum_length_is_valid()
    {
        // Arrange
        var form = new FeedbackForm
        {
            FeedbackMessage = new string('a', FeedbackForm.MaximumMessageLength),
        };

        // Act
        var errors = Validate(form);

        // Assert
        errors.ShouldBeEmpty();
    }

    private static string?[] Validate(FeedbackForm form)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(form, new ValidationContext(form), results, true);

        return [.. results.Select(result => result.ErrorMessage)];
    }
}
