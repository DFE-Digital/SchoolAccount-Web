namespace SchoolAccount.Web.Mvc.Authentication.Models;

public class AuthenticationSettings
{
    public const string SectionName = "OpenIDConnectSettings";
    public required string Authority { get; init; }
    public required string ClientId { get; init; }
}
