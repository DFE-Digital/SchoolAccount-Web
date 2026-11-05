namespace SchoolAccount.AppHost;

internal static class MockSignIn
{
    public static IResourceBuilder<ContainerResource> AddMockSignIn(
        this IDistributedApplicationBuilder builder
    )
    {
        return builder
            .AddContainer(ResourceNames.MockSignIn, "ghcr.io/navikt/mock-oauth2-server", "6.1.2")
            .WithHttpEndpoint(targetPort: 8080, name: "http")
            .WithEnvironment("JSON_CONFIG", """{"interactiveLogin": true}""")
            .WithHttpHealthCheck("/default/.well-known/openid-configuration");
    }

    public static IResourceBuilder<ProjectResource> WithMockSignIn(
        this IResourceBuilder<ProjectResource> web,
        IResourceBuilder<ContainerResource> mockSignIn
    )
    {
        var authority = ReferenceExpression.Create($"{mockSignIn.GetEndpoint("http")}/default");

        return web.WithEnvironment("OpenIDConnectSettings__Authority", authority)
            .WithEnvironment("OpenIDConnectSettings__ClientId", "school-account-local")
            .WithEnvironment(
                "Authentication__Schemes__OpenIdConnect__RequireHttpsMetadata",
                "false"
            )
            .WaitFor(mockSignIn);
    }
}
