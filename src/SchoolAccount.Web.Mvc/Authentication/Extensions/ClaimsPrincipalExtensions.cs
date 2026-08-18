using System.Security.Claims;
using System.Text.Json;

namespace SchoolAccount.Web.Mvc.Authentication.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static OrganisationClaim? GetOrganisation(
        this ClaimsPrincipal principal,
        JsonSerializerOptions? options = null
    )
    {
        options ??= new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var organisationClaim = principal.FindFirst(ClaimConstants.Organisation)?.Value;
        return !string.IsNullOrEmpty(organisationClaim)
            ? JsonSerializer.Deserialize<OrganisationClaim>(organisationClaim, options)
            : null;
    }
}
