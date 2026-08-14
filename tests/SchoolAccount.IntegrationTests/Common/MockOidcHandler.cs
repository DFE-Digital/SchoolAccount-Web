using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace SchoolAccount.IntegrationTests.Common;

public class MockOidcHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string AuthoriserRedirectUrl = "https://test-oidc.signin";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(AuthenticateResult.NoResult());
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        if (Request.Path.Equals("/account/login", StringComparison.OrdinalIgnoreCase))
        {
            Response.Redirect(AuthoriserRedirectUrl);
            return;
        }

        var redirectUrl = "/";

        var returnUrl = !string.IsNullOrEmpty(properties.RedirectUri)
            ? properties.RedirectUri
            : $"{Request.Path}{Request.QueryString}";

        if (!string.IsNullOrEmpty(returnUrl))
        {
            redirectUrl = QueryHelpers.AddQueryString(redirectUrl, "ReturnUrl", returnUrl);
        }

        Response.Redirect(redirectUrl);
        await Task.CompletedTask;
    }
}
