# End-to-end testing

End-to-end tests drive the real app in a browser with [Playwright for .NET](https://playwright.dev/dotnet/). They live in `tests/SchoolAccount.EndToEndTests`.

## Signing in without DfE Sign-in

Locally and in a pipeline, the app signs in against [mock-oauth2-server](https://github.com/navikt/mock-oauth2-server) instead of DfE Sign-in.

The mock's login page accepts any username and a JSON object of claims. Whatever claims a test sends go into the tokens, so each test chooses its academy as it signs in. Nothing about the users is configured in the mock. The test users and their `organisation` claims live in `TestUsers` in the test project; to add an academy that matches your seeded data, add an entry there.

### Code flow

mock-oauth2-server supports only the authorisation code flow. It rejects the implicit and hybrid flows. The app therefore uses code flow with PKCE (`response_type=code`). DfE Sign-in supports code flow too.

If DfE Sign-in issues the app a client secret, set `OpenIDConnectSettings:ClientSecret`. Leave it unset for a public client. The mock accepts either.

## Running the tests

Start the app and the mock:

```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.e2e.yml up --build -d
```

Run the tests against it:

```bash
E2E_BASE_URL=http://localhost:5100 dotnet test --project tests/SchoolAccount.EndToEndTests
```

Without `E2E_BASE_URL` the tests skip, so `dotnet test` on the solution stays green. Debugging a test from Rider or Visual Studio runs it against `http://localhost:5100` without setting anything. The first run downloads Chromium.

## Signing in once

A test can skip the login page entirely. Sign in once, save the session with `StorageStateAsync`, and pass it to `BrowserFixture.NewContextAsync`. `SignInTests` shows both approaches.

`SignedInSessions` does this for the whole run. It signs in once per test user, the first time a test asks for them, and every later test gets a browser that starts already signed in. `StartPageTests` shows how to use it.

## How the compose file is set up

- **Shared network.** The mock builds its issuer from the host name of each request. It therefore runs inside the web container's network (`network_mode: "service:web"`), so the app and the browser both reach it at `http://localhost:5200` and the issuer always matches. Port 5200 is published on the `web` service for the same reason.
- **One issuer per path.** The mock serves a separate issuer for each URL path. The app uses `http://localhost:5200/default`.
- **Plain HTTP.** The mock speaks plain HTTP. `Authentication__Schemes__OpenIdConnect__RequireHttpsMetadata=false` allows that. ASP.NET Core binds it from configuration, so no app code changes are needed for it.

## Limitations

- The Collect and Academies APIs are not part of this compose setup. Pages that call them, such as the journey page, fail until those services are added or stubbed. The sample tests land on the start page, which needs neither.
- The mock is deliberately permissive: it accepts any client, redirect address and secret. It tests the app's sign-in wiring, not DfE Sign-in's rules.
