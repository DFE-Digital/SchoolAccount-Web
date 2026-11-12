# Code review: SchoolAccount.E2ETests

Review of the e2e suite after it was moved in from `EfficientTestDemo` (branch `SAB-577_E2E_Tests`, 11 November 2026).
Work through the items in order. Tick an item off once it's fixed and the suite has been run against the local stack.

Severity:

- **Blocker**: a test can pass without proving anything, or the suite can damage data.
- **High**: likely to cause flaky or misleading failures.
- **Medium**: design problems that make the suite harder to trust or change.
- **Low**: maintenance and tidy-up.

## Summary

| #  | Severity | Item                                                         | Done |
|----|----------|--------------------------------------------------------------|------|
| 1  | Blocker  | Sync test passes without exercising the sync on reruns       | Accepted |
| 2  | Blocker  | Tab-order test passes when the snapshot is missing           | [x]  |
| 3  | Blocker  | Accessible-name check uses an incorrect algorithm            | [x]  |
| 4  | Blocker  | Journey page assertions only check "not empty"               | Deferred |
| 5  | Blocker  | Destructive database tests can run against any server        | [x]  |
| 6  | Blocker  | CA2100 suppression justification is inaccurate               | [x]  |
| 7  | High     | Login doesn't check it succeeded                             | [x]  |
| 8  | High     | "Sign in or register" click is a race                        | [x]  |
| 9  | High     | Test teardown throws when setup fails                        | [x]  |
| 10 | High     | Suite silently assumes an establishment account              | Accepted |
| 11 | High     | Two settings for the site URL can disagree                   | [x]  |
| 12 | High     | Tab-order snapshot depends on Collect API content            | [ ]  |
| 13 | Medium   | Start page test asserts a template default title             | [ ]  |
| 14 | Medium   | Configuration precedence is reversed and nothing is validated | [ ]  |
| 15 | Medium   | Components inherit from `BasePage`                           | [ ]  |
| 16 | Medium   | CSS-based locators instead of role and label locators        | [ ]  |
| 17 | Medium   | Dead and inconsistent settings                               | [ ]  |
| 18 | Low      | No README: nobody else can run the suite                     | [ ]  |
| 19 | Low      | Screenshots and videos accumulate forever                    | [ ]  |
| 20 | Low      | Package versions live outside `Directory.Packages.props`     | [ ]  |
| 21 | Low      | CI never builds the project                                  | [ ]  |
| 22 | Low      | Smaller code issues                                          | [ ]  |

---

## Blockers

### 1. Sync test passes without exercising the sync on reruns

[CollectData/CollectDatabaseTests.cs](CollectData/CollectDatabaseTests.cs) - `Sync_copies_a_portal_status_change_into_the_state_ledger`

The test always writes the same values (status 3, errors 1/2/3) to `COLLECTPortal`, then runs
`AddChangedCollectReturnStatus`, which only records *changes*. From the second run onwards nothing changes, no row is
written, and the test reads the newest existing row and passes. On 11 November 2026 the newest status-3 row for
`8612009` was dated 2 October 2026, despite two runs that day.

The test also leaves `COLLECTPortal` modified.

**Fix:** read the current status first, write a different one, assert a new ledger row appeared with that status and a
later `UpdatedAt`, then restore the original status.

**Decision (11 November 2026): accepted as is.** For now the test only needs to show the suite can reach the databases
and write a row. Revisit if it's meant to test the sync procedure itself.

### 2. Tab-order test passes when the snapshot is missing

[Features/Journey/JourneyUsabilityTests.cs](Features/Journey/JourneyUsabilityTests.cs) - `Tab_order_matches_the_saved_snapshot`

If `Journey.tab-order.json` is missing, the test writes the current tab order to it, compares it with itself and
passes. Deleting or renaming the snapshot produces a green test that checked nothing.

**Fix:** fail when the snapshot is missing, writing the captured order to a `.received.json` file for review instead.

