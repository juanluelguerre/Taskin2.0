# Documentation -- Taskin 2.0

## Overview

Taskin 2.0 is a full-stack Pomodoro task management application built with Angular 21
and .NET 10. It allows users to organize work into Projects, break them down into Tasks,
and track focused work sessions using the Pomodoro Technique.

The application follows Clean Architecture on the backend (CQRS with MediatR) and a
signal-based, zoneless architecture on the frontend (NgRx Signal Store, standalone
components, Material Design 3). Infrastructure is orchestrated with .NET Aspire and
includes observability via OpenTelemetry, Prometheus, and Grafana.

---

## Architecture Documentation

| Document | Description |
|----------|-------------|
| [Project Structure](architecture/project-structure.md) | Full repository layout, layer responsibilities, and architecture diagrams |

---

## Pattern Documentation

| Document | Description |
|----------|-------------|
| [Code Style](patterns/code-style.md) | TypeScript, C#, and HTML conventions (semicolons, naming, formatting) |
| [State Management](patterns/state-management.md) | NgRx Signal Store patterns, RxJS interop, flattening operators |
| [Internationalization](patterns/internationalization.md) | Transloco 8 setup, translation keys, workflow for en/es |
| [Error Handling](patterns/error-handling.md) | Backend middleware, frontend notification service, validation patterns |

---

## Agent and Configuration Docs

### Project-Level Configuration

| Document | Description |
|----------|-------------|
| [CLAUDE.md](../CLAUDE.md) | Root project instructions, agent delegation table, development commands |
| [Frontend CLAUDE.md](../ui/src/CLAUDE.md) | Angular 21 patterns, Material Design 3 syntax, Tailwind CSS v4, NgRx Signal Store |
| [Backend CLAUDE.md](../back/src/CLAUDE.md) | .NET 10 Clean Architecture, CQRS with MediatR, EF Core conventions |

### Agent Definitions

| Agent | File | Scope |
|-------|------|-------|
| Angular UI Developer | [angular-ui-developer.md](../.claude/agents/angular-ui-developer.md) | Components, templates, forms, Material, Tailwind, accessibility |
| Angular State Architect | [angular-state-architect.md](../.claude/agents/angular-state-architect.md) | Stores, services, guards, interceptors, RxJS |
| .NET Architect | [dotnet-architect.md](../.claude/agents/dotnet-architect.md) | Entities, CQRS handlers, controllers, EF Core, validation |
| .NET Infrastructure | [dotnet-infrastructure.md](../.claude/agents/dotnet-infrastructure.md) | Docker, Aspire, OpenTelemetry, Prometheus, Grafana |
| Code Reviewer | [code-reviewer.md](../.claude/agents/code-reviewer.md) | PR reviews, pattern compliance, security audits |

---

## Refactoring Guide

| Document | Description |
|----------|-------------|
| [Refactoring Guide](refactoring-guide.md) | Step-by-step refactoring strategies, migration checklists, breaking change handling |

---

## Additional Guides

| Document | Description |
|----------|-------------|
| [Angular Claude Guide](ANGULAR_CLAUDE_GUIDE.md) | Angular 21 patterns and conventions reference |
| [Angular Component Templates](ANGULAR_COMPONENT_TEMPLATES.md) | Template scaffolding and generation examples |
| [Angular Patterns Reference](ANGULAR_PATTERNS_REFERENCE.md) | Signal inputs, control flow, RxJS interop patterns |
| [Aspire Telemetry Setup](ASPIRE_TELEMETRY_SETUP.md) | .NET Aspire orchestrator and telemetry configuration |
| [Prometheus and Grafana Setup](PROMETHEUS_GRAFANA_SETUP.md) | Metrics collection and dashboard configuration |
| [Unit Tests Status](UNIT_TESTS_STATUS.md) | Current test coverage and testing strategy |

---

## External Resources

### Frontend

| Resource | URL |
|----------|-----|
| Angular | [angular.dev](https://angular.dev) |
| Angular Material | [material.angular.io](https://material.angular.io) |
| NgRx Signals | [ngrx.io/guide/signals](https://ngrx.io/guide/signals) |
| Tailwind CSS v4 | [tailwindcss.com/docs](https://tailwindcss.com/docs) |
| Transloco | [jsverse.github.io/transloco](https://jsverse.github.io/transloco) |
| TypeScript | [typescriptlang.org/docs](https://www.typescriptlang.org/docs) |

### Backend

| Resource | URL |
|----------|-----|
| .NET 10 | [learn.microsoft.com/dotnet](https://learn.microsoft.com/dotnet) |
| EF Core | [learn.microsoft.com/ef/core](https://learn.microsoft.com/ef/core) |
| MediatR | [github.com/jbogard/MediatR](https://github.com/jbogard/MediatR) |
| FluentValidation | [docs.fluentvalidation.net](https://docs.fluentvalidation.net) |
| Serilog | [serilog.net](https://serilog.net) |

### Infrastructure

| Resource | URL |
|----------|-----|
| .NET Aspire | [learn.microsoft.com/dotnet/aspire](https://learn.microsoft.com/dotnet/aspire) |
| OpenTelemetry .NET | [opentelemetry.io/docs/languages/dotnet](https://opentelemetry.io/docs/languages/dotnet) |
| Prometheus | [prometheus.io/docs](https://prometheus.io/docs) |
| Grafana | [grafana.com/docs](https://grafana.com/docs) |

---

## Documentation Conventions

### Formatting Rules

- Use ATX-style headers (`#`, `##`, `###`). Do not skip heading levels.
- Use fenced code blocks with language identifiers (```typescript, ```csharp, ```html).
- Use tables for structured data. Align columns with pipes.
- Use relative links between documentation files.
- Keep line length under 100 characters where practical.
- Use `---` horizontal rules to separate major sections.

### File Naming

- Use lowercase with hyphens: `code-style.md`, `state-management.md`.
- Place architecture docs in `docs/architecture/`.
- Place pattern docs in `docs/patterns/`.
- Place standalone guides in `docs/`.

### When to Update Documentation

- **New feature**: Add or update relevant pattern docs and translation keys.
- **Migration**: Document breaking changes, version bumps, and migration steps.
- **New pattern**: Add a pattern doc in `docs/patterns/` and link it from this index.
- **Configuration change**: Update the relevant CLAUDE.md or agent file.
- **API change**: Update the API endpoints table in the root CLAUDE.md.

---

## Documentation Checklist

Before merging documentation changes, verify:

- [ ] All internal links resolve correctly (no broken relative paths)
- [ ] Code examples compile and follow project conventions
- [ ] Tables are properly formatted and aligned
- [ ] New docs are linked from this index (`docs/README.md`)
- [ ] Language is clear, concise, and free of jargon where possible
- [ ] Both English and Spanish translation keys are documented if i18n-related
- [ ] File names use lowercase-with-hyphens convention
- [ ] No sensitive information (API keys, credentials, internal URLs) is included
