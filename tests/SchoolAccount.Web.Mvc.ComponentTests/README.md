# SchoolAccount.Web.Mvc.ComponentTests

## Running the tests

Run these commands from the repository root (the folder containing `SchoolAccount.Web.slnx`). The tests run on the
Microsoft Testing Platform, as configured in `global.json`.

Run the whole component test suite:

```bash
dotnet test --project tests/SchoolAccount.Web.Mvc.ComponentTests
```

Run a single test class:

```bash
dotnet test --project tests/SchoolAccount.Web.Mvc.ComponentTests --filter-class "*.JourneyTests"
```

Run a single test (all cases of a `[Theory]` are included):

```bash
dotnet test --project tests/SchoolAccount.Web.Mvc.ComponentTests --filter-method "*.Overview_is_hidden_when_blank"
```

Running `dotnet test` with no arguments from the repository root runs these tests along with every other test project.
