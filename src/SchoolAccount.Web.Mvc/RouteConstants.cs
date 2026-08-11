using System.Diagnostics.CodeAnalysis;
using System.Net;
using SchoolAccount.Web.Mvc.Features.Accounts;

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
        public const string SignIn = nameof(AccountController.SignIn);
        public const string SignOut = nameof(AccountController.SignOut);
        public const string SignedOut = nameof(AccountController.SignedOut);
    }

    public static string GeneratePath(params string[] sections)
    {
        return GeneratePath(sections, null);
    }

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
    public static string GeneratePath(string[] sections, object? query)
    {
        var path = ($"/" + string.Join("/", sections)).Replace("//", "/");
        var queryString = new List<string>();

        if (query is not null)
        {
            foreach (var property in query.GetType().GetProperties())
            {
                queryString.Add(
                    $"{property.Name}={WebUtility.UrlEncode(property.GetValue(query)?.ToString())}"
                );
            }
        }

        return query is null
            ? path.ToLowerInvariant()
            : $"{path.ToLowerInvariant()}?{string.Join("&", queryString)}";
    }
}
