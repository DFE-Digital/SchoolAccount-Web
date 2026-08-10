namespace SchoolAccount.Web.Mvc;

public static class RouteConstants
{
    public const string Root = "/";
    public const string Dashboard = "dashboard";

    public static class Error
    {
        public const string Index = "error";
        public const string StatusCode = "{statusCode}";
    }

    public static class Account
    {
        public const string Index = Root + "account";
        public const string Login = "login";
        public const string Logout = "logout";
        public const string FullLogoutPath = Index + "/logout";
    }
}
