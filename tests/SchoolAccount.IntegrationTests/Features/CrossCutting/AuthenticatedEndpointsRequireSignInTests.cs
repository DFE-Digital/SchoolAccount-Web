using System.Net;
using Microsoft.AspNetCore.Authorization;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AuthenticatedEndpointsRequireSignInTests(
    SchoolAccountWebApplicationFactory<Program> factory
) : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private static readonly HttpStatusCode[] _acceptableUnauthenticatedStatusCodes =
    [
        HttpStatusCode.Unauthorized,
        HttpStatusCode.Redirect,
        HttpStatusCode.Found,
        HttpStatusCode.Forbidden,
    ];

    private static readonly string[] _staticAssetFileExtensions =
    {
        ".css",
        ".js",
        ".map",
        ".png",
        ".jpg",
        ".jpeg",
        ".gif",
        ".svg",
        ".ico",
        ".woff",
        ".woff2",
        ".ttf",
        ".eot",
        ".webmanifest",
        ".gz",
        ".json",
    };

    public static IEnumerable<object[]> ProtectedStaticGetRoutes()
    {
        using var factory = new SchoolAccountWebApplicationFactory<Program>();
        using var scope = factory.Services.CreateScope();
        var endpointDataSource = scope.ServiceProvider.GetRequiredService<EndpointDataSource>();

        return endpointDataSource
            .Endpoints.OfType<RouteEndpoint>()
            .Where(e => e.Metadata.GetMetadata<IAllowAnonymous>() is null)
            .Where(e =>
                e.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.Contains("GET") ?? true
            )
            .Select(e => e.RoutePattern.RawText)
            .Where(route =>
                !string.IsNullOrEmpty(route) && !IsStaticAsset(route) && !route.Contains('{')
            ) // skip parameterised routes; cover those in dedicated tests, this could be tweaked in future
            .Distinct()
            .Select(route => new object[] { route! });
    }

    [Theory]
    [MemberData(nameof(ProtectedStaticGetRoutes))]
    public async Task Protected_endpoint_should_not_return_a_successful_response_when_request_is_unauthenticated(
        string route
    )
    {
        // Arrange
        var client = factory.CreateUnauthorisedClient();

        // Act
        var response = await client.GetAsync(route, TestContext.Current.CancellationToken);

        // Arrange
        _acceptableUnauthenticatedStatusCodes.ShouldContain(
            response.StatusCode,
            $"Expected '{route}' to require authentication, but an unauthenticated request returned {response.StatusCode}."
        );
    }

    private static bool IsStaticAsset(string route)
    {
        return _staticAssetFileExtensions.Any(ext =>
            route.EndsWith(ext, StringComparison.OrdinalIgnoreCase)
        );
    }
}
