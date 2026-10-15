using System.Net.Http.Headers;
using Azure.Core;

namespace SchoolAccount.Web.Mvc.Hosting.Telemetry;

/// <summary>
/// Adds a Microsoft Entra bearer token to OTLP export requests, which Azure Monitor's OTLP
/// ingestion endpoints require. The token is cached and refreshed shortly before it expires.
/// </summary>
/// <remarks>
/// Overrides <see cref="Send"/> as well as <see cref="SendAsync"/>: the OTLP HTTP exporter sends
/// synchronously over HTTP/1.1, and the base <see cref="Send"/> would skip the token.
/// </remarks>
public sealed class AzureMonitorAuthenticationHandler(TokenCredential credential)
    : DelegatingHandler
{
    public const string Scope = "https://monitor.azure.com/.default";

    private static readonly TimeSpan RefreshMargin = TimeSpan.FromMinutes(5);

    private readonly TokenRequestContext _tokenRequest = new([Scope]);
    private readonly Lock _lock = new();
    private AccessToken _token;

    protected override HttpResponseMessage Send(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        Authorize(request, GetToken(cancellationToken));
        return base.Send(request, cancellationToken);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var token = IsFresh(_token)
            ? _token
            : await RefreshAsync(cancellationToken).ConfigureAwait(false);

        Authorize(request, token);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private AccessToken GetToken(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (!IsFresh(_token))
            {
                _token = credential.GetToken(_tokenRequest, cancellationToken);
            }

            return _token;
        }
    }

    private async Task<AccessToken> RefreshAsync(CancellationToken cancellationToken)
    {
        var token = await credential
            .GetTokenAsync(_tokenRequest, cancellationToken)
            .ConfigureAwait(false);

        lock (_lock)
        {
            _token = token;
        }

        return token;
    }

    private static bool IsFresh(AccessToken token) =>
        token.Token is not null && token.ExpiresOn - RefreshMargin > DateTimeOffset.UtcNow;

    private static void Authorize(HttpRequestMessage request, AccessToken token) =>
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
}
