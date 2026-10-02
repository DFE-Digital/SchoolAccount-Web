using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Abstractions.Clients;

public interface IAzureTableClient
{
    Task<Result> SendFeedback(
        string message,
        string? ukprn,
        string organisationId,
        CancellationToken cancellationToken
    );
}
