# AGENTS.md

## Stack
ASP.NET Core 10, C# 14, Entity Framework Core.

## Language & Style
Follow [docs/coding-standards.md](docs/coding-standards.md). The `.editorconfig` is the
source of truth and violations fail the build, so run `dotnet build` before finishing.

## Architecture
- Follow SOLID principles
- Use dependency injection — no service locator patterns
- Prefer controller endpoints over minimal APIs by default; use minimal APIs only for simple endpoints or when explicitly requested
- Use middleware for cross-cutting concerns
- Use `BackgroundService`/`IHostedService` for long-running tasks

## Data & Config
- Entity Framework Core for database access
- Strongly-typed config via `IOptions<T>`
- Environment-specific `appsettings.{Environment}.json`

## Endpoints
- MVC controllers under `Features/`; no API controllers or minimal APIs exist today
- Mark every controller `[Authorize]` or `[AllowAnonymous]` explicitly

## Testing
Follow our [docs/testing-standards.md](docs/testing-standards.md) for unit tests and
[docs/integration-testing.md](docs/integration-testing.md) for endpoint tests.
- Unit test business logic
- Integration test API endpoints (WebApplicationFactory / AngleSharp / Playwright)
- Test command: `dotnet test`