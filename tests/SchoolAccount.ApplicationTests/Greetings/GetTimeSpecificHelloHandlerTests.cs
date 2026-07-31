using NSubstitute;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.SharedKernel;
using Shouldly;
using static SchoolAccount.Application.Greetings.GetTimeSpecificHello.GetTimeSpecificHelloHandler.Messages;

namespace SchoolAccount.ApplicationTests.Greetings;

public class GetTimeSpecificHelloHandlerTests
{
    [Fact]
    public async Task Good_morning_message_after_5am_before_12pm()
    {
        // Arrange
        IDateTimeProvider dateTimeProvider = SubstituteDateTimeProvider(5);
        var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
            new GetTimeSpecificHelloQuery(),
            CancellationToken.None
        );

        // Arrange
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(Morning);
    }

    [Fact]
    public async Task Good_afternoon_message_after_12pm_before_5pm()
    {
        // Arrange
        IDateTimeProvider dateTimeProvider = SubstituteDateTimeProvider(12);
        var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
            new GetTimeSpecificHelloQuery(),
            CancellationToken.None
        );

        // Arrange
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(Afternoon);
    }

    [Fact]
    public async Task Good_evening_message_after_5pm_before_10pm()
    {
        // Arrange
        IDateTimeProvider dateTimeProvider = SubstituteDateTimeProvider(18);
        var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);

        // Act
        Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
            new GetTimeSpecificHelloQuery(),
            CancellationToken.None
        );

        // Arrange
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe(Evening);
    }

    // [Fact]
    // public async Task Good_night_message_after_10pm_before_5am()
    // {
    //     // Arrange
    //     IDateTimeProvider dateTimeProvider = SubstituteDateTimeProvider(23);
    //     var handler = new GetTimeSpecificHelloHandler(dateTimeProvider);
    //
    //     // Act
    //     Result<GetTimeSpecificHelloResponse> result = await handler.Handle(
    //         new GetTimeSpecificHelloQuery(),
    //         CancellationToken.None
    //     );
    //
    //     // Arrange
    //     result.IsSuccess.ShouldBeTrue();
    //     result.Value.Message.ShouldBe(Night);
    // }

    private static IDateTimeProvider SubstituteDateTimeProvider(int hour)
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
