using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Infrastructure.Clients.Azure;

public class AzureTableClient(TableClient tableClient, ILogger<AzureTableClient> logger)
    : IFeedbackClient
{
    public async Task<Result> SendFeedback(
        string message,
        string? ukprn,
        string organisationId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var now = DateTime.UtcNow;
            var feedback = new FeedbackModel
            {
                PartitionKey = organisationId,
                RowKey = Guid.NewGuid().ToString(),
                Ukprn = ukprn,
                Message = message,
                Timestamp = now,
            };

            var response = await tableClient.AddEntityAsync(feedback, cancellationToken);

            return Result.Success(response.Status);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send feedback");
            return Result.Failure<RequestFailedException>(
                Error.Failure("Azure Table Client", "Failed to send feedback")
            );
        }
    }
}

public class FeedbackModel : ITableEntity
{
    public string Message { get; init; }
    public string? Ukprn { get; init; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
