using NSubstitute;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using Shouldly;
using Xunit.Sdk;

namespace SchoolAccount.ApplicationTests.Feedback;

public class FeedbackHandlerTests
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly IAzureTableClient _azureTableClient = Substitute.For<IAzureTableClient>();

    [Fact]
    public async Task Command_values_are_passed_to_the_client()
    {
        // Arrange
        var command = new FeedbackCommand
        {
            Message = "test-feedback",
            Ukprn = "test-ukprn",
            OrganisationId = "test-id",
        };
        var handler = new FeedbackHandler(_azureTableClient);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        await _azureTableClient
            .Received(1)
            .SendFeedback(
                command.Message,
                command.Ukprn,
                command.OrganisationId,
                _cancellationToken
            );
    }

    [Fact]
    public async Task Client_action_returns_success()
    {
        // Arrange
        var command = new FeedbackCommand
        {
            Message = "test-feedback",
            Ukprn = "test-ukprn",
            OrganisationId = "test-id",
        };
        _azureTableClient
            .SendFeedback(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success);
        var handler = new FeedbackHandler(_azureTableClient);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
