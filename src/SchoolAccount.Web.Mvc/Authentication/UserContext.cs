using System.Security.Principal;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Authentication;

public sealed class UserContext : IUserContext, IIdentity
{
    public UserContext(IHttpContextAccessor contextAccessor)
    {
        var user = contextAccessor.HttpContext?.User;
        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;
        Id = user?.FindFirst("sid")?.Value;
        AuthenticationType = user?.Identity?.AuthenticationType;
        GivenName = user?.FindFirst("given_name")?.Value;
        Surname = user?.FindFirst("family_name")?.Value;
        EmailAddress = user?.FindFirst("email")?.Value;
    }

    public string? GivenName { get; }
    public string? Surname { get; }

    public bool IsAuthenticated { get; }
    public string? Id { get; }
    public string? AuthenticationType { get; }
    public string? Name => $"{GivenName} {Surname}".Trim();
    public string? EmailAddress { get; }
}
