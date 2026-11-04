// Runs the web app against navikt/mock-oauth2-server instead of DfE Sign-in.
// The end-to-end tests start this AppHost themselves, so `dotnet test` brings
// the whole stack up and down. It can also be run on its own with `dotnet run`.
var builder = DistributedApplication.CreateBuilder(args);

// Shows a login page taking any username and a JSON object of claims, so each
// test chooses its academy when it signs in. The mock builds its issuer from
// the request's host; the app and the browser both run on this machine and
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
    // A static file, so the check needs no sign-in or backing APIs.
    .WithHttpHealthCheck("/assets/manifest.json")
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

await builder.Build().RunAsync();
