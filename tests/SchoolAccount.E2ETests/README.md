# SchoolAccount.E2ETests

Playwright E2E Tests that run against the local dockerised/containerised version of the application. 

In theory this can be pointed at any website, but due to the difficulties with the sign-in system, this is primarily setup for local use.

## What needs to be running

The suite doesn't start anything itself. Before running it, you need:

| Service       | Where                                                   | Needed by                     |
|---------------|---------------------------------------------------------|-------------------------------|
| School Account web app | `https://127.0.0.1:7041`                       | Every browser test            |
| Collect API   | Wherever the web app is configured to call it            | Journey tests                 |
| SQL Server    | `localhost,1433`, with `COLLECTPortal` and `CollectStateLedger` | Database tests and Journey tests |


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

### Playwright browser

Build once, then install Chromium. This needs [PowerShell](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
(`brew install powershell` on macOS).

```bash
dotnet build tests/SchoolAccount.E2ETests
pwsh tests/SchoolAccount.E2ETests/bin/Debug/net10.0/playwright.ps1 install chromium
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

## Configuration

Settings come from `appsettings.json`, then environment variables, then user secrets. Later sources win, so user
secrets take priority.

| Setting                  | Default                     | Notes                                                    |
|--------------------------|-----------------------------|----------------------------------------------------------|
| `SchoolAccount:Url`      | `https://127.0.0.1:7041`    | The site under test                                      |
| `SchoolAccount:Email`    | none                        | User secrets. Required for the signed-in tests           |
| `SchoolAccount:Password` | none                        | User secrets. Required for the signed-in tests           |
| `Database:Server`        | `localhost,1433`            | Must be `localhost` or a `127.x.x.x` address (see below) |
| `Database:UserId`        | none                        | User secrets                                             |
| `Database:Password`      | none                        | User secrets                                             |
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
  wording change there also fails this test.
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
```
