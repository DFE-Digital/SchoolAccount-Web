namespace SchoolAccount.TestCommon.Constants;

public static class UrlConstants
{
    public const string BaseUrl = "/";

    public const string Error404Url = "/Error/404";
    public const string Error500Url = "/Error/500";
    public const string FeedbackUrl = "/Feedback";
    public const string FeedbackFooterThanksUrl = "#feedback-thanks";
    public const string FeedbackPageThanksUrl = "/Feedback#feedback-thanks";
    public const string FeedbackSubmissionUrl = "/Feedback/Submit";
    public const string JourneyUrl = "/Journey";
    public const string JourneyReturnUrl = "/?ReturnUrl=%2FJourney";
    public const string LoggedOutUrl = "/Account/LoggedOut";
    public const string LoginUrl = "/Account/Login";
    public const string LoginWithExternalReturnUrl =
        "/Account/Login?returnUrl=https%3A%2F%2Fwww.google.com";
    public const string LoginWithJourneyReturnUrl = "/Account/Login?returnUrl=%2FJourney";
    public const string LogoutUrl = "/Account/Logout";
}
