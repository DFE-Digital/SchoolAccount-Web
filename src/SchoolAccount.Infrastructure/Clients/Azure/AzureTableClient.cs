using Azure;
using Azure.Data.Tables;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Infrastructure.Clients.Azure;

public class AzureTableClient(TableClient tableClient) : IFeedbackClient
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
            var feedback = new FeedbackModel
            {
                PartitionKey = organisationId,
                RowKey = Guid.NewGuid().ToString(),
                Ukprn = ukprn,
                Message = message,
            };

            await tableClient.AddEntityAsync(feedback, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Failure("Azure Table Client", $"Failed to send feedback: {ex.Message}")
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
