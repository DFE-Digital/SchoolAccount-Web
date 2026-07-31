using System.Security.Claims;
using Microsoft.FeatureManagement.FeatureFilters;

namespace SchoolAccount.Web.Mvc.Infrastructure;

public class FeatureManagementContextAccessor(IHttpContextAccessor httpContextAccessor)
    : ITargetingContextAccessor
{
    public ValueTask<TargetingContext> GetContextAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var userId = httpContext?.User?.Identity?.Name;
        var groups = new List<string>();

        if (httpContext?.User != null)
        {
            groups.AddRange(
                httpContext.User.FindAll(ClaimTypes.Role).Select(roleClaim => roleClaim.Value)
            );
        }

        return new ValueTask<TargetingContext>(
            new TargetingContext { UserId = userId ?? "anonymous", Groups = groups }
        );
    }
}
