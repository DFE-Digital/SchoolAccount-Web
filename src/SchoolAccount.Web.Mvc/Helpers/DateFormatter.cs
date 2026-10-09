using System.Globalization;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Helpers;

public class DateFormatter(IDateTimeProvider dateTimeProvider)
{
    public string ToDayMonthNameAndYear(DateOnly date)
    {
        return date.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
    }

    public string ToDaysAgo(DateTime date)
    {
        var daysAgo = (int)Math.Ceiling((dateTimeProvider.UtcNow - date).TotalDays);
        var dayText = daysAgo == 1 ? "day" : "days";
        return daysAgo == 0 ? "today" : $"{daysAgo} {dayText} ago";
    }
}
