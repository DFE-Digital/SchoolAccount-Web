using NSubstitute;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

public static class DateFormatterTests
{
    private static readonly IDateTimeProvider _dateTimeProvider =
        Substitute.For<IDateTimeProvider>();

    public class ToDaysAgo
    {
        [Fact]
        public void Today_is_returned_for_the_current_date()
        {
            // Arrange
            var twoAmSeptember14 = new DateTime(2026, 9, 14, 2, 0, 0, DateTimeKind.Utc);
            var nineAmSeptember14 = new DateTime(2026, 9, 14, 9, 0, 0, DateTimeKind.Utc);
            var dateFormatter = CreateDateFormatterAt(nineAmSeptember14);

            // Act
            var result = dateFormatter.ToDaysAgo(twoAmSeptember14);

            // Assert
            result.ShouldBe("Today");
        }

        [Fact]
        public void Day_singular_is_returned_for_one_day_ago()
        {
            // Arrange
            var twoAmSeptember14 = new DateTime(2026, 9, 14, 2, 0, 0, DateTimeKind.Utc);
            var nineAmSeptember15 = new DateTime(2026, 9, 15, 9, 0, 0, DateTimeKind.Utc);
            var dateFormatter = CreateDateFormatterAt(nineAmSeptember15);

            // Act
            var result = dateFormatter.ToDaysAgo(twoAmSeptember14);

            // Assert
            result.ShouldBe("1 day ago");
        }

        [Fact]
        public void An_offset_day_adds_a_day_to_the_days_ago_count()
        {
            // Arrange
            var twoAmSeptember15 = new DateTime(2026, 9, 15, 02, 0, 0, DateTimeKind.Utc);
            var nineAmSeptember15 = new DateTime(2026, 9, 15, 9, 0, 0, DateTimeKind.Utc);
            var dateFormatter = CreateDateFormatterAt(nineAmSeptember15);

            // Act
            var result = dateFormatter.ToDaysAgo(twoAmSeptember15, 1);

            // Assert
            result.ShouldBe("1 day ago");
        }

        [Fact]
        public void Days_plural_are_returned_for_more_than_one_day_ago()
        {
            // Arrange
            var twoAmSeptember15 = new DateTime(2026, 9, 15, 02, 0, 0, DateTimeKind.Utc);
            var nineAmSeptember17 = new DateTime(2026, 9, 17, 9, 0, 0, DateTimeKind.Utc);
            var dateFormatter = CreateDateFormatterAt(nineAmSeptember17);

            // Act
            var result = dateFormatter.ToDaysAgo(twoAmSeptember15);

            // Assert
            result.ShouldBe("2 days ago");
        }

        private static DateFormatter CreateDateFormatterAt(DateTime utcNow)
        {
            _dateTimeProvider.UtcNow.Returns(utcNow);
            return new DateFormatter(_dateTimeProvider);
        }
    }

    public class ToDayMonthNameAndYear
    {
        [Fact]
        public void Full_dates_are_returned_with_day_and_year_as_numbers_and_month_as_full_name()
        {
            // Arrange
            var halloween2026 = DateOnly.FromDateTime(
                new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc)
            );
            var dateFormatter = new DateFormatter(_dateTimeProvider);

            // Act
            var result = dateFormatter.ToDayMonthNameAndYear(halloween2026);

            // Assert
            result.ShouldBe("31 October 2026");
        }
    }
}
