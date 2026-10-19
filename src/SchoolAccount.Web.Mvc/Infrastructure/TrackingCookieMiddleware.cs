using Microsoft.AspNetCore.Mvc.Controllers;

namespace SchoolAccount.Web.Mvc.Infrastructure;

/// <summary>
/// Gives each browser a random identifier in a cookie, so the web application firewall has a
/// stable value to track a client by when its IP address changes. The identifier is a label, not
/// a credential: nothing in the application reads it or trusts it for anything.
/// </summary>
internal sealed class TrackingCookieMiddleware(RequestDelegate next)
{
    private const string _cookieName = "SchoolAccountSessionId";

    private static readonly CookieOptions _cookieOptions = new()
    {
        HttpOnly = true,
        Secure = true,
        // None, not Lax, so it is still sent on the cross-site POST back from DfE Sign-in
        SameSite = SameSiteMode.None,
        IsEssential = true,
    };

    public Task InvokeAsync(HttpContext context)
    {
        if (IsPageRequest(context) && !HasIdentifier(context.Request))
        {
            context.Response.Cookies.Append(_cookieName, Guid.NewGuid().ToString(), _cookieOptions);
        }

        return next(context);
    }

    // Pages only. Static assets are cached publicly, and a shared cache must not be handed a
    // Set-Cookie that it could replay to other users.
    private static bool IsPageRequest(HttpContext context)
    {
        return context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>()
            is not null;
    }

    // A value that is not one of our GUIDs is replaced, so the firewall never tracks junk
    private static bool HasIdentifier(HttpRequest request)
    {
        return request.Cookies.TryGetValue(_cookieName, out var value)
            && Guid.TryParseExact(value, "D", out _);
    }
}
