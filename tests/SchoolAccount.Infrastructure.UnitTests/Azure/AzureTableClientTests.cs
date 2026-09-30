using Azure;
using Azure.Data.Tables;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SchoolAccount.Infrastructure.Clients.Azure;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.Infrastructure.UnitTests.Azure;

public class AzureTableClientTests
{
    private readonly TableClient _tableClient = Substitute.For<TableClient>();

    [Fact]
    public async Task A_table_is_added_by_the_client_successfully()
    {
        // Arrange
        var message = "test-message";
        var ukprn = "test-ukprn";
        var laestab = "test-laestab";

        var client = new AzureTableClient(_tableClient);

        // Act
        var result = await client.SendFeedback(message, ukprn, laestab);

        // Assert
        await _tableClient
            .Received(1)
            .AddEntityAsync(Arg.Any<FeedbackModel>(), Arg.Any<CancellationToken>());
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Failure_returned_when_client_throws_exception()
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
        var result = await client.SendFeedback(message, ukprn, laestab);

        // Assert
        await _tableClient
            .Received(1)
            .AddEntityAsync(Arg.Any<FeedbackModel>(), Arg.Any<CancellationToken>());
        result.IsFailure.ShouldBeTrue();
    }
}
