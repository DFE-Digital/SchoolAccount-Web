using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SchoolAccount.IntegrationTests.Common;

public class MockOidcHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "NoAuthScheme";
    public const string AuthoriserRedirectUrl = "https://test-oidc.signin";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
        Task.FromResult(AuthenticateResult.NoResult());

    protected override Task HandleChallengeAsync(AuthenticationProperties? properties)
    {
        Response.Redirect(AuthoriserRedirectUrl);
        return Task.CompletedTask;
    }
}
