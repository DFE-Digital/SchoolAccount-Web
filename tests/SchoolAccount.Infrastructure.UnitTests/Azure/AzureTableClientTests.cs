using Azure;
using Azure.Data.Tables;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Infrastructure.Clients.Azure;
using Shouldly;

namespace SchoolAccount.Infrastructure.UnitTests.Azure;

public class AzureTableClientTests
{
    private readonly TableClient _tableClient = Substitute.For<TableClient>();

    [Fact]
    public async Task Feedback_is_saved_to_table_storage()
    {
        // Arrange
        var message = "test-message";
        var ukprn = "test-ukprn";
        var laestab = "test-laestab";

        var client = new AzureTableClient(_tableClient);

        // Act
        var result = await client.SendFeedback(message, ukprn, laestab, CancellationToken.None);

        // Assert
        await _tableClient
            .Received(1)
            .AddEntityAsync(Arg.Any<FeedbackModel>(), Arg.Any<CancellationToken>());
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task A_storage_error_is_returned_as_a_failure()
    {
        // Arrange
        var message = "test-message";
        var ukprn = "test-ukprn";
        var laestab = "test-laestab";

        _tableClient
            .AddEntityAsync(Arg.Any<FeedbackModel>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new RequestFailedException(""));
        var client = new AzureTableClient(_tableClient);

        // Act
        var result = await client.SendFeedback(message, ukprn, laestab, CancellationToken.None);

        // Assert
        await _tableClient
            .Received(1)
            .AddEntityAsync(Arg.Any<FeedbackModel>(), Arg.Any<CancellationToken>());
        result.IsFailure.ShouldBeTrue();
    }
}
