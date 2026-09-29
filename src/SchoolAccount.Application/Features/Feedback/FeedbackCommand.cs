using Azure;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Features.Feedback;

public record FeedbackCommand : ICommand
{
    public string Message { get; init; }
    public string Ukprn { get; init; }
    public string? Laestab { get; init; }
}
