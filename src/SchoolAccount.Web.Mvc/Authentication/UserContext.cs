using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Web.Mvc.Authentication;

public sealed class UserContext : IUserContext, IIdentity
{
    private readonly ILogger<UserContext> _logger;
    private static readonly JsonSerializerOptions? _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public UserContext(IHttpContextAccessor contextAccessor, ILogger<UserContext> logger)
    {
        _logger = logger;
        var user = contextAccessor.HttpContext?.User;
        if (user is not null)
        {
            IsAuthenticated = user.Identity?.IsAuthenticated ?? false;
            Id = GetClaim(ClaimConstants.Id, user);
            AuthenticationType = user.Identity?.AuthenticationType;
            GivenName = GetClaim(ClaimConstants.GivenName, user);
            Surname = GetClaim(ClaimConstants.FamilyName, user);
            EmailAddress = GetClaim(ClaimConstants.Email, user);
            var organisationJson = GetClaim(ClaimConstants.Organisation, user);
            if (!string.IsNullOrEmpty(organisationJson))
            {
                Organisation = DeserializeOrganisation(organisationJson);
            }
        }
    }

    public string GivenName { get; }
    public string Surname { get; }
    public bool IsAuthenticated { get; }
    public string Id { get; }
    public string? AuthenticationType { get; }
    public string Name => $"{GivenName} {Surname}".Trim();
    public string EmailAddress { get; }
    public Organisation? Organisation { get; }

    private static string GetClaim(string claimType, ClaimsPrincipal user)
    {
        var claim = user.FindFirst(claimType);
        return claim is null ? string.Empty : claim.Value;
    }

    private Organisation? DeserializeOrganisation(string organisationJson)
    {
        try
        {
            return JsonSerializer.Deserialize<Organisation>(organisationJson, _options);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize organisation claim");
            return null;
        }
    }
}
