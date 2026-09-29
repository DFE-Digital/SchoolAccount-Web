using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.SharedKernel;
using Serilog;

namespace SchoolAccount.Infrastructure.Clients.Blob;

public class AzureTableClient : IAzureTableClient
{
    public async Task<Result> SendFeedback(string message, string ukprn, string? laestab)
    {
        try
        {
            var tableClient = new TableClient(
                "DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;",
                "FeedbackTable"
            );

            await tableClient.CreateIfNotExistsAsync();

            var now = DateTime.UtcNow;
            var feedback = new FeedbackModel
            {
                PartitionKey = ukprn,
                RowKey = $"{DateTime.MaxValue.Ticks - now.Ticks:D19}_{Guid.NewGuid():N}",
                Laestab = laestab,
                Message = message,
                Timestamp = now,
            };

            var response = await tableClient.AddEntityAsync(feedback);

            return Result.Success(response.Status);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send feedback");
            return Result.Failure<RequestFailedException>(
                Error.Failure("Azure Blob Client", "Failed to send feedback")
            );
        }
    }
}

public class FeedbackModel : ITableEntity
{
    public string Message { get; init; }
    public string? Laestab { get; init; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
