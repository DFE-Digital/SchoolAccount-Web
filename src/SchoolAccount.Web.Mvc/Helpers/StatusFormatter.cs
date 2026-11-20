namespace SchoolAccount.Web.Mvc.Helpers;

public class StatusFormatter
{
    public const string UnavailableStatus = "Unavailable";
    public const string StatusUnavailable = "Status unavailable";

    public string FormatStatus(string status)
    {
        return status is UnavailableStatus ? StatusUnavailable : status;
    }
}
