using System.Security.Claims;
using System.Security.Principal;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Authentication.Extensions;

namespace SchoolAccount.Web.Mvc.Authentication;

public sealed class UserContext : IUserContext, IIdentity
{
    public UserContext(IHttpContextAccessor contextAccessor)
    {
        var user = contextAccessor.HttpContext?.User;
        if (user is not null)
        {
            IsAuthenticated = user.Identity?.IsAuthenticated ?? false;
            Id = GetClaim(ClaimConstants.Id, user);
            AuthenticationType = user.Identity?.AuthenticationType;
            GivenName = GetClaim(ClaimConstants.GivenName, user);
            Surname = GetClaim(ClaimConstants.FamilyName, user);
            EmailAddress = GetClaim(ClaimConstants.Email, user);
            OrganisationName = user.GetOrganisation()?.Name;
        }
    }

    public string GivenName { get; } = string.Empty;
    public string Surname { get; } = string.Empty;
    public bool IsAuthenticated { get; }
    public string Id { get; } = string.Empty;
    public string? AuthenticationType { get; }
    public string Name => $"{GivenName} {Surname}".Trim();
    public string EmailAddress { get; } = string.Empty;

    public string? OrganisationName { get; }

    private static string GetClaim(string claimType, ClaimsPrincipal user)
    {
        var claim = user.FindFirst(claimType);
        return claim is null ? string.Empty : claim.Value;
    }
}
