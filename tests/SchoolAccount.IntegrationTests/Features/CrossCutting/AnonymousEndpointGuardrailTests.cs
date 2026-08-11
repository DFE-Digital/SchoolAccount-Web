using Microsoft.AspNetCore.Authorization;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AnonymousEndpointGuardrailTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    // Every endpoint allowed to bypass authentication must be listed here explicitly.
    // Adding a new anonymous endpoint should be a conscious, reviewed decision, not an accident.
    private static readonly HashSet<string> _allowlistedAnonymousEndpoints =
    [
        "/health",
        "/signin-oidc",
        "/account/signout",
        "/account/signedout",
    ];

    [Fact]
    public void Endpoints_marked_as_AllowAnonymous_should_only_be_the_ones_on_the_allowlist()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var endpointDataSource = scope.ServiceProvider.GetRequiredService<EndpointDataSource>();

        var anonymousRoutes = endpointDataSource
            .Endpoints.OfType<RouteEndpoint>()
            .Where(e => e.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            .Select(e => e.RoutePattern.RawText ?? e.DisplayName)
            .Where(x => !string.IsNullOrEmpty(x))
            .OfType<string>()
            .ToList();

        var unexpected = anonymousRoutes
            .Where(route => !_allowlistedAnonymousEndpoints.Contains(route))
            .ToList();

        // Assert
        unexpected.ShouldBeEmpty(
            "Found endpoint(s) marked [AllowAnonymous]/.AllowAnonymous() that aren't on the allowlist: "
                + string.Join(", ", unexpected)
                + $". If intentional, add to {nameof(_allowlistedAnonymousEndpoints)}; if not, remove AllowAnonymous."
        );
    }
}
