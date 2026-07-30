using NSubstitute;
using SchoolAccount.Application.Greetings.GetTimeSpecifyHellos;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Trueman;

public class GetTimeSpecificHelloHandlerTests
{
    [Fact]
    public async Task Handler_provides_correct_hello_message_after_5am_before_12pm()
    {
        // Arrange
        IDateTimeProvider dateTimeProvider = EmulateDateTimeProvider(5);
        var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
            new GetTimeSpecificHelloQuery(),
            CancellationToken.None
        );

        // Arrange
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(GetTimeSpecificHelloHandler.Messages.Morning);
    }

    [Fact]
    public async Task Handler_provides_correct_hello_message_after_12pm_before_5pm()
    {
        // Arrange
        IDateTimeProvider dateTimeProvider = EmulateDateTimeProvider(12);
        var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
            new GetTimeSpecificHelloQuery(),
            CancellationToken.None
        );

        // Arrange
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(GetTimeSpecificHelloHandler.Messages.Afternoon);
    }

    [Fact]
    public async Task Handler_provides_correct_hello_message_after_5pm_before_10pm()
    {
        // Arrange
        IDateTimeProvider dateTimeProvider = EmulateDateTimeProvider(18);
        var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
            new GetTimeSpecificHelloQuery(),
            CancellationToken.None
        );

        // Arrange
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(GetTimeSpecificHelloHandler.Messages.Evening);
    }

    [Fact]
    public async Task Handler_provides_correct_hello_message_after_10pm_before_5am()
    {
        // Arrange
        IDateTimeProvider dateTimeProvider = EmulateDateTimeProvider(23);
        var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
            new GetTimeSpecificHelloQuery(),
            CancellationToken.None
        );

        // Arrange
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(GetTimeSpecificHelloHandler.Messages.Night);
    }

    private static IDateTimeProvider EmulateDateTimeProvider(int hour)
    {
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

        return dateTimeProvider;
    }
}
