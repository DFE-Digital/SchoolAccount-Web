using System.Net;
using Azure.Core;
using NSubstitute;
using SchoolAccount.Web.Mvc.Hosting.Telemetry;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Hosting.Telemetry;

public class AzureMonitorAuthenticationHandlerTests
{
    private const string Endpoint = "https://example.uksouth-1.ingest.monitor.azure.com/v1/traces";

    [Fact]
    public void Adds_a_bearer_token_to_synchronous_sends()
    {
        // Arrange
        var credential = CredentialReturning("sync-token", DateTimeOffset.UtcNow.AddHours(1));
        using var inner = new RecordingHandler();
        using var handler = Handler(credential, inner);

        // Act
        SendRequest(handler);

        // Assert
        inner.Authorization.ShouldBe("Bearer sync-token");
        credential
            .Received(1)
            .GetToken(
                Arg.Is<TokenRequestContext>(context =>
                    context.Scopes.SequenceEqual(new[] { AzureMonitorAuthenticationHandler.Scope })
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Adds_a_bearer_token_to_asynchronous_sends()
    {
        // Arrange
        var credential = CredentialReturning("async-token", DateTimeOffset.UtcNow.AddHours(1));
        using var inner = new RecordingHandler();
        using var handler = Handler(credential, inner);
        using var invoker = new HttpMessageInvoker(handler, disposeHandler: false);
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);

        // Act
        using var response = await invoker.SendAsync(
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        inner.Authorization.ShouldBe("Bearer async-token");
    }

    [Fact]
    public void Reuses_the_token_until_it_is_close_to_expiring()
    {
        // Arrange
        var credential = CredentialReturning("cached", DateTimeOffset.UtcNow.AddHours(1));
        using var inner = new RecordingHandler();
        using var handler = Handler(credential, inner);

        // Act
        SendRequest(handler);
        SendRequest(handler);

        // Assert
        credential
            .Received(1)
            .GetToken(Arg.Any<TokenRequestContext>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Refreshes_a_token_that_is_close_to_expiring()
    {
        // Arrange - inside the five minute refresh margin
        var credential = CredentialReturning("expiring", DateTimeOffset.UtcNow.AddMinutes(2));
        using var inner = new RecordingHandler();
        using var handler = Handler(credential, inner);

        // Act
        SendRequest(handler);
        SendRequest(handler);

        // Assert
        credential
            .Received(2)
            .GetToken(Arg.Any<TokenRequestContext>(), Arg.Any<CancellationToken>());
    }

    private static void SendRequest(HttpMessageHandler handler)
    {
        using var invoker = new HttpMessageInvoker(handler, disposeHandler: false);
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
        using var response = invoker.Send(request, CancellationToken.None);
    }

    private static TokenCredential CredentialReturning(string token, DateTimeOffset expiresOn)
    {
        var accessToken = new AccessToken(token, expiresOn);
        var credential = Substitute.For<TokenCredential>();
        credential
            .GetToken(Arg.Any<TokenRequestContext>(), Arg.Any<CancellationToken>())
            .Returns(accessToken);
        // Configuring a substitute calls the ValueTask-returning method without awaiting it.
#pragma warning disable CA2012
        credential
            .GetTokenAsync(Arg.Any<TokenRequestContext>(), Arg.Any<CancellationToken>())
            .Returns(_ => ValueTask.FromResult(accessToken));
#pragma warning restore CA2012
        return credential;
    }

    private static AzureMonitorAuthenticationHandler Handler(
        TokenCredential credential,
        HttpMessageHandler inner
    ) => new(credential) { InnerHandler = inner };

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public string? Authorization { get; private set; }

        protected override HttpResponseMessage Send(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            Authorization = request.Headers.Authorization?.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        ) => Task.FromResult(Send(request, cancellationToken));
    }
}
