// Runs School Account locally: `dotnet run --project src/SchoolAccount.AppHost`.
// The end-to-end tests start this same AppHost, so `dotnet test` brings the
// whole stack up and down.
//
// By default the app signs in against navikt/mock-oauth2-server, so you can
// pick any test academy without DfE Sign-in credentials. To sign in through
// DfE Sign-in instead, run with UseMockSignIn=false.
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var useMockSignIn = builder.Configuration.GetValue("UseMockSignIn", defaultValue: true);

if (useMockSignIn)
{
    AddWebWithMockSignIn(builder);
}
else
{
    // Uses the web project's own launch profile and settings, exactly as if it
    // were run directly, so its address matches the one registered with DfE Sign-in.
    builder.AddProject<Projects.SchoolAccount_Web_Mvc>("web").WithHttpHealthCheck("/health");
}

await builder.Build().RunAsync();

static void AddWebWithMockSignIn(IDistributedApplicationBuilder builder)
{
    // Shows a login page taking any username and a JSON object of claims, so
    // you choose an academy as you sign in. The mock builds its issuer from the
    // request's host; the app and the browser both run on this machine and
    // reach it at the same localhost address, so the issuer always matches.
    var identityProvider = builder
        .AddContainer("mock-oauth2-server", "ghcr.io/navikt/mock-oauth2-server", "6.1.2")
        .WithHttpEndpoint(targetPort: 8080, name: "http")
        .WithEnvironment("JSON_CONFIG", """{"interactiveLogin": true}""")
        .WithHttpHealthCheck("/default/.well-known/openid-configuration");

    var identityProviderUrl = identityProvider.GetEndpoint("http");

    builder
        .AddProject<Projects.SchoolAccount_Web_Mvc>("web", launchProfileName: null)
        .WithHttpEndpoint(name: "http")
        .WithHttpHealthCheck("/health")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        // The mock serves one issuer per path; "default" is used here.
        .WithEnvironment(
            "OpenIDConnectSettings__Authority",
            ReferenceExpression.Create($"{identityProviderUrl}/default")
        )
        .WithEnvironment("OpenIDConnectSettings__ClientId", "school-account-local")
        // The mock speaks plain HTTP.
        .WithEnvironment("Authentication__Schemes__OpenIdConnect__RequireHttpsMetadata", "false")
        .WaitFor(identityProvider);
}
