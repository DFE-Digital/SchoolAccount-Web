# Component Testing

Component tests render a single Razor view or partial with a test-built model and assert against the resulting HTML.
They cover the states a component can be in (empty lists, missing statuses, optional attributes) without the
authenticated requests, stubbed handlers and full pages that integration tests need.

## How it works

[ComponentRenderer](../tests/SchoolAccount.Web.Mvc.ComponentTests/Common/ComponentRenderer.cs) boots the site once per
test class with `WebApplicationFactory` (`IntegrationTests` environment), then renders views directly through
`IRazorViewEngine`, with no HTTP request, controller or authentication.

- Views render as partials, so `_Layout` isn't applied.
- Links (`Url.Action`, `asp-controller`) are generated from the app's real routes.
- Short partial names (`<partial name="_StartCards" />`) resolve from the view's feature folder, as they do at runtime.
- Tests can supply `tempData` and a `requestUrl` for views that read them:

  ```csharp
  await renderer.RenderAsync<object?>(ViewPath, null,
      tempData: new Dictionary<string, object> { [TempDataKeys.FeedbackSubmitted] = true },
      requestUrl: "/Journey?year=2026");
  ```

- The output is parsed with [AngleSharp](https://anglesharp.github.io/) and queried with CSS selectors.

## Writing a component test

```csharp
public sealed class MultipleSchoolsStatusTableTests(ComponentRenderer renderer)
    : IClassFixture<ComponentRenderer>
{
    private const string ViewPath =
        "~/Features/Shared/MultipleSchoolsStatusTable/MultipleSchoolsStatusTable.cshtml";

    [Fact]
    public async Task Table_body_is_empty_when_the_trust_has_no_schools()
    {
        var model = ATableOf();

        using var html = await renderer.RenderAsync(ViewPath, model);

        html.QuerySelectorAll("tbody tr").ShouldBeEmpty();
    }
}
```

- Put the test class under `Features/`, mirroring the view's folder in the web project.
- View paths must start with `~/Features/`; the renderer uses the path to work out the feature folder.
- Follow the [testing standards](./testing-standards.md).

## What to assert on

Assert on what users, assistive technology or the page's JavaScript depend on: visible text, conditional sections,
order, `aria-*` attributes, and the ids, `data-*` attributes and classes the JavaScript and GOV.UK styles rely on.
Don't compare whole HTML strings.

## Running

See the [component tests README](../tests/SchoolAccount.Web.Mvc.ComponentTests/README.md). The tests also run as part
of `dotnet test` on the solution and in CI.
