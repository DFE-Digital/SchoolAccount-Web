# End-to-end testing

End-to-end tests drive the real app in a browser with [Playwright for .NET](https://playwright.dev/dotnet/). They live in `tests/SchoolAccount.EndToEndTests`.

## Signing in without DfE Sign-in

Locally and in a pipeline, the app signs in against [Authelia](https://www.authelia.com/) instead of DfE Sign-in. Authelia is an open-source authentication server with an OpenID Connect provider, configured by the files in `tests/authelia`:

- `configuration.yml` registers the app as a client. It uses the implicit flow (`response_type=id_token`), the same flow as DfE Sign-in, and puts every claim the app needs into the ID token.
- `users.yml` holds one user per test academy. To add an academy that matches your seeded data, add a user to this file.
- `generate-secrets.sh` creates Authelia's TLS certificate, token signing key and secrets the first time the stack starts. Nothing secret is committed; the generated files live in the `authelia-secrets` Docker volume.

Each test user signs in with their username and the password `password`. There is no two-factor step or organisation picker.

Authelia's custom attributes can only be strings, integers or booleans, so the `organisation` claim is sent as a JSON string rather than a JSON object. The app accepts either.

## Running the tests

Start the app and Authelia:

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

- **HTTPS and a real-looking domain.** Authelia insists on HTTPS and on a cookie domain containing a period, so `localhost` won't do. It runs at `https://auth.localtest.me:5200`. `localtest.me` is public DNS that always resolves to 127.0.0.1. The web container also maps it to 127.0.0.1 with `extra_hosts`, so the app doesn't depend on that DNS.
- **Shared network.** Authelia runs inside the web container's network (`network_mode: "service:web"`), so the app and the browser reach it at the same address and the issuer always matches. Port 5200 is published on the `web` service for the same reason.
- **Trusting the certificate.** The web container trusts Authelia's self-signed certificate through `SSL_CERT_DIR`, which adds the generated certificate to the usual CAs. The Playwright tests set `IgnoreHTTPSErrors`. No app code changes.
- **Pushed authorization requests.** Authelia advertises pushed authorization requests (PAR), and .NET uses them automatically when advertised. Neither side can switch them off through configuration alone. The app therefore sends its sign-in request to Authelia over a back channel first, which DfE Sign-in's flow doesn't do.

To start again with fresh secrets, remove the volume:

```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.e2e.yml down --volumes
```

## Limitations

- The Collect and Academies APIs are not part of this compose setup. Pages that call them, such as the journey page, fail until those services are added or stubbed. The sample tests land on the start page, which needs neither.
- Signing in needs the browser to resolve `auth.localtest.me`, which relies on public DNS. Some networks block DNS answers that point at 127.0.0.1.
