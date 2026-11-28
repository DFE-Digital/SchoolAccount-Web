---
status: "accepted"
date: 2026-11-28
decision-makers: Paul Custance
---

# Use Aspire to run the app and its dependencies locally and in the E2E tests

## Context and Problem Statement

Running School Account locally means starting the web app, the Collect API from its own repository, SQL Server with
the `COLLECTPortal` and `CollectStateLedger` databases, and Azurite for the feedback table. Until now that meant cloning
SchoolAccount-LocalDevTools, restoring a backup of `COLLECTPortal`, and starting each piece by hand. The E2E tests
assumed all of it was already running, with data that changed from one machine to the next. How do we start the whole
stack the same way every time, both when developing and when the E2E tests run?

## Decision Drivers

* One command starts everything the web app needs, with known test data.
* The E2E tests start their own dependencies and leave nothing running, so a run doesn't depend on what's on the
  machine.
* The web app must stay at `https://127.0.0.1:7041`, the only local address DfE Sign-in sends users back to.
* Telemetry from both apps can be seen locally without extra tools.
* Database schema and test data live in source control, built the same way as SchoolAccount-Database.

## Considered Options

* An Aspire AppHost, started by developers and by the E2E tests
* Docker Compose for the dependencies, with the apps run by hand
* Testcontainers inside the E2E test project
* Keep SchoolAccount-LocalDevTools and a restored backup

## Decision Outcome

Chosen option: "An Aspire AppHost, started by developers and by the E2E tests", because it's the only option that
starts the .NET projects and the containers together, wires their addresses into each other, and gives the tests the
same stack developers run.

How it works:

* `src/SchoolAccount.AppHost` starts:
  * SQL Server, with `COLLECTPortal` and `CollectStateLedger` created with the iStore collation (`Latin1_General_CI_AS`)
    and then migrated and seeded by Flyway containers. The migrations and seed data are in
    `tests/SchoolAccount.E2ETests/Databases`.
  * Azurite, with the `SchoolAccountFeedback` table created once the emulator is ready.
  * The Collect API, from `../SchoolAccount-CollectAPI` cloned next to this repository, on any free port. It's given the
    SQL Server's connection string once both databases are seeded.
  * The web app, from its `https` launch profile so it stays at `https://127.0.0.1:7041`. It's given the Collect API's
    address and the Azurite connection string.
* Both apps send logs, traces and metrics to the Aspire dashboard. Aspire sets `OTEL_EXPORTER_OTLP_ENDPOINT` for each
  project, and both already export over OTLP in Development (see
  [ADR 0012](0012-use-azure-monitor-opentelemetry-distro.md)).
* When a developer runs the AppHost, SQL Server and Azurite are persistent: they keep running after it stops and are
  reused, data and all, the next time. The SQL Server password Aspire generates is kept in the AppHost's user secrets
  so it stays the same between runs. Every container is labelled so Docker Desktop groups them as `schoolaccount`.
* The E2E tests start the AppHost through `Aspire.Hosting.Testing`, once per run, in `AspireFixture`. A test run turns
  off port randomisation, so the web app keeps its address, and turns off persistence, so it gets its own containers,
  grouped as `schoolaccount-tests`, and removes them when it ends. The fixture then points the suite's settings at the
  SQL Server and site it started. Setting `Aspire:Enabled` to `false` runs the suite against things started by hand.
* Projects are added with `AddDotnetProject`, by path, on a plain `Microsoft.NET.Sdk` project, as Aspire 17 asks.

### Consequences

* Good, because `dotnet run --project src/SchoolAccount.AppHost` starts everything, with the same data on every machine.
* Good, because the E2E tests start from clean data every run and stop everything when they finish.
* Good, because the dashboard shows the logs, traces and health of every resource in one place.
* Good, because the test data, including the schools on the DfE Sign-in test accounts, is reviewed and versioned
  with the code.
* Bad, because the Collect API has to be cloned next to this repository, and Docker has to be running.
* Bad, because only one AppHost can run at a time. Port 7041 fits one web app, and starting the AppHost with the Aspire
  CLI, which `dotnet run` does, stops one that's already running.
* Bad, because the first start is slow while SQL Server and the images download, and a test run takes about a minute
  to start.
* Bad, because Aspire is moving quickly. Version 17 made `AddProject` and the AppHost SDK obsolete, and upgrades may
  need code changes again.
* Neutral, because the seed data is copied from a restored COLLECTPortal backup by a one-off script. Changing it means
  regenerating the seed files.

### Confirmation

The E2E suite starts the AppHost on every run, so a broken AppHost fails it: the fixture names the resource that didn't
start. The suite's database tests check the sync procedure finds nothing to change in the seeded data.

## Pros and Cons of the Options

### An Aspire AppHost, started by developers and by the E2E tests

* Good, because it runs .NET projects and containers together and passes addresses and connection strings between
  them.
* Good, because the same AppHost serves local development and the E2E tests.
* Good, because the dashboard collects telemetry with no extra setup.
* Bad, because it adds a project and a set of packages to keep up to date.

### Docker Compose for the dependencies, with the apps run by hand

The two databases were first run this way, from a compose file in the E2E test project, before the AppHost replaced
it. Keeping both meant two ways to build the same databases, so the compose route was removed.

* Good, because it needs only Docker.
* Bad, because the web app and the Collect API still have to be started and configured by hand.
* Bad, because the tests can't start or stop it themselves.

### Testcontainers inside the E2E test project

* Good, because the tests start their own containers.
* Bad, because it covers only the containers. The web app and the Collect API would need starting some other way.
* Bad, because developers would run the stack differently from the tests.

### Keep SchoolAccount-LocalDevTools and a restored backup

* Good, because nothing new is needed.
* Bad, because the data differs between machines and changes as people use it, so tests aren't repeatable.
* Bad, because every piece is started by hand, and the E2E tests can't start any of it.

## More Information

The Aspire docs that this follows are the
[ASPIRE012 migration](https://aspire.dev/diagnostics/aspire012/),
[launch profiles](https://aspire.dev/integrations/dotnet/launch-profiles/) and
[testing](https://aspire.dev/testing/overview/) pages.

Seq was tried as a log viewer in the AppHost and taken out of the app entirely, as the Aspire dashboard is enough for
now.

How to run the E2E tests, and the test data they use, are described in
[tests/SchoolAccount.E2ETests/README.md](../tests/SchoolAccount.E2ETests/README.md).
