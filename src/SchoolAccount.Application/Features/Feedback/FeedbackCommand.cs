using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Features.Feedback;

public record FeedbackCommand : ICommand
{
    public string Message { get; init; }
    public string? Ukprn { get; init; }
    public string OrganisationId { get; init; }
}
