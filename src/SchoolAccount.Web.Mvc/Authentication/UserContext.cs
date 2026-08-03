using System.Security.Claims;
using System.Security.Principal;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Authentication;

internal sealed class UserContext : IUserContext, IIdentity
{
    public UserContext(IHttpContextAccessor contextAccessor)
    {
        var user = contextAccessor.HttpContext?.User;
        AuthenticationType = user?.Identity?.AuthenticationType;
        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;
        Id = user?.FindFirst("sid")?.Value;
        GivenName = user?.FindFirst("given_name")?.Value;
        Surname = user?.FindFirst("family_name")?.Value;
        EmailAddress = user?.FindFirst("email")?.Value;
    }

    public string? AuthenticationType { get; }
    public bool IsAuthenticated { get; }
    public string? Id { get; }
    public string? GivenName { get; }
    public string? Surname { get; }
    public string? Name => $"{GivenName} {Surname}".Trim();
    public string? EmailAddress { get; }
}
