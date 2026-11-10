---
description: Adds a plain-English summary to the top of a GitHub Release's notes

# Started by the prod deploy's Publish release job once the Release exists. Can also be
# run by hand for any existing Release.
on:
  workflow_dispatch:
    inputs:
      tag:
        description: Release tag to summarise (e.g. v2026.11.09.1)
        required: true
        type: string

# The agent can only read. Its one write, editing the Release description, is applied
# by a separate safe-outputs job. copilot-requests uses the organisation's Copilot
# subscription, so no token or secret is needed.
permissions:
  contents: read
  pull-requests: read

# A named model, as "auto" needs a model-catalogue lookup that can fail. claude-sonnet-5
# is what "auto" resolved to in DFE-Digital/SchoolAccount-AgenticWorkflowTest.
engine:
  id: copilot
  model: claude-sonnet-5

# One run per release at a time; runs for different releases don't wait for each other
concurrency:
  job-discriminator: ${{ github.event.inputs.tag }}

tools:
  github:
    toolsets: [repos, pull_requests]

network: defaults

safe-outputs:
  update-release:
    max: 1

timeout-minutes: 10
---

# Release notes summary

Write a short, plain-English summary of release `${{ github.event.inputs.tag }}` of
`${{ github.repository }}` and add it to the top of that release's description.

## Find what changed

1. Get the release for tag `${{ github.event.inputs.tag }}`. Its description was generated
   by GitHub: a "What's Changed" list of merged pull requests and a "Full Changelog" link
   comparing the previous release with this one.
2. Read the title and description of each pull request in that list.

## Write the summary

The readers are the people who run the service, not only developers. Write so someone
who hasn't seen the code understands what is different for users.

- Start with a `## Summary` heading.
- List the changes users will notice first, one short bullet each, in plain English.
- Then a single bullet, starting "Behind the scenes:", that sums up internal work such as
  tests, refactoring and dependency updates. Leave it out if there was none.
- Use only what the pull requests say. If a change's effect on users isn't clear from its
  pull request, describe it as it is written rather than guessing.
- Keep it under 150 words. Don't repeat the pull request numbers or authors; the list
  below the summary already has them.

## Update the release

Use `update-release` for tag `${{ github.event.inputs.tag }}`:

- If the description doesn't already start with a `## Summary` section, use the
  `prepend` operation with your summary.
- If it does (this workflow has run before for this release), use the `replace`
  operation with your new summary followed by the rest of the existing description,
  unchanged, so there is still only one summary.

If the release has no pull requests listed, don't update it.
