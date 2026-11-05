// Runs School Account locally: `dotnet run --project src/SchoolAccount.AppHost`.
//
// Sign-in
// - By default the app signs in through DfE Sign-in. It uses the web project's
//   own launch profile and settings, as if it were run directly, so its address
//   matches the one registered with DfE Sign-in.
// - With UseMockSignIn=true it signs in through navikt/mock-oauth2-server
//   instead. The end-to-end tests run it this way. The mock's login page takes
//   any username and a JSON object of claims, so each test chooses its academy
//   as it signs in. The web app gets a free port, so the tests never clash
//   with a stack that is already running.
//
// How the mock is wired up
// - The mock builds its issuer from each request's host. The app and the browser
//   both run on this machine and reach it at the same localhost address, so the
//   issuer always matches.
// - The mock serves one issuer per path; the app uses "/default".
// - The mock speaks plain HTTP, so the app's RequireHttpsMetadata is turned off.
//
// Every resource reports healthy only once it answers requests: the web app on
// /health, the mock on its discovery document.
//
// Where things live
// - WebApp.cs, MockSignIn.cs: one file per resource, each with an
//   Add method, and a With method where the web app depends on it.
// - LocalProjects.cs: runs a project on its own launch profile or a free port.
// - ResourceNames.cs: resource names, shared with the end-to-end tests.
using Microsoft.Extensions.Configuration;
using SchoolAccount.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var useMockSignIn = builder.Configuration.GetValue("UseMockSignIn", defaultValue: false);

var web = builder.AddWebApp(useFreePort: useMockSignIn);

if (useMockSignIn)
{
    web.WithMockSignIn(builder.AddMockSignIn());
}

await builder.Build().RunAsync();
