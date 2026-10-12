using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace SchoolAccount.TestCommon.Fakes;

public class DisabledAntiforgery : IAntiforgery
{
    private static readonly AntiforgeryTokenSet Tokens = new(
        "test-token",
        "test-token",
        "__RequestVerificationToken",
        "RequestVerificationToken"
    );

    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext) => Tokens;

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext) => Tokens;

    public Task<bool> IsRequestValidAsync(HttpContext httpContext) => Task.FromResult(true);

    public Task ValidateRequestAsync(HttpContext httpContext) => Task.CompletedTask;

    public void SetCookieTokenAndHeader(HttpContext httpContext) { }
}
