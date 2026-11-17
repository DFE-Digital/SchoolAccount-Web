using System.Globalization;

namespace SchoolAccount.Web.Mvc.Helpers;

public class DateFormatter(TimeProvider timeProvider)
{
    public string ToDayMonthNameAndYear(DateOnly date)
    {
        return date.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
    }

    public string ToDaysAgo(DateTime date, int offsetDays = 0)
    {
        var daysAgo = (int)Math.Floor((timeProvider.GetUtcNow().UtcDateTime - date).TotalDays) + offsetDays;
        var dayText = daysAgo == 1 ? "day" : "days";
        return daysAgo == 0 ? "Today" : $"{daysAgo} {dayText} ago";
    }
}
