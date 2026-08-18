using System.Security.Principal;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Authentication.Extensions;

namespace SchoolAccount.Web.Mvc.Authentication;

public sealed class UserContext : IUserContext, IIdentity
{
    public UserContext(IHttpContextAccessor contextAccessor)
    {
        var user = contextAccessor.HttpContext?.User;
        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;
        Id = user?.FindFirst(ClaimConstants.Id)?.Value;
        AuthenticationType = user?.Identity?.AuthenticationType;
        GivenName = user?.FindFirst(ClaimConstants.GivenName)?.Value;
        Surname = user?.FindFirst(ClaimConstants.FamilyName)?.Value;
        EmailAddress = user?.FindFirst(ClaimConstants.Email)?.Value;
        OrganisationName = user?.GetOrganisation()?.Name;
    }

    public string? GivenName { get; }
    public string? Surname { get; }
    public bool IsAuthenticated { get; }
    public string? Id { get; }
    public string? AuthenticationType { get; }
    public string? Name => $"{GivenName} {Surname}".Trim();
    public string? EmailAddress { get; }

    public string? OrganisationName { get; }
}
