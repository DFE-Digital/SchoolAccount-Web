# End-to-end testing

End-to-end tests drive the real app in a browser with [Playwright for .NET](https://playwright.dev/dotnet/). They live in `tests/SchoolAccount.EndToEndTests`.

## Signing in without DfE Sign-in

Locally and in a pipeline, the app signs in against [mock-oauth2-server](https://github.com/navikt/mock-oauth2-server) instead of DfE Sign-in.

The mock's login page accepts any username and a JSON object of claims. Whatever claims a test sends go into the tokens, so each test chooses its academy as it signs in. Nothing about the users is configured in the mock. The test users and their `organisation` claims live in `TestUsers` in the test project; to add an academy that matches your seeded data, add an entry there.

### Code flow

mock-oauth2-server supports only the authorisation code flow. It rejects the implicit and hybrid flows. The app therefore uses code flow with PKCE (`response_type=code`). DfE Sign-in supports code flow too.

If DfE Sign-in issues the app a client secret, set `OpenIDConnectSettings:ClientSecret`. Leave it unset for a public client. The mock accepts either.

## Running the tests

Docker must be running. Then:

```bash
dotnet test --project tests/SchoolAccount.EndToEndTests
```

That's all. The tests start the app and the mock through the Aspire AppHost in `tests/SchoolAccount.AppHost`, run against them, and stop them at the end. Running or debugging a test from Rider or Visual Studio works the same way, with nothing to set up. The first run downloads Chromium and the mock's image.

The tests skip when the `CI` environment variable is set, because the pipeline has no step for them yet.

To run the stack on its own, for example to click around while signed in as a test academy, run the AppHost:

```bash
dotnet run --project tests/SchoolAccount.AppHost
```

The compose overlay, `docker-compose.e2e.yml`, still works for running the app's Docker image against the mock.

## Signing in once

`EndToEndApp` is shared by every test in the run. It signs in once per test user, the first time a test asks for them, and every later test gets a browser that starts already signed in. `StartPageTests` shows how to use it. `SignInTests` shows signing in through the login page, and reusing a saved session by hand.

## How the AppHost is set up

- **Same address for the app and the browser.** The mock builds its issuer from the host name of each request. The app runs as a normal process on this machine, like the browser, so both reach the mock at the same `localhost` address and the issuer always matches. No shared Docker network is needed.
- **Ports.** Aspire picks free ports each run. The tests read the app's address from Aspire, and the mock accepts any redirect address, so nothing depends on a fixed port.
- **Ready before the tests start.** The app waits for the mock's discovery document, and the tests wait for the app to serve a static file, so the first test never races start-up.
- **One issuer per path.** The mock serves a separate issuer for each URL path. The app uses `/default`.
- **Plain HTTP.** The mock speaks plain HTTP. `Authentication__Schemes__OpenIdConnect__RequireHttpsMetadata=false` allows that. ASP.NET Core binds it from configuration, so no app code changes are needed for it.

## Limitations

- The Collect and Academies APIs are not part of the AppHost yet. Pages that call them, such as the journey page, fail until those services are added, as containers or projects, or stubbed. The sample tests land on the start page, which needs neither.
- The mock is deliberately permissive: it accepts any client, redirect address and secret. It tests the app's sign-in wiring, not DfE Sign-in's rules.
