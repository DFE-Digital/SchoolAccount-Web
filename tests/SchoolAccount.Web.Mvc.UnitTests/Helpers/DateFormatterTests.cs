using NSubstitute;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

// One nested class per behaviour. No method names appear in class or
// test names, so renaming a method never leaves a name out of date.
public static class DateFormatterTests
{
    public class ShowingHowLongAgo
    {
        private static readonly DateTime TwoAmSeptember14 = new(
            2026,
            9,
            14,
            2,
            0,
            0,
            DateTimeKind.Utc
        );

        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        private readonly DateFormatter _dateFormatter;

        public ShowingHowLongAgo()
        {
            _dateFormatter = new DateFormatter(_dateTimeProvider);
        }

        [Theory]
        [InlineData(7, "Today")]
        [InlineData(31, "1 day ago")]
        [InlineData(55, "2 days ago")]
        public void It_counts_the_whole_days_since_the_date(int hoursLater, string expected)
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(TwoAmSeptember14.AddHours(hoursLater));

            // Act
            var result = _dateFormatter.ToDaysAgo(TwoAmSeptember14);

            // Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void An_offset_adds_days_to_the_count()
        {
            // Arrange
            _dateTimeProvider.UtcNow.Returns(TwoAmSeptember14.AddHours(7));

            // Act
            var result = _dateFormatter.ToDaysAgo(TwoAmSeptember14, offsetDays: 1);

            // Assert
            result.ShouldBe("1 day ago");
        }
    }

    public class ShowingAFullDate
    {
        private readonly DateFormatter _dateFormatter = new(Substitute.For<IDateTimeProvider>());

        [Fact]
        public void The_month_is_written_in_full_and_the_day_and_year_as_numbers()
        {
            // Arrange
            var halloween2026 = new DateOnly(2026, 10, 31);

            // Act
            var result = _dateFormatter.ToDayMonthNameAndYear(halloween2026);

            // Assert
            result.ShouldBe("31 October 2026");
        }
    }
}
