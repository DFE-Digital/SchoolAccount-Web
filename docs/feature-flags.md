# Feature Flags

## Overview

Feature flags let us ship code for a feature before it's switched on, and switch it on or off per environment without
a release. We use [Microsoft.FeatureManagement](https://learn.microsoft.com/en-us/azure/azure-app-configuration/feature-management-dotnet-reference),
with defaults in the appsettings files and per-environment values in Azure App Configuration.

See [Use Microsoft.FeatureManagement with flags held in Azure App Configuration](../decisions/0013-use-feature-flags-from-azure-app-configuration.md)
for why.

## Where a flag's value comes from

| Where the app runs                  | Value comes from                                                           |
|-------------------------------------|----------------------------------------------------------------------------|
| Locally                             | `appsettings.Development.json`                                             |
| Integration tests                   | `appsettings.IntegrationTests.json`, or the test itself (see below)        |
| Deployed environments               | Azure App Configuration, falling back to `appsettings.json` if it's absent |

App Configuration is only used when `AzureAppConfiguration:Enabled` is true. The setup is in
`Hosting/Extensions/AzureAppConfigurationExtensions.cs` and `Hosting/Extensions/ConfigurationExtensions.cs`.

## Adding a flag

1. Name it. Use PascalCase and describe the feature, not the change, for example `FeedbackFooter`.
2. Add a constant to `FeatureFlags` in `src/SchoolAccount.Web.Mvc/Infrastructure/FeatureFlags.cs`. Always use the
   constant, never the string.

   ```csharp
   public static class FeatureFlags
   {
       public const string FeedbackFooter = "FeedbackFooter";
   }
   ```

3. Add a default to each appsettings file under `feature_management:feature_flags`:
   * `appsettings.json`: `false`, so deployed environments stay off until someone switches the flag on.
   * `appsettings.Development.json` and `appsettings.IntegrationTests.json`: `true`, so the feature can be worked on
     and tested.

   ```json
   "feature_management": {
     "feature_flags": [
       { "id": "FeedbackFooter", "enabled": false }
     ]
   }
   ```

4. Gate the feature (see below).
5. Test it on and off (see below).

## Gating a feature

### In a view

Wrap the markup in the `<feature>` tag helper:

```cshtml
<feature name="@FeatureFlags.FeedbackFooter">
    <partial name="~/Features/Feedback/_Feedback.cshtml" />
</feature>
```

The tag helper is registered in `Features/_ViewImports.cshtml`. Add `@using SchoolAccount.Web.Mvc.Infrastructure` to
the view for `FeatureFlags`.

### In a controller

If hiding the view isn't enough, for example a form that posts to an endpoint, gate the controller or action too:

```csharp
[FeatureGate(FeatureFlags.FeedbackFooter)]
public class FeedbackController(...) : Controller
```

When the flag is off, a gated endpoint returns 404.

### In code

Inject `IFeatureManager` and call `IsEnabledAsync(FeatureFlags.YourFlag)`. Prefer the tag helper or `[FeatureGate]`
where they fit.

## Testing a flag

Write integration tests for both states. Flags are on in `appsettings.IntegrationTests.json`, so tests for the
on state need nothing extra.

For the off state, call `DisableFeature` from `Common/Extensions/ServiceCollectionExtensions.cs` when creating the
client:

```csharp
var client = _factory.CreateAuthorisedClient(services =>
    services.DisableFeature(FeatureFlags.FeedbackFooter)
);
```

It turns off only the named flag. Other flags keep the state the appsettings give them, and calls can be chained to
turn off more than one.

If the feature has an endpoint gated with `[FeatureGate]`, test that it returns 404 when the flag is off.

## Switching a flag in an environment

1. In the Azure portal, open the environment's App Configuration store and go to **Feature manager**.
2. Create the flag with the same name as the constant, if it isn't there. Leave the label empty. The app only loads
   flags without a label.
3. Turn it on or off.

The change is picked up shortly after the next request, without a restart. The request that triggers the refresh
still sees the old value.

If a flag isn't in App Configuration, the app uses the default from `appsettings.json`, which is off.

## Removing a flag

Once a feature is settled on or off everywhere, remove its flag:

1. Remove the `<feature>` tag helpers, `[FeatureGate]` attributes and `IFeatureManager` checks. Keep the feature's code
   if it stays on, or remove the feature if it doesn't.
2. Remove the constant from `FeatureFlags`.
3. Remove the flag from each appsettings file.
4. Remove the tests for the state that no longer exists.
5. Delete the flag from App Configuration in each environment once the release without it is deployed.
