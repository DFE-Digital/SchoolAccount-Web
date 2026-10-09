using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Helpers;

public static class UrlHelperExtensions
{
    /// <summary>
    /// Returns <paramref name="returnUrl"/> when it points to this site, otherwise the home page,
    /// so a crafted return URL can't send users to another site.
    /// </summary>
    public static Uri SafeReturnUrl(this IUrlHelper url, string? returnUrl) =>
        new(url.IsLocalUrl(returnUrl) ? returnUrl : "/", UriKind.Relative);
}
