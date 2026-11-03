# End-to-end testing

End-to-end tests drive the real app in a browser with [Playwright for .NET](https://playwright.dev/dotnet/). They live in `tests/SchoolAccount.EndToEndTests`.

## Signing in without DfE Sign-in

Locally and in a pipeline, the app signs in against [oidc-server-mock](https://github.com/Soluto/oidc-server-mock) instead of DfE Sign-in. It is a Docker image of Duende IdentityServer, configured entirely by the files in `tests/oidc-server-mock`:

- `clients.json` registers the app as a client. It uses the implicit flow (`response_type=id_token`), the same flow as DfE Sign-in.
- `identity-resources.json` adds the `organisation` scope, so the `organisation` claim goes into the ID token.
- `users.json` holds one user per test academy. The `organisation` claim has `"ValueType": "json"`, so it is sent as a JSON object. To add an academy that matches your seeded data, add a user to this file.

Each test user signs in with their username and the password `password`. There is no two-factor step or organisation picker.

oidc-server-mock is for development and testing only. Production use needs a Duende IdentityServer licence.

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

A test can skip the login form entirely. Sign in once, save the session with `StorageStateAsync`, and pass it to `BrowserFixture.NewContextAsync`. `SignInTests` shows both approaches.

## How the compose file is set up

- **Shared network.** IdentityServer builds its URLs from the host name of each request. The mock therefore runs inside the web container's network (`network_mode: "service:web"`), so the app and the browser both reach it at `http://localhost:5200` and the issuer always matches. Port 5200 is published on the `web` service for the same reason.
- **Plain HTTP.** The mock speaks plain HTTP. `Authentication__Schemes__OpenIdConnect__RequireHttpsMetadata=false` allows that. ASP.NET Core binds it from configuration, so no app code changes.
- **No pushed authorization requests.** IdentityServer advertises pushed authorization requests (PAR), and .NET uses them automatically when advertised. They are switched off in the mock to keep the app's flow close to DfE Sign-in's.
- **amd64 only.** The arm64 build of version 0.12.1 crashes on startup, so the compose file pins `platform: linux/amd64`. On Apple silicon it runs under emulation.

## Limitations

- The Collect and Academies APIs are not part of this compose setup. Pages that call them, such as the journey page, fail until those services are added or stubbed. The sample tests land on the start page, which needs neither.
- oidc-server-mock's last release was May 2025.
