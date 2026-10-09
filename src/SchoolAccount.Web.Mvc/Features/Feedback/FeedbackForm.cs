using System.ComponentModel.DataAnnotations;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

/// <summary>
/// The feedback form posted from the footer of any page.
/// </summary>
public sealed class FeedbackForm
{
    public const int MaximumMessageLength = 32000;

    [Required(ErrorMessage = "Enter your feedback or describe the problem")]
    [MaxLength(
        MaximumMessageLength,
        ErrorMessage = "Your feedback must be 32,000 characters or fewer"
    )]
    public string? FeedbackMessage { get; init; }

    public string? ReturnUrl { get; init; }
}
