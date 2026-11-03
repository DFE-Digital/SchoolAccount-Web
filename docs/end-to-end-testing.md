# End-to-end testing

End-to-end tests drive the real app in a browser with [Playwright for .NET](https://playwright.dev/dotnet/). They live in `tests/SchoolAccount.EndToEndTests`.

## Signing in without DfE Sign-in

Locally and in a pipeline, the app signs in against a mock identity provider (`tests/SchoolAccount.MockIdentityProvider`) instead of DfE Sign-in. The mock implements only what the app uses: OpenID Connect with `response_type=id_token` and `response_mode=form_post`.

The mock shows one button per test user. Each user belongs to one academy, set by the `organisation` claim in `test-users.json`. Signing in as a different academy is one click, with no password, two-factor step or organisation picker. To add an academy that matches your seeded data, add a user to that file.

The mock is a test tool. Every endpoint is anonymous and it signs any token you ask for, so never deploy it.

## Running the tests

Start the app and the mock:

```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.e2e.yml up --build -d
```

Run the tests against it:

```bash
E2E_BASE_URL=http://localhost:5100 dotnet test --project tests/SchoolAccount.EndToEndTests
```

Without `E2E_BASE_URL` the tests skip, so `dotnet test` on the solution stays green. The first run downloads Chromium.

## Signing in once

Signing in through the mock is quick, but a test can skip it entirely. Sign in once, save the session with `StorageStateAsync`, and pass it to `BrowserFixture.NewContextAsync`. `SignInTests` shows both approaches.

## Limitations

- The Collect and Academies APIs are not part of this compose setup. Pages that call them, such as the journey page, fail until those services are added or stubbed. The sample tests land on the start page, which needs neither.
- The app reaches the mock over plain HTTP inside the Docker network. `docker-compose.e2e.yml` sets `Authentication__Schemes__OpenIdConnect__RequireHttpsMetadata=false` for this. ASP.NET Core binds that from configuration, so no app code changes.
