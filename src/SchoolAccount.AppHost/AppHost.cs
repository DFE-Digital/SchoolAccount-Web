// Runs School Account locally: `dotnet run --project src/SchoolAccount.AppHost`.
//
// What it starts
// - The web app.
// - The Collect API, from its own repository cloned next to this one
//   (../SchoolAccount-CollectAPI). The AppHost builds it, and the web app waits
//   for it and is given its address.
//
// Both use their own launch profiles and settings, as if they were run directly,
// so the web app's address matches the one registered with DfE Sign-in. The
// Collect API gets any free port, so one already running by hand doesn't clash.
//
// Every resource reports healthy only once it answers requests on /health.
//
// Where things live
// - WebApp.cs, CollectApi.cs: one file per resource, each with an Add method, and
//   a With method where another resource depends on it.
// - ResourceNames.cs: resource names.
using SchoolAccount.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var collectApi = builder.AddCollectApi();

builder.AddWebApp().WithCollectApi(collectApi);

await builder.Build().RunAsync();
