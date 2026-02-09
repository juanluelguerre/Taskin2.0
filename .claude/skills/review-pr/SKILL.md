---
name: review-pr
description: Review a pull request for pattern compliance, security, and best practices
user_invocable: true
---

# Review PR Skill

Perform a comprehensive code review of a pull request.

## Arguments

The user provides: PR number or branch name (optional — defaults to current branch vs main).

## Steps

1. **Get PR diff**:
   ```bash
   # If PR number provided:
   gh pr diff {number}

   # If no PR number, diff current branch vs main:
   git diff main...HEAD
   ```

2. **Get list of changed files**:
   ```bash
   gh pr diff {number} --name-only
   # or
   git diff main...HEAD --name-only
   ```

3. **Read each changed file** to understand full context (not just the diff)

4. **Review against project conventions** from `CLAUDE.md`, `ui/src/CLAUDE.md`, `back/src/CLAUDE.md`

5. **Generate structured review**:

## Review Template

```markdown
# PR Review: {title}

## Summary
{1-2 sentence overview of changes}

## Files Changed
- {list of files with change type: added/modified/deleted}

## Frontend Review
### Pattern Compliance
- [ ] OnPush change detection
- [ ] Signal inputs/outputs (no decorators)
- [ ] Templates in separate files
- [ ] track in @for blocks
- [ ] Transloco keys in both en/es
- [ ] MD3 button syntax
- [ ] Tailwind v4 classes
- [ ] Path aliases used
- [ ] No any type
- [ ] Semicolons

### State Management
- [ ] switchMap for queries
- [ ] exhaustMap for mutations
- [ ] Error handling with notifications

## Backend Review
### Pattern Compliance
- [ ] Commands have validators
- [ ] Primary constructor DI
- [ ] required on mandatory props
- [ ] Nullable refs for optional
- [ ] POST returns 201

### Security
- [ ] No hardcoded secrets
- [ ] Input validation
- [ ] Parameterized queries

## Issues Found

### Critical
{issues that must be fixed}

### Warnings
{issues that should be addressed}

### Suggestions
{nice-to-have improvements}
```

## Rules

- This is a **read-only** review — never modify files
- Always read full files, not just diffs, for proper context
- Reference specific file paths and line numbers
- Prioritize issues by severity
- Be constructive — suggest specific fixes
