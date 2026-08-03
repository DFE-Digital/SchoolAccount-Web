
---
date: "2026-08-03"
decision-makers: Mark Harrop & Paul Custance
---

# Use feature structure within the MVC application

## Context and Problem Statement

MVC applications expect the code to be organised within Views, Models, Controllers etc in different folders. Following the [Clean Architecture](../docs/clean-architecture.md) application folder structure each features items should be held within a Features folder structure to keep related items together. To get the MVC application to be able to work with this approach we need to be more explicit to the program where each item is within the file structure.

## Decision Drivers

* Controller and view routing by convention - minimal annotation and view configuration when creating a new feature
* Co-location of controller, views, and view models by page

## Considered Options

* Standard MVC routing - separate folders for controllers, views, and models
* Bespoke 'feature' structure - controllers, views and models arranged by feature using

## Decision Outcome

Chosen option: "Bespoke feature structure" is the only option that aligns with the Clean Architecture approach of arranging code by features, maintaining a consistent approach for both front end and API projects.

### Consequences

* Good, because allows for co-location of related items, making it easier to find and maintain code for a given feature
* Good because it is consistent with API project structure
* Bad, because it introduces complexity in the project routing

### Confirmation

All integrations tests continue to pass without any changes, demonstrating the project URLs remain the same