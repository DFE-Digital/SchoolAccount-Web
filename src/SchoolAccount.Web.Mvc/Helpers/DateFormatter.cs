using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Helpers;

public class DateFormatter(IDateTimeProvider dateTimeProvider)
{
    public string ToDayMonthNameAndYear(DateOnly date)
    {
        return date.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
    }

    public string ToDaysAgo(DateTime date, int offset = 0)
    {
        var daysAgo = (int)(Math.Ceiling((dateTimeProvider.UtcNow - date).TotalDays) + offset);
        var dayText = daysAgo == 1 ? "day" : "days";
        return daysAgo == 0 ? "today" : $"{daysAgo} {dayText} ago";
    }
}
