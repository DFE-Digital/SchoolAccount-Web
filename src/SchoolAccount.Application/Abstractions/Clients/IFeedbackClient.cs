using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Abstractions.Clients;

public interface IFeedbackClient
{
    Task<Result> SendFeedback(
        string message,
        string? ukprn,
        string organisationId,
        CancellationToken cancellationToken
    );
}
