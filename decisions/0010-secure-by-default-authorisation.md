---
status: "accepted"
date: "2026-08-11"
decision-makers: 
---

# Enforce authentication by default via a global fallback authorisation policy

## Context and Problem Statement

Endpoints in the application have previously gone live without authorisation checks because they lacked an explicit `[Authorize]` 
attribute and no deliberate decision was made to mark them `[AllowAnonymous]`. This meant a simple omission, forgetting to 
add authorization metadata, was silently equivalent to making a page public. How do we ensure that every endpoint requires 
authentication unless a developer explicitly and deliberately opts it out?

## Decision Drivers

* Prior incident(s) where pages were unintentionally accessible without sign-in
* Desire for a "secure by default" (fail-safe default) posture, where missing configuration fails closed rather than open
* Minimal ongoing burden on developers adding new endpoints, correctness shouldn't depend on remembering a step
* Must remain compatible with the existing DfE Sign In authentication integration
* Must still support genuinely public endpoints (e.g. health checks, sign-in callback routes) without excessive friction

## Considered Options

* Rely on developer discipline to add `[Authorize]` to every new endpoint (status quo)
* Configure a global ASP.NET Core default + fallback authorisation policy requiring an authenticated user
* Enforce authentication via custom middleware or an endpoint filter applied globally
* Catch missing `[Authorize]` via static analysis / lint rule in CI

## Decision Outcome

Chosen option: "Configure a global ASP.NET Core default + fallback authorisation policy requiring an authenticated user", 
because it flips the default from opt-in to opt-out using a native, well-supported framework mechanism, closing the specific 
failure mode that caused prior incidents without introducing custom middleware to maintain.

```csharp
public static void AddDsiAuthentication(
    this IServiceCollection services,
    IConfigurationManager configuration
)
{
    // [...]
    
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
    services
        .AddAuthorizationBuilder()
        .SetDefaultPolicy(policy)
        .SetFallbackPolicy(policy);
}
```

`SetFallbackPolicy` applies to any endpoint with no authorisation metadata at all (no `[Authorize]`, no `[AllowAnonymous]`), 
which is the specific gap that let pages slip through previously. `SetDefaultPolicy` applies to endpoints that use `[Authorize]` 
without naming a specific policy.

### Consequences

* Good, because an endpoint with no authorisation metadata now fails closed (requires sign-in) instead of failing open.
* Good, because it uses a built-in ASP.NET Core mechanism, no custom middleware to write, test, or maintain.
* Good, because it's a single point of configuration, not dependent on every developer remembering a step per endpoint.
* Bad, because genuinely public endpoints now require an explicit `[AllowAnonymous]` (or `.AllowAnonymous()`), shifting a small amount of friction onto developers adding intentionally public routes.
* Bad, because without additional tooling, a developer could still add `[AllowAnonymous]` incorrectly, this policy prevents *accidental* omission, not deliberate misuse.

### Confirmation

Two automated tests (using xUnit + Shouldly) run in CI to validate this decision from complementary angles:

* **`AnonymousEndpointGuardrailTests.Endpoints_marked_as_AllowAnonymous_should_only_be_the_ones_on_the_allowlist`** enumerates all registered endpoints at runtime via `EndpointDataSource` and asserts that any endpoint carrying `[AllowAnonymous]`/`.AllowAnonymous()` metadata is present on an explicit, reviewed allowlist. It is resolved from final endpoint metadata rather than reflection alone, so it catches minimal API and Razor Page conventions in addition to controller attributes. This answers: *is anything anonymous that shouldn't be?*
* **`AuthenticatedEndpointsRequireSignInTests.Protected_endpoint_should_not_return_a_successful_response_when_request_is_unauthenticated`** sends real unauthenticated HTTP requests through the full pipeline to every non-allowlisted static GET route and asserts a 401/403/redirect comes back rather than a 200. Because it exercises the actual middleware pipeline rather than just inspecting metadata, it also catches configuration bugs the metadata check can't see, e.g. `UseAuthentication()`/`UseAuthorization()` missing or ordered incorrectly. This answers: *is protection actually enforced at runtime?*

N.B. Parameterised routes (e.g. `/users/{id}`) are excluded from the automatic HTTP-level check and should be covered by dedicated, explicit tests where they carry auth risk, this could change over time but felt a cleaner approach.

## Pros and Cons of the Options

### Rely on developer discipline

No framework or process change required.

* Good, because it requires no code change.
* Neutral, because it works correctly as long as every developer remembers every time.
* Bad, because this is exactly the approach that caused the prior incident(s), a single missed attribute silently exposes a page.
* Bad, because it doesn't scale with team size or codebase growth.

### Global default + fallback authorisation policy 

Native ASP.NET Core authorisation feature; requires authentication unless an endpoint is explicitly marked otherwise.

* Good, because it fixes the failure mode at its root: an endpoint can no longer be anonymous by accident.
* Good, because it's a small, well-understood, framework-native change.
* Neutral, because existing genuinely-anonymous endpoints need to be identified and explicitly marked, which is a one-off migration cost.
* Bad, because it doesn't prevent someone deliberately (if mistakenly) applying `[AllowAnonymous]`, this is mitigated separately by the guardrail test.

### Custom middleware / endpoint filter

A hand-rolled global filter that inspects each request and enforces authentication.

* Good, because it could be extended with custom logic beyond what policies support.
* Neutral, because it achieves a similar practical outcome to the fallback policy.
* Bad, because it duplicates functionality ASP.NET Core already provides, adding code to maintain and test.
* Bad, because it's less discoverable to developers familiar with standard ASP.NET Core authorisation conventions.

## More Information

* Implemented alongside a CI guardrail test (`AnonymousEndpointGuardrailTests`) that fails the build if an unlisted endpoint is found with `[AllowAnonymous]`/`.AllowAnonymous()`.
* A prior version of this policy included a `ProviderRequirement` (restricting auth to a specific identity provider); this was removed as out of scope for this decision and may be revisited separately if needed.
* This decision should be revisited if the application introduces a large number of genuinely public routes, at which point an allowlist-based approach may become harder to maintain than an explicit-opt-in-per-route approach.




