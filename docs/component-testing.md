# Component Testing

## Overview

A component test renders a single piece of UI - in this project a Razor partial such as
[StepByStep](../src/SchoolAccount.Web.Mvc/Features/Shared/StepByStep/StepByStep.cshtml) - with a model the test
builds, then checks the HTML it produces.

It sits between the existing test types:

| Test type        | What runs                                   | Answers                                              |
|------------------|---------------------------------------------|------------------------------------------------------|
| Unit             | A class or method                           | Is the logic correct?                                |
| **Component**    | **One view/partial + the Razor engine**     | **Does this component render correctly for model X?** |
| Integration      | Routing, auth, controller, full page        | Is the stack wired up and does the page respond?     |

Integration tests can check markup too, but every scenario needs an authenticated HTTP request, stubbed handlers and a
whole page to search through. Component tests skip all of that, which makes it cheap to cover every *state* a
component can be in - empty lists, missing statuses, optional attributes - and keeps each failure pointing at one
component.

## How it works

[ComponentRenderer](../tests/SchoolAccount.Web.Mvc.ComponentTests/Common/ComponentRenderer.cs) does four things:

1. Boots the site once with `WebApplicationFactory` so the real Razor view engine and tag helpers are available.
2. Renders the requested view with your model into a string - no HTTP request, controller or authentication. Links
   (`Url.Action`, `asp-controller`) are generated from the app's real routes, and short partial names
   (`<partial name="_StartCards" />`) resolve from the view's own feature folder, just as they do at runtime.
3. Lets a test supply `tempData` (values the view reads from `TempData`, such as validation errors after a redirect)
   and a `requestUrl` (the page the view is pretending to be on). For example:

   ```csharp
   await renderer.RenderAsync<object?>(ViewPath, null,
       tempData: new Dictionary<string, object> { ["FeedbackSubmitted"] = true },
       requestUrl: "/Journey?year=2026");
   ```
4. Parses the string with [AngleSharp](https://anglesharp.github.io/) into a DOM you query with CSS selectors, just as
   you would in browser dev tools.

## Writing a component test

```csharp
public class MultipleSchoolsStatusTableTests(ComponentRenderer renderer)
    : IClassFixture<ComponentRenderer>
{
    private const string ViewPath =
        "~/Features/Shared/MultipleSchoolsStatusTable/MultipleSchoolsStatusTable.cshtml";

    [Fact]
    public async Task Table_body_is_empty_when_the_trust_has_no_schools()
    {
        // Arrange - build the model for the state you want to test
        var model = ATableOf(); // no schools

        // Act - render just this component
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert - query the rendered HTML
        html.QuerySelectorAll("tbody tr").ShouldBeEmpty();
    }
}
```

To test a new component:

1. Add a test class under `Features/` mirroring the view's folder in the web project.
2. Take `ComponentRenderer` via `IClassFixture<ComponentRenderer>` (the site boots once per class).
3. Write one test per behaviour, following the [testing standards](./testing-standards.md).

## Reading the tests - suggested order

Each test file starts with a summary of the component it tests. Each test follows Arrange / Act / Assert, with a
comment on *why* the behaviour matters wherever that isn't obvious. Suggested order, simplest first:

1. [StartCardsTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Start/StartCardsTests.cs) - no model,
   fixed content. The basic shape of a test.
2. [DashboardTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Dashboard/DashboardTests.cs) - a simple
   model, plus checking that text is HTML-encoded.
3. [ErrorTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Error/ErrorTests.cs) - `[Theory]` tests
   that run once per input.
4. [MultipleSchoolsStatusTableTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Shared/MultipleSchoolsStatusTable/MultipleSchoolsStatusTableTests.cs)
   - helper methods that build test data, and accessibility checks.
5. [StepByStepTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Shared/StepByStep/StepByStepTests.cs) - checking
   the hooks that JavaScript relies on.
6. [ErrorSummaryTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Shared/ErrorSummaryTests.cs) - a
   component driven by `TempData` instead of a model.
7. [FeedbackTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Feedback/FeedbackTests.cs) - a form with
   three states (not sent, sent with an error, sent), each with its own render helper.
8. [JourneyTests](../tests/SchoolAccount.Web.Mvc.ComponentTests/Features/Journey/JourneyTests.cs) - a page with
   lots of conditional sections, each tested "shown when…" and "hidden when…".
9. [ComponentRenderer](../tests/SchoolAccount.Web.Mvc.ComponentTests/Common/ComponentRenderer.cs) - how rendering
   works under the hood.

## Glossary

| Term | Meaning |
|------|---------|
| **DOM** | The tree of elements a browser builds from HTML. AngleSharp builds the same tree, so tests can search it. |
| **CSS selector** | A pattern for finding elements: `.name` (has class), `#id` (has id), `a[href='/x']` (attribute value), `ul li` (inside), `ul > li` (directly inside), `h2 + p` (directly after). |
| **`QuerySelector` / `QuerySelectorAll`** | Find the first matching element (or `null`), or all matching elements. |
| **`TextContent`** | All the text inside an element, with tags removed. Often needs `.Trim()` because of whitespace in the `.cshtml`. |
| **`ShouldBe`, `ShouldContain`, `ShouldNotBeNull`…** | [Shouldly](https://docs.shouldly.org/) assertions. They fail the test with a readable message if the check isn't true. |
| **`[Fact]` / `[Theory]`** | An xUnit test that runs once, or once per `[InlineData]` row of inputs. |
| **`IClassFixture<T>`** | xUnit creates one `T` and shares it between every test in the class. Used here so the app starts once per class. |
| **`with { … }`** | Copies a C# record, changing only the properties listed. |
| **HTML encoding** | Razor converts `<` to `&lt;` (and so on) so data is shown as text and can't inject HTML or scripts. |
| **TempData** | Data that survives one redirect. Used here to carry form errors or a "sent" flag back to the page the form was on. |
| **Visually hidden text** | Text with the `govuk-visually-hidden` class: read aloud by screen readers but not shown on screen. |

## What to assert on

Assert on what a user, assistive technology or the page's JavaScript depends on:

- **Visible text** - headings, labels, table cells, tag text
- **Conditional content** - shown when X, hidden when Y
- **Structure and order** - number of rows/steps and their order
- **Behavioural hooks** - ids, `data-*` attributes and classes that JavaScript or CSS rely on (e.g. `govuk-tag--grey`)
- **Accessibility** - `scope` on headers, `aria-*` attributes, visually hidden text

Avoid comparing the whole HTML string. Whitespace or harmless markup changes would break the test without anything
being wrong, and the failure wouldn't tell you *what* broke.

## Bugs these tests caught when introduced

All of these rendered without errors and looked fine at a glance:

- `MultipleSchoolsStatusTable` rows had `lass="govuk-table__row"` instead of `class=...`, so rows never received GOV.UK
  styling.
- `StepByStep` built `data-remember-state="true"` as a string, which Razor HTML-encoded, producing an attribute value of
  `"true"` (quotes included). The first attempted fix, `data-remember-state="@(... ? "true" : null)"`, was also caught:
  Razor omits null-valued attributes *except* `data-*` ones, so the attribute was always present. It now writes the
  attribute with `Html.Raw`.
- `StepByStep` ignored `StepByStepViewModel.IsOpen`. The step-by-step JavaScript opens a step on load when its `<li>`
  has a `data-show` attribute, which the view never wrote, so steps the API marked as open stayed collapsed.

## Running

```bash
dotnet test --project tests/SchoolAccount.Web.Mvc.ComponentTests
```

They also run as part of `dotnet test` on the solution and in CI.
