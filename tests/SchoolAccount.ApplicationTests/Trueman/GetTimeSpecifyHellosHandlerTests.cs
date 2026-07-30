using NSubstitute;
using SchoolAccount.Application.Greetings.GetTimeSpecifyHellos;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Trueman;

public class GetTimeSpecifyHellosHandlerTests
{
    [Theory]
    [InlineData(5, "Good morning")]
    [InlineData(12, "Good afternoon")]
    [InlineData(18, "Good evening")]
    [InlineData(23, "Good night (go to bed)")]
    public async Task Handler_provides_correct_hello_message_depending_on_time(
        int hour,
        string expectedMessage
    )
    {
        // Arrange
        var date = new DateTime(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month,
            DateTime.UtcNow.Day,
            hour,
            0,
            0,
            DateTimeKind.Utc
        );

        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(date);

        var handler = new GetTimeSpecifyHellosHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecifyHellosResponse> result = await handler.Handle(
            new GetTimeSpecifyHellosQuery(),
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(expectedMessage);
    }
}
