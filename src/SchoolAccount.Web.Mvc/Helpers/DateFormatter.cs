using System.Globalization;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Helpers;

public class DateFormatter(IDateTimeProvider dateTimeProvider)
{
    public string ToDayMonthNameAndYear(DateOnly date)
    {
        return date.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
    }

    public string ToDaysAgo(DateTime date, int offsetDays = 0)
    {
        var daysAgo = (int)Math.Floor((dateTimeProvider.UtcNow - date).TotalDays) + offsetDays;
        var dayText = daysAgo == 1 ? "day" : "days";
        return daysAgo == 0 ? "Today" : $"{daysAgo} {dayText} ago";
    }
}
