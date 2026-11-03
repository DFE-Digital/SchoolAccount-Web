namespace SchoolAccount.Web.Mvc.Authentication.Models;

public class AuthenticationSettings
{
    public const string SectionName = "OpenIDConnectSettings";

    public required string Authority { get; init; }
    public required string ClientId { get; init; }

    /// <summary>
    /// Sent when redeeming the authorisation code, if the DSI client has one.
    /// Leave unset for a public client.
    /// </summary>
    public string? ClientSecret { get; init; }
}
