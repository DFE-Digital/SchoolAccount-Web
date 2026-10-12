using NSubstitute;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

public class DateFormatterTests
{
    [Fact]
    public void ToDaysAgo_returns_today_for_same_day()
    {
        // Arrange
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var twoAmSeptember14 = new DateTime(2026, 9, 14, 2, 0, 0, DateTimeKind.Utc);
        var nineAmSeptember14 = new DateTime(2026, 9, 14, 9, 0, 0, DateTimeKind.Utc);
        dateTimeProvider.UtcNow.Returns(nineAmSeptember14);
        var dateFormatter = new DateFormatter(dateTimeProvider);

        // Act
        var result = dateFormatter.ToDaysAgo(twoAmSeptember14);

        // Assert
        result.ShouldBe("Today");
    }

    [Fact]
    public void ToDaysAgo_returns_day_singular_for_one_day_ago()
    {
        // Arrange
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var twoAmSeptember14 = new DateTime(2026, 9, 14, 2, 0, 0, DateTimeKind.Utc);
        var nineAmSeptember15 = new DateTime(2026, 9, 15, 9, 0, 0, DateTimeKind.Utc);
        dateTimeProvider.UtcNow.Returns(nineAmSeptember15);
        var dateFormatter = new DateFormatter(dateTimeProvider);

        // Act
        var result = dateFormatter.ToDaysAgo(twoAmSeptember14);

        // Assert
        result.ShouldBe("1 day ago");
    }

    [Fact]
    public void ToDaysAgo_applies_offset_days()
    {
        // Arrange
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var twoAmSeptember15 = new DateTime(2026, 9, 15, 02, 0, 0, DateTimeKind.Utc);
        var nineAmSeptember15 = new DateTime(2026, 9, 15, 9, 0, 0, DateTimeKind.Utc);
        dateTimeProvider.UtcNow.Returns(nineAmSeptember15);
        var dateFormatter = new DateFormatter(dateTimeProvider);

        // Act
        var result = dateFormatter.ToDaysAgo(twoAmSeptember15, 1);

        // Assert
        result.ShouldBe("1 day ago");
    }

    [Fact]
    public void ToDaysAgo_returns_days_plural_for_more_than_one_day_ago()
    {
        // Arrange
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var twoAmSeptember15 = new DateTime(2026, 9, 15, 02, 0, 0, DateTimeKind.Utc);
        var nineAmSeptember17 = new DateTime(2026, 9, 17, 9, 0, 0, DateTimeKind.Utc);
        dateTimeProvider.UtcNow.Returns(nineAmSeptember17);
        var dateFormatter = new DateFormatter(dateTimeProvider);

        // Act
        var result = dateFormatter.ToDaysAgo(twoAmSeptember15);

        // Assert
        result.ShouldBe("2 days ago");
    }

    [Fact]
    public void ToDaysMonthNameAndYear_returns_month_as_full_name_and_day_and_year_as_numbers()
    {
        // Arrange
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var halloween2026 = DateOnly.FromDateTime(
            new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc)
        );
        var dateFormatter = new DateFormatter(dateTimeProvider);

        // Act
        var result = dateFormatter.ToDayMonthNameAndYear(halloween2026);

        // Assert
        result.ShouldBe("31 October 2026");
    }
}
