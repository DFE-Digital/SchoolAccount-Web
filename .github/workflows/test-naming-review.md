---
description: |
  Reviews test method names added or renamed in a pull request against the
  Naming section of docs/testing-standards.md and leaves review comments
  suggesting better names where they fall short.

on:
  pull_request:
    types: [opened, synchronize, reopened]
    paths:
      - "tests/**/*.cs"
  workflow_dispatch:

permissions:
  contents: read
  pull-requests: read
  copilot-requests: write

network:
  allowed:
    - defaults
    - github

tools:
  bash: ["cat", "grep", "git"]
  github:
    toolsets: [pull_requests, repos]

safe-outputs:
  create-pull-request-review-comment:
    max: 20
  submit-pull-request-review:
    max: 1
---

# Test naming review

You are reviewing pull request #${{ github.event.pull_request.number }} in ${{ github.repository }}.

## Standard

Read `docs/testing-standards.md`, in particular the **Naming** section. In short, a test name should read as a plain statement of what the system does under a given condition, and answer: *what does the system do, and when?*

Names that fall short:

- Describe test mechanics, e.g. `Test_GetOpenStatus_1`
- Are too generic to explain a failure, e.g. `Returns_correct_result`
- Describe the method under test rather than the behaviour, e.g. `GetStatus_works`

## Task

1. Get the list of files changed in the pull request and their diffs.
2. In changed files under `tests/`, find every method with a `[Fact]` or `[Theory]` attribute that was **added or renamed** in this pull request. Ignore tests that are unchanged or only had their body edited.
3. Judge each name against the standard, using the test body to understand what it actually checks.
4. For each name that falls short, leave a review comment on the line with the method name. Work through each test on its own: read only that method's body, from its signature to its closing brace, before writing its comment. Write every comment in this format:

   > **`<method name>`** asserts `<the expected value or condition from this method's Assert section>`.
   > <why the name falls short, in one sentence>
   > Suggest: `<better name>`

   The suggested name must describe the value or condition you quoted on the first line. Before submitting, re-read each comment and check the quoted method name, the quoted assertion and the line it is attached to all belong to the same method.
5. Submit one review:
   - If you left any comments, submit with event `COMMENT` and a one-line summary such as "2 of 5 new test names could be clearer."
   - If every name meets the standard, submit with event `COMMENT` and a one-line summary saying so.
   - If no test methods were added or renamed, do not submit a review.

Keep comments short and specific. Do not comment on anything other than test names.
