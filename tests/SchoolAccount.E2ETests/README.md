# SchoolAccount.E2ETests

Playwright E2E Tests that run against the local dockerised/containerised version of the application. 

In theory this can be pointed at any website, but due to the difficulties with the sign-in system, this is primarily setup for local use.

## What needs to be running

By default the run starts everything it needs itself, through the Aspire AppHost in
`src/SchoolAccount.AppHost`, and stops it all when it ends:

| Service                | Where                                                        | Needed by                        |
|------------------------|--------------------------------------------------------------|----------------------------------|
| School Account web app | `https://127.0.0.1:7041`                                     | Every browser test               |
| Collect API            | A free port, given to the web app                            | Journey tests                    |
| SQL Server             | A free port, with `COLLECTPortal` and `CollectStateLedger` seeded by Flyway | Database tests and Journey tests |
| Azurite                | Free ports, with the `SchoolAccountFeedback` table created, given to the web app | The feedback form |

For that you need Docker running, and
[SchoolAccount-CollectAPI](https://github.com/DFE-Digital/SchoolAccount-CollectAPI) cloned next to
this repository. Start-up takes about a minute, and the run fails with the resource at fault if any
of them doesn't come up. Nothing else may be using port 7041, so stop the web app if you have it
running.

To run against a site and databases you've started yourself instead, set `Aspire:Enabled` to
`false`. The suite then uses `SchoolAccount:Url` and `Database:*` as they are.

When you run the AppHost yourself (`dotnet run --project src/SchoolAccount.AppHost`), SQL Server and
Azurite keep running after you stop it, and the next start reuses them, data and all. They're grouped
as `schoolaccount` in Docker Desktop. Remove them for a clean start with:

```bash
docker rm -f $(docker ps -aq --filter label=com.docker.compose.project=schoolaccount)
```

A test run doesn't use those. It starts its own containers, grouped as `schoolaccount-tests`, and
removes them when it ends, so every run starts from clean data. Set `PersistentContainers=false` on
the AppHost to get that behaviour when you run it yourself.

Two things the set-up needs:

- **The site must be at exactly `https://127.0.0.1:7041`.** DfE Sign-in only accepts that address as a redirect for
  local development. `https://localhost:7041` won't work.
- **The web app must use the DfE Sign-in test environment** (`https://test-oidc.signin.education.gov.uk`).

## One-off set-up

### User secrets

Run these from this folder:

```bash
dotnet user-secrets set "SchoolAccount:Email" "<test account email>"
dotnet user-secrets set "SchoolAccount:Password" "<test account password>"
dotnet user-secrets set "Database:UserId" "<SQL Server user>"
dotnet user-secrets set "Database:Password" "<SQL Server password>"
```

The signed-in tests expect an **establishment** account. A trust account doesn't see the census status or the
Go to COLLECT button, so those tests fail.

If the test account is linked to more than one organisation, DfE Sign-in asks which one to use after the password.
Set the URN of the school to sign in as, and the tests pick it from that list:

```bash
dotnet user-secrets set "SchoolAccount:OrganisationUrn" "<school URN>"
```

The URN is shown under each school's name on the "Select your organisation" page. It's matched instead of the name
because some trusts and schools share a name, and only schools have a URN.

### Playwright browser

Build once, then install Chromium:

```bash
dotnet build tests/SchoolAccount.E2ETests
pwsh tests/SchoolAccount.E2ETests/bin/Debug/net10.0/playwright.ps1 install chromium
```

That needs [PowerShell](https://learn.microsoft.com/powershell/scripting/install/installing-powershell). If you don't
have it, the build also copies in Playwright's own copy of Node, which can install Chromium straight from bash. Swap
`darwin-arm64` for your machine's folder under `.playwright/node/` (for example `darwin-x64` or `linux-x64`).

```bash
cd tests/SchoolAccount.E2ETests/bin/Debug/net10.0
.playwright/node/darwin-arm64/node .playwright/package/cli.js install chromium
```

Run this again whenever the `Microsoft.Playwright` package version changes.

## Running the tests

Run these from the repository root:

```bash
# Everything
dotnet test --project tests/SchoolAccount.E2ETests

# One class
dotnet test --project tests/SchoolAccount.E2ETests --filter-class "*JourneyPageTests"

# One test
dotnet test --project tests/SchoolAccount.E2ETests --filter-method "*Tab_order_matches_the_saved_snapshot"

# Watch the browser while the tests run
Headless=false dotnet test --project tests/SchoolAccount.E2ETests
```

CI builds this project but doesn't run it, because there's no site, sign-in or database there. The csproj turns
the test run off when `CI=true`, which GitHub Actions sets.

## Databases

The AppHost runs one SQL Server with `COLLECTPortal` and `CollectStateLedger` on it, both built and
filled with test data by Flyway from the folders under `Databases/`, laid out in the same way as
SchoolAccount-Database. They share a server because the ledger's sync procedure reads
`COLLECTPortal` by its three-part name. Both use `Latin1_General_CI_AS`, as in iStore.

To see what Flyway applied, open the `portal-migrations` and `ledger-migrations` logs in the Aspire
dashboard. To start again from clean data when running the AppHost yourself, remove the persistent
containers as shown above.

### COLLECTPortal

`Databases/CollectPortal/flyway/` only has the four tables `AddChangedCollectReturnStatus` reads:
`DataCollection`, `Organisation`, `OrganisationRole` and `DataReturn`. Their columns and keys match
iStore.

The seed is copied from a restored COLLECTPortal backup for the same schools as the ledger seed, with
telephone, fax, email and web left out. Each return's status and counts match the school's latest
ledger row, so running the sync adds nothing until a test changes a return. The made-up
`SchoolCensus2026_Autumn` (DCID 1205) is cloned from Autumn 2025 a year on, with its own roles
(school roles numbered DataReturnID + 1500000) and returns.

### CollectStateLedger

What's in `Databases/CollectStateLedger/flyway/`:

| Folder        | Contents                                                                                     |
|---------------|----------------------------------------------------------------------------------------------|
| `migrations/` | The tables, from `sql/tables.sql` in SchoolAccount-CollectStateLedgerDatabase                |
| `repeatable/` | `AddChangedCollectReturnStatus`, from `sql/stored-procedures.sql` in the same repo           |
| `seed/`       | Test data, re-run after every migrate. A school that already has rows for a census is skipped |

The seed data is taken from real ledger rows (school names and LAEStabs are public data), with
`UpdatedAt` set relative to when the seed runs:

- **SchoolCensus2025_Spring**: about 200 schools at their final status, plus 13 schools with a full
  status history. Goldenhill Primary Academy (`8612009`) ends on its real status, so its hash matches.
- **SchoolCensus2026_Autumn**: 74 schools at earlier stages, as the Collect API treats this census
  as open. Its DCID (1205) and DataReturnIDs are made up.
- **RegisteredUsers** for the schools with a history, using made-up `example.com` addresses, and a
  `JobStatus` row for `collect-notifications-local` from a day ago.
- **DfE Sign-in test schools**: the 15 establishments developers' accounts are linked to in the DfE
  Sign-in test environment, so whichever one you sign in as has data. Each has its real Spring 2025
  row and is at a different Autumn 2026 status, so you can see the journey page at every stage by
  switching organisation:

  | School                                         | URN    | LAEStab   | Autumn 2026 status          |
  |------------------------------------------------|--------|-----------|-----------------------------|
  | Abbey Manor College                            | 130856 | `2091103` | No data                     |
  | Bede Academy                                   | 135619 | `9296905` | Awaiting file upload        |
  | Beulah Junior School                           | 101712 | `3062003` | File uploaded               |
  | Bourne Abbey Church of England Primary Academy | 136354 | `9253510` | File upload failed          |
  | Brookland Infant and Nursery School            | 101263 | `3022008` | Waiting for validation      |
  | Colsterworth Church of England Primary School  | 146279 | `9252049` | Validation in progress      |
  | East Herrington Primary Academy                | 138331 | `3942091` | Loaded and validated        |
  | Goldington Green Academy                       | 139523 | `8222008` | Uploaded, validation failed |
  | Hazelwick School                               | 137263 | `9384029` | Awaiting submission         |
  | E-ACT Heartlands Academy                       | 135907 | `3306905` | Submitted                   |
  | Heath Lane Nursery School                      | 117070 | `9191009` | Awaiting approval           |
  | Manor Park Primary School                      | 103691 | `3312141` | Approved                    |
  | Muriel Green Nursery School                    | 133975 | `9191028` | Rejected                    |
  | Oakridge Junior School                         | 115974 | `8502227` | Awaiting authorisation      |
  | Park Spring Primary School                     | 107964 | `3832488` | Authorised                  |

  East Herrington is the school the E2E tests sign in as, so it also has a full history in both
  censuses. The trusts on those accounts have no census return, so they have no data.

## Configuration

Settings come from `appsettings.json`, then environment variables, then user secrets. Later sources win, so user
secrets take priority. When `Aspire:Enabled` is on, the run then sets `SchoolAccount:Url` and `Database:*` to what
the AppHost started, on top of all of those, so the `Database:*` user secrets are only needed when it's off.

| Setting                  | Default                     | Notes                                                    |
|--------------------------|-----------------------------|----------------------------------------------------------|
| `Aspire:Enabled`         | `true`                      | Start the dependencies through the AppHost (see above)   |
| `SchoolAccount:Url`      | `https://127.0.0.1:7041`    | The site under test                                      |
| `SchoolAccount:Email`    | none                        | User secrets. Required for the signed-in tests           |
| `SchoolAccount:Password` | none                        | User secrets. Required for the signed-in tests           |
| `SchoolAccount:OrganisationUrn` | none                 | User secrets. Needed if the account has more than one organisation |
| `Database:Server`        | `localhost,1433`            | Set by the run when Aspire is on. Must be `localhost` or a `127.x.x.x` address (see below) |
| `Database:UserId`        | none                        | User secrets. Only needed when Aspire is off             |
| `Database:Password`      | none                        | User secrets. Only needed when Aspire is off             |
| `Headless`               | `true`                      | `false` opens a visible browser                          |
| `Screenshot:*`, `Video:*` | on, for passes and failures | `Enabled`, `CaptureOnSuccess`, `CaptureOnFailure`        |

To set a value with an environment variable, replace `:` with `__`, for example
`SchoolAccount__Url=https://127.0.0.1:7041`.

## Things to know

- **Sign-in happens once per run.** Every signed-in test reuses the same session. If sign-in fails, it's retried
  from scratch up to 3 times, and every signed-in test then fails with *"Sign-in failed after 3 attempts"* and the
  reason for each attempt. If DfE Sign-in shows an error, such as a wrong password, it isn't retried, so the test
  account can't get locked.
- **The database tests change data**, so they refuse to run against anything other than `localhost` or a
  `127.x.x.x` address. Running them elsewhere means changing `LocalDatabase` deliberately.
- **Tab-order snapshot.** `Features/Journey/Journey.tab-order.json` records the expected keyboard tab order on the
  Journey page. If the order changes, the test fails and writes `Journey.tab-order.received.json` next to it. If
  the new order is correct, rename that file over the snapshot. The step titles come from the Collect API, so a
  wording change there also fails this test. The census season (Autumn, Spring or Summer) is recorded as
  `[season]`, so the snapshot doesn't need changing when the census moves on.
- **Screenshots and videos** of each browser test are kept so you can see what the browser was showing when a
  test failed, without rerunning it. They're saved under `bin/Debug/net10.0/screenshots/` and
  `bin/Debug/net10.0/videos/`, in a folder for each test class, named after the test and the time it ran.
  `BrowserTestBase` takes them when each test finishes, and the `Screenshot:*` and `Video:*` settings control
  whether they're kept for passing tests, failing tests or neither. The code that names and places the files is
  in `Common/Artifacts/`.

## Layout

```
Common/         Configuration, fixtures, page objects, database helpers, and screenshot and video
                naming (Artifacts/)
Features/       Browser tests, one folder per page of the site
CollectData/    Tests that read and write the Collect databases
Databases/      Flyway migrations and seed data for the Collect databases, run by the AppHost
```

`Common/Fixtures/AspireFixture.cs` starts the AppHost once for the run, before any other fixture.
