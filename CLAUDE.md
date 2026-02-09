# CLAUDE.md — Taskin 2.0

## MANDATORY: Agent Delegation

**Before starting ANY task, delegate to the appropriate agent:**

| Task Type | Agent | Trigger |
|-----------|-------|---------|
| Components, templates, forms, Material, Tailwind, a11y | `angular-ui-developer` | Any UI work in `ui/src/` |
| Stores, services, guards, interceptors, RxJS | `angular-state-architect` | State/business logic |
| Entities, CQRS, controllers, EF Core, validation | `dotnet-architect` | Any backend code |
| Docker, Aspire, OpenTelemetry, Prometheus, Grafana | `dotnet-infrastructure` | Infrastructure/observability |
| Code review, pattern compliance, security audit | `code-reviewer` | PR reviews, audits |

**Custom skills available:** `/generate-component`, `/create-cqrs-handler`, `/create-ef-migration`, `/create-feature`, `/update-translations`, `/review-pr`

## Context7 Integration

Always use Context7 MCP for up-to-date docs. Key queries:
- `@angular/core` signals, DI, control flow
- `@angular/material` MD3 components
- `@ngrx/signals` signal store patterns
- `Microsoft.EntityFrameworkCore` queries and migrations
- `MediatR` CQRS patterns

## Project Overview

Full-stack Pomodoro task management app.

| Layer | Stack |
|-------|-------|
| Frontend | Angular 21.1.3, Material 21, Tailwind CSS 4, NgRx Signals 21, Transloco 8 |
| Backend | .NET 9, ASP.NET Core, EF Core, MediatR, FluentValidation, Serilog |
| Infrastructure | .NET Aspire, SQL Server, Redis, Seq, OpenTelemetry, Prometheus, Grafana |

**Zoneless** — no zone.js. `@angular/build` builder.

## Repository Structure

```
taskin2.0/
├── back/src/                              # Backend (.NET 9 Clean Architecture)
│   ├── ElGuerre.Taskin.Api/               # Controllers, middleware
│   ├── ElGuerre.Taskin.Application/       # CQRS commands/queries
│   ├── ElGuerre.Taskin.Domain/            # Entities, enums
│   ├── ElGuerre.Taskin.Infrastructure/    # EF Core, data access
│   ├── Taskin2.0.AppHost/                 # .NET Aspire orchestrator
│   └── Taskin2.0.ServiceDefaults/         # Shared observability
├── ui/src/                                # Frontend (Angular 21)
│   └── src/app/
│       ├── core/                          # Auth, services, interceptors
│       ├── features/                      # dashboard, projects, tasks, pomodoros
│       ├── layout/                        # header, sidenav, footer
│       └── shared/                        # Reusable components, pipes
├── deploy/                                # Observability configs (Grafana, Prometheus, etc.)
└── docs/                                  # Architecture and pattern docs
```

## Core Domain

`Project` → has many `Task` → has many `Pomodoro`

## Development Commands

### Frontend (`ui/src/`)
```bash
npm start                    # Dev server at http://localhost:4200
npm run build                # Production build
npx ng g c features/X/pages/Y --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
```

### Backend (`back/src/`)
```bash
dotnet build                 # Build solution
dotnet run --project ElGuerre.Taskin.Api  # API at https://localhost:6001
dotnet test                  # Run tests
dotnet ef migrations add Name --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

## API Endpoints

Base: `https://localhost:6001/api`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Projects` | List projects (page, size, search, status, sort, order) |
| GET | `/Projects/{id}` | Get project details |
| POST | `/Projects` | Create project |
| PUT | `/Projects/{id}` | Update project |
| DELETE | `/Projects/{id}` | Delete project |
| GET | `/Projects/stats` | Project statistics |
| GET | `/Tasks?projectId={id}` | Tasks by project |
| GET/POST/PUT/DELETE | `/Tasks/{id}` | Task CRUD |
| GET/POST/PUT/DELETE | `/Pomodoros/{id}` | Pomodoro CRUD |

## Key Conventions

- **Semicolons**: Required in TypeScript
- **Templates**: Always in separate `.html` files, never inline
- **Change detection**: Always `OnPush`
- **Components**: Standalone, zoneless, signal-based `input()`/`output()`
- **State**: NgRx Signal Store (`signalStore`, `withState`, `withComputed`, `withMethods`, `rxMethod`)
- **i18n**: Transloco with `en.json` and `es.json` only
- **Path aliases**: `@core`, `@shared`, `@theme`, `@env`
- **Backend**: Clean Architecture, CQRS with MediatR, primary constructors, `required` properties
- **No `any`**: Use `unknown` when type is uncertain
- **Tailwind CSS v4**: `@import 'tailwindcss'`, `shadow-xs`, `@media (width >= 768px)`
- **Material Design 3**: `matButton="filled"`, `matButton="elevated"`, etc.

## Documentation

- `ui/src/CLAUDE.md` — Frontend patterns and conventions
- `back/src/CLAUDE.md` — Backend patterns and conventions
- `docs/architecture/project-structure.md` — Full architecture
- `docs/patterns/code-style.md` — Code style conventions
- `docs/patterns/state-management.md` — NgRx Signal Store patterns
