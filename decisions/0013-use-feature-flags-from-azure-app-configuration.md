---
status: "proposed"
date: 2026-10-16
decision-makers: Paul Custance
---

# Use Microsoft.FeatureManagement with flags held in Azure App Configuration

## Context and Problem Statement

We want to ship code for a feature before it's switched on, and switch it on or off per environment without a release.
The first case is the page feedback form above the footer. How do we toggle features like this, and where do the
settings live?

## Decision Drivers

* Turn a feature on or off per environment without building or deploying.
* Use what we already run. The app already reads settings from Azure App Configuration.
* Gate both the view and the endpoints behind it, so a hidden feature can't be used by posting to it directly.
* Run locally and in tests without App Configuration.
* Be able to test a feature both on and off.

## Considered Options

* Microsoft.FeatureManagement, with flags held in Azure App Configuration
* Plain boolean settings read through `IOptions<T>`
* A third-party feature flag service

## Decision Outcome

Chosen option: "Microsoft.FeatureManagement, with flags held in Azure App Configuration", because App Configuration is
already part of the app and has a feature manager for flags, and the library gives us a tag helper for views and an
attribute for controllers, so we don't write our own gating.

How it works:

* Flag names are constants in `FeatureFlags` (`Infrastructure/FeatureFlags.cs`), so views, controllers and tests use
  the same name.
* `appsettings.{Environment}.json` holds the default for each flag under `feature_management:feature_flags`, using the
  same schema App Configuration uses:
  * `appsettings.json` turns new features off, so a deployed environment only gets a feature when it's switched on in
    App Configuration.
  * `appsettings.Development.json` and `appsettings.IntegrationTests.json` turn them on, so they can be worked on and
    tested.
* When `AzureAppConfiguration:Enabled` is true, the app loads flags from App Configuration with `UseFeatureFlags()`.
  App Configuration is added after the appsettings files, so its value wins. Only flags without a label are loaded.
* When App Configuration is enabled, the app also registers its refresh middleware, so a changed flag is picked up
  shortly after the next request, without a restart.
* Views are gated with the `<feature>` tag helper, and controllers or actions with `[FeatureGate]`. A gated endpoint
  returns 404 when its flag is off.
* `services.AddFeatureManagement(configuration)` in `DependencyInjection.cs` registers the feature manager.

See [Feature flags](../docs/feature-flags.md) for how to add, test and remove a flag.

### Consequences

* Good, because a feature can be switched on or off in an environment from the Azure portal, with no release.
* Good, because local runs and tests don't need App Configuration. Flags come from the appsettings files.
* Good, because a gated controller can't be reached when its flag is off.
* Good, because the library supports filters, such as a percentage of users or a time window, if we need them later.
* Bad, because every flag is a branch in the code that has to be tested both ways and removed once the feature is
  settled.
* Bad, because a flag that's missing from App Configuration silently falls back to the appsettings default, which is
  off in deployed environments.
* Bad, because refresh isn't instant. The first request after a change still sees the old value.

### Confirmation

Integration tests check each gated feature with its flag on and off, including that gated endpoints return 404 when it's
off. Code review checks that new flags follow the [Feature flags](../docs/feature-flags.md) guidance.

## Pros and Cons of the Options

### Microsoft.FeatureManagement, with flags held in Azure App Configuration

* Good, because App Configuration is already loaded at startup, so this adds one package and a few lines of setup.
* Good, because App Configuration has a feature manager in the portal for switching flags.
* Good, because the tag helper and `[FeatureGate]` cover views and endpoints without custom code.
* Neutral, because it adds a dependency on `Microsoft.FeatureManagement.AspNetCore`.

### Plain boolean settings read through `IOptions<T>`

For example, a `Features:Feedback` setting bound to an options class and checked in views and controllers.

* Good, because it needs no new package.
* Bad, because we'd write and test our own gating for views and endpoints.
* Bad, because settings don't refresh unless we use `IOptionsSnapshot<T>` or `IOptionsMonitor<T>` and set up refresh
  ourselves.
* Bad, because flags would be mixed in with other settings in App Configuration, without the portal's feature manager.

### A third-party feature flag service

* Good, because these services usually offer richer targeting, auditing and experiments.
* Bad, because it's another service to procure, secure and run, when App Configuration already covers our needs.

## More Information

Revisit this if we need targeting by user or organisation beyond what the library's filters offer, or experiments with
measured outcomes.

See [Microsoft.FeatureManagement](https://learn.microsoft.com/en-us/azure/azure-app-configuration/feature-management-dotnet-reference)
and [Use feature flags in an ASP.NET Core app](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core).
