---
status: "accepted"
date: "2026-07-29"
decision-makers: Paul Custance & Mark Harrop
---

# Supporting SASS within GDS Styles

## Context and Problem Statement

To implement government standards into the application to align closer with the design team. 

<!-- This is an optional element. Feel free to remove. -->
## Decision Drivers

* To closer align with the design team we should support SASS components
* Easier for a like for like comparison

<!-- This is an optional element. Feel free to remove. -->
### Consequences

* Good, because we can use the GDS SASS variables, colours and layout rather than redefining them within the css.
* Good, because it aligns closer with the component structure within the design prototype project.
* Bad, as it adds further complexity to the components and file structure.

<!-- This is an optional element. Feel free to remove. -->
### Confirmation

Changes to the SASS files are reflected within the generated css when building the application.

<!-- This is an optional element. Feel free to remove. -->
## More Information

* See [Sass Examples](https://github.com/x-govuk/govuk-frontend-aspnetcore/tree/main/samples/Samples.Sass) on how to integrate SASS within a GDS MVC app.
