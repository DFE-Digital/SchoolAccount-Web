namespace SchoolAccount.Web.Mvc.Helpers;

public class StatusFormatter
{
    public const string CollectUnavailableStatus = "Unavailable";
    public const string StatusUnavailableLabel = "Status unavailable";

    public string FormatStatus(string status)
    {
        return status is CollectUnavailableStatus ? StatusUnavailableLabel : status;
    }
}
