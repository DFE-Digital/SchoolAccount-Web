// Runs School Account locally: `dotnet run --project src/SchoolAccount.AppHost`.
//
// Sign-in
// - By default the app signs in through DfE Sign-in. It uses the web project's
//   own launch profile and settings, as if it were run directly, so its address
//   matches the one registered with DfE Sign-in.
// - With UseMockSignIn=true it signs in through navikt/mock-oauth2-server
//   instead. The end-to-end tests run it this way. The mock's login page takes
//   any username and a JSON object of claims, so each test chooses its academy
//   as it signs in.
//
// How the mock is wired up
// - The mock builds its issuer from each request's host. The app and the browser
//   both run on this machine and reach it at the same localhost address, so the
//   issuer always matches.
// - The mock serves one issuer per path; the app uses "/default".
// - The mock speaks plain HTTP, so the app's RequireHttpsMetadata is turned off.
// - The app waits for the mock's discovery document before it starts.
//
// Both the app and the mock report healthy only once they answer requests: the
// app on /health, the mock on its discovery document.
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

if (builder.Configuration.GetValue("UseMockSignIn", defaultValue: false))
{
    AddWebWithMockSignIn(builder);
}
else
{
    AddWebWithDfESignIn(builder);
}

await builder.Build().RunAsync();

static void AddWebWithDfESignIn(IDistributedApplicationBuilder builder) =>
    builder.AddProject<Projects.SchoolAccount_Web_Mvc>("web").WithHttpHealthCheck("/health");

static void AddWebWithMockSignIn(IDistributedApplicationBuilder builder)
{
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
        .WithEnvironment(
            "OpenIDConnectSettings__Authority",
            ReferenceExpression.Create($"{identityProviderUrl}/default")
        )
        .WithEnvironment("OpenIDConnectSettings__ClientId", "school-account-local")
        .WithEnvironment("Authentication__Schemes__OpenIdConnect__RequireHttpsMetadata", "false")
        .WaitFor(identityProvider);
}
