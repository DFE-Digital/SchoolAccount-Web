---
date: "2026-09-09"
decision-makers: Marc Pacheco, Mark Harrop, Andy Cunningham 
consulted: Tejaswini Tellamekalachandra, Regional Services Team
---

# Use Academies API 

## Context and Problem Statement

DSI can surface the UKPRN for an organisation but has no further knowledge of other organisations that are associated with a Multi Academy Trust (MAT) or Local Authority (LA).
The Academies API can allow us to search for a MAT or LA via a UKPRN as well as any further establishments that sit under their authority.

## Considered Options

* Academies API
* GIAS API

## Decision Outcome

The Academies API has a NuGet package that can be more easily integrated within the application.
