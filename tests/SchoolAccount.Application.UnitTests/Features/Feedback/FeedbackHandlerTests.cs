using NSubstitute;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.Application.UnitTests.Features.Feedback;

public class FeedbackHandlerTests
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly IFeedbackClient _azureTableClient = Substitute.For<IFeedbackClient>();

    [Fact]
    public async Task Command_values_are_passed_to_the_client()
    {
        // Arrange
        var command = CreateFeedbackCommand();
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
    public async Task Feedback_is_sent_successfully_when_the_client_succeeds()
    {
        // Arrange
        var command = CreateFeedbackCommand();
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

    [Fact]
    public async Task A_failure_is_reported_when_the_client_fails()
    {
        // Arrange
        var command = CreateFeedbackCommand();

        var error = Error.Failure("Azure Table Client", "Failed to send feedback: test-exception");

        _azureTableClient
            .SendFeedback(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Failure(error));
        var handler = new FeedbackHandler(_azureTableClient);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    private static FeedbackCommand CreateFeedbackCommand()
    {
        return new FeedbackCommand
        {
            Message = "test-feedback",
            Ukprn = "test-ukprn",
            OrganisationId = "test-id",
        };
    }
}