**Done (11 November 2026):** without adding a package. When the snapshot is missing or differs, the test fails and
writes the order it found to `Journey.tab-order.received.json` with instructions to review and rename it. The received
file is deleted once the test passes again and is ignored by git (`.gitignore` in this folder). The tab-capturing loop
moved out of the test body into a helper.

### 3. Accessible-name check uses an incorrect algorithm

[Features/Journey/JourneyUsabilityTests.cs](Features/Journey/JourneyUsabilityTests.cs) - `Every_visible_interactive_element_has_an_accessible_name`

The hand-written script treats `aria-label`, `textContent` or `value` as the name. It ignores `<label for>`,
`aria-labelledby` and `title`, and counts visually hidden text as a name. It only scans `/Journey`, where the feedback
textarea is hidden, so it never checks a form field. It also stops at the first failure and reports only the tag name.

**Fix:** replace it with an axe scan (`Deque.AxeCore.Playwright`), run against each page, reporting every violation with
its selector.

**Done (11 November 2026):** the test now runs axe-core's accessible-name rules (links, buttons, form labels, images,
`<summary>` and ARIA roles) through `PageAccessibilityExtensions.FindElementsWithoutAccessibleNamesAsync`, and lists
every failing element with its HTML, selector and guidance link. Still only runs on `/Journey`; pointed at `/Feedback`
it reports the unlabelled feedback textarea.

### 4. Journey page assertions only check "not empty"

[Features/Journey/JourneyPageTests.cs](Features/Journey/JourneyPageTests.cs)

All six tests assert presence only: name not empty, status not empty, at least one step, dates not empty, heading
contains "census". They would pass with any content at all.

**Fix:** assert the known values for the local test account (for example "East Herrington Primary Academy",
"Unavailable", census day "1 October 2026"), kept in one place so they're easy to update.

**Decision (11 November 2026): deferred.** The tests currently prove the suite can sign in and reach the page and
database. Improve the assertions in the near future.

### 5. Destructive database tests can run against any server

[Common/Database/TestDatabase.cs](Common/Database/TestDatabase.cs),
[Common/Database/CollectPortalScripts.cs](Common/Database/CollectPortalScripts.cs)

`Database:Server` can be overridden by user secrets or environment variables. Pointed at a shared server, the suite
updates `COLLECTPortal.dbo.DataReturn` and inserts rows into the state ledger there. `TrustServerCertificate = true`
removes the last warning sign.

**Fix:** refuse to run unless the server is local (`localhost`, `127.0.0.1` or `host.docker.internal`), with a clear
message.

**Done (11 November 2026):** `TestDatabase` refuses any `Database:Server` whose host isn't `localhost` or a `127.x.x.x`
address (any port, instance name or `tcp:` prefix is allowed), before connecting. The rule is deliberately in code with
no setting to override it: running against another server means changing `EnsureServerIsLocal`.

### 6. CA2100 suppression justification is inaccurate

[Common/Database/TestDatabase.cs](Common/Database/TestDatabase.cs)

The justification says values are passed as parameters. `DeleteAsync` takes a raw `whereClause` string, and table
names are interpolated into SQL.

**Fix:** make `DeleteAsync` take ids (parameterised), keep table names as constants, then correct the justification.

**Done (11 November 2026):** `DeleteAsync(tableName, whereClause)` is replaced by `DeleteByIdsAsync(tableName, ids)`,
which passes each id as a SQL parameter. The justification now says SQL text is only built from constants and record
property names, and every value is a parameter, which is true.

---

## High

### 7. Login doesn't check it succeeded

[Common/Pages/SchoolAccountLoginPage.cs](Common/Pages/SchoolAccountLoginPage.cs),
[Common/Fixtures/SchoolAccountBrowserFixture.cs](Common/Fixtures/SchoolAccountBrowserFixture.cs)

After clicking Sign in it waits for `NetworkIdle` (which Playwright advises against) and saves the browser state
whatever happened. A wrong password, an organisation picker or a locked account all surface later as unrelated
timeouts in every Journey test.

**Fix:** wait for the URL to be `/Journey` and fail the fixture with "Sign-in failed" otherwise.

