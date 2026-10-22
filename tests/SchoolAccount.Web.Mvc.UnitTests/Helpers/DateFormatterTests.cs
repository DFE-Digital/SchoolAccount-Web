
using NSubstitute;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

// Each test is labelled with the method it covers. The trait uses nameof, so renaming a method
// either updates the trait or breaks the build. Filter with:
// dotnet test --filter-trait "Method=ToDaysAgo"
public class DateFormatterTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly DateFormatter _dateFormatter;

    public DateFormatterTests()
    {
        _dateFormatter = new DateFormatter(_dateTimeProvider);
    }

    [Fact]
    [Trait("Method", nameof(DateFormatter.ToDaysAgo))]
    public void A_date_earlier_today_is_shown_as_today()
    {
        // Arrange
        var twoAmSeptember14 = new DateTime(2026, 9, 14, 2, 0, 0, DateTimeKind.Utc);
        _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 9, 14, 9, 0, 0, DateTimeKind.Utc));

        // Act
        var result = _dateFormatter.ToDaysAgo(twoAmSeptember14);

        // Assert
        result.ShouldBe("Today");
    }

    [Fact]
    [Trait("Method", nameof(DateFormatter.ToDaysAgo))]
    public void One_day_ago_uses_the_singular()
    {
        // Arrange
        var twoAmSeptember14 = new DateTime(2026, 9, 14, 2, 0, 0, DateTimeKind.Utc);
        _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 9, 15, 9, 0, 0, DateTimeKind.Utc));

        // Act
        var result = _dateFormatter.ToDaysAgo(twoAmSeptember14);

        // Assert
        result.ShouldBe("1 day ago");
    }

    [Fact]
    [Trait("Method", nameof(DateFormatter.ToDaysAgo))]
    public void More_than_one_day_ago_uses_the_plural()
    {
        // Arrange
        var twoAmSeptember15 = new DateTime(2026, 9, 15, 2, 0, 0, DateTimeKind.Utc);
        _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 9, 17, 9, 0, 0, DateTimeKind.Utc));

        // Act
        var result = _dateFormatter.ToDaysAgo(twoAmSeptember15);

        // Assert
        result.ShouldBe("2 days ago");
    }

    [Fact]
    [Trait("Method", nameof(DateFormatter.ToDaysAgo))]
    public void An_offset_adds_days_to_the_count()
    {
        // Arrange
        var twoAmSeptember15 = new DateTime(2026, 9, 15, 2, 0, 0, DateTimeKind.Utc);
        _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 9, 15, 9, 0, 0, DateTimeKind.Utc));

        // Act
        var result = _dateFormatter.ToDaysAgo(twoAmSeptember15, offsetDays: 1);

        // Assert
        result.ShouldBe("1 day ago");
    }

    [Fact]
    [Trait("Method", nameof(DateFormatter.ToDayMonthNameAndYear))]
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
