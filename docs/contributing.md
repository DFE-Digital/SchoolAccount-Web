# Contributing

This guide covers how to branch, commit, and raise pull requests on this repository, and how branch naming links work
back to Jira.

## Branch naming

Branch from `main` using the convention:

- `<type>` - Either task, feature, bugfix or hotfix.
- `<Jira-key>` - This is the Jira issue id e.g. `SAB-123` and what links with the branch.
- `<short-description>` - Few words outlying the issue or the ticket title.

### Good

```csharp
task/SAB-123-fix-login-redirect
feature/SAB-456-add-school-search-feature
```

### Avoid

```csharp
task/fix-login-redirect         # no issue key — won't link to Jira
feature/SA-456                  # no description — hard to scan in a branch list
my-branch                       # no type prefix, no key
```

### Why the Jira key matters

An integration between both Github and Jira exist for this repository. The integration scans branch names, commit
messages, pull request titles and descriptions for anything matching the Jira issue key pattern (PROJECT-NUMBER) and
automatically links it to the matching ticket.

If the key is missing, malformed, or uses a project prefix that doesn't exist in Jira, nothing links — the ticket won't
show the branch, commit, or PR. Include the key in the branch name as the minimum needed to make linking work.

## Commits

- Group files that share a concern or belong to the same change into one commit; keep unrelated changes in separate
  commits.
- Commit often - small, focused commits are easier to review, revert, and understand than large ones.
- Write a clear message - a short summary line explaining what changed and why. Use the imperative mood ("Add", "Fix",
  "Update"), and keep the summary under 72 characters. A description can be added to further explain the changes within
  the commit message using two new lines in most IDE's.
    - Note: Adding the Jira issue key within commit message reinforces or can establish a connection between the branch
      and the Jira issue.

### Good

```csharp
SA-123 Add null reference check on login redirect
```

### Avoid

```csharp
fixed stuff
WIP
SA-123 changed the login controller and also tidied the CSS and bumped the package version
```

For further guidance on [how to write a good commit message](https://cbea.ms/git-commit/).

## Pull requests (PR)

1. Open a PR in the [github project repository](https://github.com/DFE-Digital/SchoolAccount-Web/pulls) against main
2. Keep the PR focused on a single ticket where possible
3. Include the Jira key in the PR title (e.g. SA-123 Add school search filter)
4. Add a description explaining what the PR does, along with the acceptance criteria from the associated ticket
5. Tests are written and pass for the changes, maintaining at least 70% code coverage
6. Build workflow passes 
7. PR is reviewed by a team member
8. Squash and commit merging is preferred following the commit message guidance