**Done (12 November 2026):** sign-in only counts as successful once DfE Sign-in returns the browser to `/Journey`
(30 second limit); the `NetworkIdle` wait is gone. Failed attempts are retried from scratch in a new browser context,
up to 3 times, and the final error lists every attempt's reason. Only Playwright failures (timeouts, pages that didn't
load) are retried. If DfE Sign-in shows an error summary, sign-in stops immediately without retrying, so a wrong
password can't lock the account.

Not yet verified: the error-summary check assumes DfE Sign-in shows rejected credentials in a GOV.UK error summary
(`.govuk-error-summary`). This hasn't been tried against a real wrong password, to avoid locking the test account.

### 8. "Sign in or register" click is a race

[Common/Pages/SchoolAccountLoginPage.cs](Common/Pages/SchoolAccountLoginPage.cs)

`IsVisibleAsync()` doesn't wait, so if the button renders late the click is skipped and the next step times out.

**Fix:** wait for either the button or the email field to appear, then act on whichever did.

**Done (12 November 2026):** the login page now waits for whichever of the two appears first before deciding whether
to click "Sign in or register".

### 9. Test teardown throws when setup fails

[Common/Fixtures/BrowserTestBase.cs](Common/Fixtures/BrowserTestBase.cs)

If `NewContextAsync` throws, `_context` is null and `DisposeAsync` throws, hiding the real error. If the screenshot
throws, the context is never closed and the video is never handled.

**Fix:** null checks and `try/finally` so the context is always closed and the original failure is the one reported.

**Done (12 November 2026):** `BrowserTestBase` skips teardown if the browser context was never created, closes the
context in a `finally` (so a failed screenshot can't stop it closing), and only handles the screenshot and video if the
page exists. `BrowserFixture` does the same for the browser and Playwright. `Page` and `Browser` now throw a clear
error if used before setup, instead of being `null!`. Checked by forcing a setup failure: the test reported the
original error, not a `NullReferenceException`.

### 10. Suite silently assumes an establishment account

[Features/Journey/JourneyPageTests.cs](Features/Journey/JourneyPageTests.cs)

The status tag and Go to COLLECT button are only shown to establishments. Sign in with a trust account and those tests
fail for reasons unrelated to any bug.

**Fix:** document the required account type, and check it in the fixture so a wrong account fails clearly.

**Decision (12 November 2026): accepted as is.** Whoever runs the suite knows it needs an establishment account.

### 11. Two settings for the site URL can disagree

[Features/Start/StartPageTests.cs](Features/Start/StartPageTests.cs) uses `BaseUrl` (appsettings.json); the Journey
tests use `SchoolAccount:Url` (user secrets).

**Fix:** use one setting for the site URL everywhere.

**Done (12 November 2026):** `BaseUrl` is gone. Every test uses `Settings.SiteUrl`, which reads `SchoolAccount:Url`
from user secrets first, then environment variables, then `appsettings.json` (`https://127.0.0.1:7041`). Checked by
setting `SchoolAccount__Url` to a bogus address: the user-secrets value still won and the Start page test passed.

### 12. Tab-order snapshot depends on Collect API content

[Features/Journey/Journey.tab-order.json](Features/Journey/Journey.tab-order.json)

Step titles come from the Collect API, so a content change upstream fails a keyboard-accessibility test.

**Fix:** decide whether the test checks the order of *kinds* of control (header, step buttons, call to action, footer)
rather than exact text, or accept the coupling and say so in the test.

---

## Medium

### 13. Start page test asserts a template default title

[Features/Start/StartPageTests.cs](Features/Start/StartPageTests.cs)

It asserts the browser tab title contains "Home Page", the ASP.NET template default in `Start.cshtml`, then calls
`AssertTitle`, which checks the `<h1>`. The variable is still called `homePage`.

**Fix:** split into two tests: the tab title should be descriptive (expected to fail until `Start.cshtml` is fixed) and
the heading says "Sign in to School Account". Rename `AssertTitle` to `AssertHeading` and the variable to `startPage`.

### 14. Configuration precedence is reversed and nothing is validated

[Common/Configuration/ConfigurationLoader.cs](Common/Configuration/ConfigurationLoader.cs)

User secrets are added after environment variables, so secrets override them. Missing settings are only discovered as
a sign-in or connection failure much later.

**Fix:** load user secrets before environment variables. Validate required settings at startup with a message naming
the missing key.

**Decision (12 November 2026), from item 11:** user secrets deliberately take priority over environment variables, so
the first half of this item no longer applies. Only the validation is left.

### 15. Components inherit from `BasePage`

[Common/Pages/Components/](Common/Pages/Components/)

Components inherit `BasePage` only to get the page, so they expose `AssertTitle()`, which checks the page heading.
`ImportantDatesComponent.GetDateRowValue` searches the whole page rather than the Important dates section.

**Fix:** give components their own root locator and no page-level methods.

### 16. CSS-based locators instead of role and label locators

[Common/Extensions/PageLocatorExtensions.cs](Common/Extensions/PageLocatorExtensions.cs)

Wrappers like `InputLocatorById` build CSS selectors. Playwright's `GetByRole` and `GetByLabel` are more robust and
also prove elements are accessible: they would have caught the unlabelled feedback textarea.

**Fix:** move page objects to role and label locators and delete the extension class.

### 17. Dead and inconsistent settings

- `Database:DatabaseName` and `DatabaseSettings.DatabaseName` are never used; database names are hard-coded in
  [Common/Database/TestDatabases.cs](Common/Database/TestDatabases.cs).
- `CaptureSettings` uses `set`; the other settings classes use `init`.
- `xunit.runner.json` only contains `$schema`.

**Fix:** remove the unused setting and file, and use `init` throughout.

---

## Low

### 18. No README: nobody else can run the suite

Running the suite needs the full Docker stack (started by a script outside the repo), six user-secret keys, a DfE
Sign-in account for an establishment, Playwright browsers installed, and the local compose override, which only exists
committed on another branch.

**Fix:** add a README covering prerequisites, the secret keys, installing browsers, how to run, and what each test
group needs.

### 19. Screenshots and videos accumulate forever

[appsettings.json](appsettings.json), [Common/TestArtifactPaths.cs](Common/TestArtifactPaths.cs)

Capture is on for passing tests by default, file names are timestamped, and nothing cleans them up. Video recording also
slows every test.

**Fix:** capture on failure only by default, and clear the artifact folders at the start of a run.

### 20. Package versions live outside `Directory.Packages.props`

[SchoolAccount.E2ETests.csproj](SchoolAccount.E2ETests.csproj)

Kept here so the suite is self-contained in `tests/`. Dependabot's groups are organised around
`Directory.Packages.props`, so these may not be updated. `Microsoft.Extensions.Configuration` 10.0.9 sits alongside
10.0.11 and 10.0.13 elsewhere in the repo.

**Fix:** confirm whether Dependabot picks them up. If not, note them for manual updates or move them to
`Directory.Packages.props`.

### 21. CI never builds the project

A deliberate decision: the suite runs locally against a running site. Analyser violations and broken code will only be
found when someone runs it. CI only checks formatting.

**Fix:** none needed now. Revisit if the suite becomes part of a release check.

### 22. Smaller code issues

- `using` instead of `await using` on `SqlConnection`
  ([Common/Database/TestDatabase.cs](Common/Database/TestDatabase.cs)).
- `AddWithValue` infers SQL types.
- Bogus data has no fixed seed, so failures can't be reproduced
  ([Common/Database/CollectReturnStatusFaker.cs](Common/Database/CollectReturnStatusFaker.cs)).
- Fake `LAEStab` values look like `999/1234`; real ones are 7 digits, like `8612009`.
- `null!` and `!` throughout, against the repo's testing standards.
- Everything is `public` and unsealed; `AGENTS.md` asks for `internal sealed` by default. Test classes and fixtures
  must stay public.
