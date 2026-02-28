# Project Structure — Taskin 2.0

## Repository Layout

```
taskin2.0/
├── back/src/                                    # Backend (.NET 10)
│   ├── ElGuerre.Taskin.Api/                     # Web API Layer
│   │   ├── Controllers/                         # REST controllers
│   │   ├── Middleware/                           # Exception handling, logging
│   │   ├── Program.cs                           # App startup
│   │   └── appsettings.json                     # Configuration
│   ├── ElGuerre.Taskin.Application/             # Application Layer (CQRS)
│   │   ├── Projects/                            # Feature: Projects
│   │   │   ├── Commands/                        # Create, Update, Delete
│   │   │   ├── Queries/                         # GetAll, GetById, GetStats
│   │   │   └── DTOs/                            # List, Details DTOs
│   │   ├── Tasks/                               # Feature: Tasks
│   │   ├── Pomodoros/                           # Feature: Pomodoros
│   │   ├── Data/                                # ITaskinDbContext
│   │   └── Observability/                       # TaskinMetrics, TelemetryConstants
│   ├── ElGuerre.Taskin.Domain/                  # Domain Layer
│   │   ├── Entities/                            # Project, Task, Pomodoro
│   │   └── SeedWork/                            # Entity, TrackedEntity, IUnitOfWork
│   ├── ElGuerre.Taskin.Infrastructure/          # Infrastructure Layer
│   │   └── EntityFramework/
│   │       ├── EntityConfigurations/            # IEntityTypeConfiguration<T>
│   │       ├── Migrations/                      # EF Core migrations
│   │       └── TaskinDbContext.cs               # DbContext implementation
│   ├── Taskin2.0.AppHost/                       # .NET Aspire Orchestrator
│   │   └── AppHost.cs                           # Container orchestration
│   └── Taskin2.0.ServiceDefaults/               # Shared Observability
│       └── Extensions.cs                        # OpenTelemetry, health checks
│
├── ui/src/                                      # Frontend (Angular 21)
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                            # Singleton services
│   │   │   │   ├── authentication/              # Auth service, guards
│   │   │   │   ├── services/                    # Navigation, notification, bootstrap
│   │   │   │   └── interceptors/                # HTTP interceptors
│   │   │   ├── features/                        # Feature modules
│   │   │   │   ├── dashboard/                   # Dashboard + analytics
│   │   │   │   ├── projects/                    # Project CRUD
│   │   │   │   │   ├── pages/                   # Route components
│   │   │   │   │   │   ├── projects/            # List view
│   │   │   │   │   │   ├── project-details/     # Detail view
│   │   │   │   │   │   └── project-new/         # Create/edit form
│   │   │   │   │   ├── stores/                  # NgRx Signal Store
│   │   │   │   │   └── services/                # HTTP service + DTOs
│   │   │   │   ├── tasks/                       # Task management
│   │   │   │   └── pomodoros/                   # Pomodoro tracking
│   │   │   ├── layout/                          # App shell
│   │   │   │   ├── layout/                      # Main layout container
│   │   │   │   ├── header/                      # Top header bar
│   │   │   │   ├── sidenav/                     # Side navigation
│   │   │   │   └── footer/                      # Footer
│   │   │   └── shared/                          # Reusable
│   │   │       ├── components/                  # Shared UI components
│   │   │       ├── pipes/                       # Custom pipes
│   │   │       └── services/                    # Shared services
│   │   ├── assets/
│   │   │   ├── i18n/                            # en.json, es.json
│   │   │   ├── images/                          # Logo, icons
│   │   │   └── icons/                           # SVG icon sets
│   │   ├── environments/                        # Environment configs
│   │   └── styles/                              # SCSS modules
│   ├── styles.scss                              # Material theme entry
│   └── styles.css                               # Tailwind CSS v4 entry
│
├── deploy/                                      # Observability Configs
│   ├── grafana/                                 # Dashboards + provisioning
│   ├── prometheus/                              # Scrape config + alerts
│   ├── otel-collector/                          # OTLP pipeline config
│   ├── tempo/                                   # Trace storage config
│   ├── loki/                                    # Log aggregation config
│   └── alloy/                                   # Faro frontend telemetry
│
├── docs/                                        # Documentation
│   ├── architecture/                            # Architecture docs
│   └── patterns/                                # Code patterns
│
└── .claude/                                     # Claude Code Config
    ├── agents/                                  # 5 specialized agents
    ├── skills/                                  # 6 custom skills
    ├── settings.json                            # Shared permissions + hooks
    └── settings.local.json                      # Local overrides
```

## Architecture Diagrams

### Backend — Clean Architecture

```
┌─────────────────────────────────────────────┐
│                  API Layer                   │
│  Controllers → MediatR → Command/Query      │
├─────────────────────────────────────────────┤
│             Application Layer                │
│  Commands + Handlers + Validators            │
│  Queries + Handlers                          │
│  DTOs, Interfaces                            │
├─────────────────────────────────────────────┤
│               Domain Layer                   │
│  Entities, Enums, SeedWork                   │
│  (Zero external dependencies)                │
├─────────────────────────────────────────────┤
│           Infrastructure Layer               │
│  EF Core DbContext, Configurations           │
│  Repository implementations                  │
└─────────────────────────────────────────────┘
```

### Frontend — Feature Architecture

```
┌──────────────────────────────────────────┐
│            App Shell (Layout)             │
│  Header | Sidenav | Footer | Router      │
├──────────────────────────────────────────┤
│          Feature Modules                  │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ │
│  │Dashboard │ │Projects  │ │Tasks     │ │
│  │          │ │ Pages    │ │ Pages    │ │
│  │          │ │ Store    │ │ Store    │ │
│  │          │ │ Service  │ │ Service  │ │
│  └──────────┘ └──────────┘ └──────────┘ │
├──────────────────────────────────────────┤
│           Core Services                   │
│  Auth | Navigation | Notification        │
│  HTTP Interceptors | Bootstrap            │
├──────────────────────────────────────────┤
│          Shared Components                │
│  Pipes | Directives | Common UI           │
└──────────────────────────────────────────┘
```

### Infrastructure — Observability Stack

```
┌─────────────┐     ┌──────────────────┐
│  Angular UI  │────→│  .NET API        │
│  (Faro/Alloy)│     │  (OTLP exporter) │
└─────────────┘     └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │  OTel Collector   │
                    │  (4317/4318)      │
                    └──┬───────┬───┬───┘
                       │       │   │
              ┌────────▼──┐ ┌─▼─┐ ┌▼────────┐
              │ Prometheus │ │Tem│ │  Loki    │
              │ (metrics)  │ │po │ │  (logs)  │
              │ :9090      │ │:32│ │  :3100   │
              └────────┬───┘ │00 │ └──┬───────┘
                       │     └─┬─┘    │
                    ┌──▼───────▼──────▼──┐
                    │     Grafana         │
                    │     :3000           │
                    └─────────────────────┘
```

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Frontend Framework | Angular | 21.1.3 |
| UI Components | Angular Material | 21.1.3 |
| CSS Framework | Tailwind CSS | 4.1.18 |
| State Management | NgRx Signals | 21.0.1 |
| i18n | Transloco | 8.x |
| TypeScript | TypeScript | 5.9.3 |
| Backend Framework | ASP.NET Core | .NET 10 |
| ORM | Entity Framework Core | 10.0.2 |
| CQRS | MediatR | 14.0.0 |
| Validation | FluentValidation | Latest |
| Logging | Serilog | 10.0.0 |
| Orchestration | .NET Aspire | Latest |
| Database | SQL Server | Latest |
| Cache | Redis | Latest |
| Metrics | Prometheus | v2.45.0 |
| Tracing | Grafana Tempo | 2.3.1 |
| Logs | Grafana Loki | 3.0.0 |
| Dashboards | Grafana | 10.3.3 |
| Telemetry | OpenTelemetry Collector | 0.139.0 |

---

## Detailed Core Layer (Frontend)

The `core/` directory contains singleton services, guards, and interceptors that are instantiated once at application startup. These are provided at the root level and should never be imported by `shared/`.

```
core/
├── authentication/
│   ├── auth.service.ts           # Auth state management
│   └── auth.guard.ts             # Route protection (CanActivateFn)
├── services/
│   ├── navigation.service.ts     # Sidenav & routing state
│   ├── notification.service.ts   # Snackbar notifications
│   └── bootstrap.service.ts      # App initialization
└── interceptors/
    └── api.interceptor.ts        # Base URL, error handling
```

**auth.service.ts** manages the current user session, login/logout flows, and exposes reactive signals for authentication state that other parts of the application can consume.

**auth.guard.ts** is a functional guard (`CanActivateFn`) that protects routes requiring authentication. It redirects unauthenticated users to the login page.

**navigation.service.ts** manages sidenav open/close state and tracks the current active route for highlighting the correct menu item.

**notification.service.ts** wraps Angular Material's `MatSnackBar` to provide a consistent API for success, error, and info notifications throughout the application.

**bootstrap.service.ts** runs during `APP_INITIALIZER` to load configuration, set up the initial locale for Transloco, and perform any one-time setup the application requires before rendering.

**api.interceptor.ts** is a functional HTTP interceptor that prepends the base API URL from the environment configuration and handles global error responses (401, 403, 500) with appropriate user notifications.

## Detailed Features Layer (Frontend)

Each feature follows a consistent internal structure. Features are isolated domains that encapsulate pages, state management, HTTP services, and DTOs for a single entity.

### Page Naming Convention

Pages follow a strict naming convention based on the entity they represent:

| Pattern | Purpose | Example |
|---------|---------|---------|
| `features/{entity}/pages/{entities}/` | List page (plural) | `features/projects/pages/projects/` |
| `features/{entity}/pages/{entity}-new/` | Create page | `features/projects/pages/project-new/` |
| `features/{entity}/pages/{entity}-details/` | Detail/edit page | `features/projects/pages/project-details/` |
| `features/{entity}/stores/` | NgRx Signal Store | `features/projects/stores/` |
| `features/{entity}/services/` | HTTP service + DTOs | `features/projects/services/` |

### Example: Projects Feature

```
features/projects/
├── pages/
│   ├── projects/                    # List page — displays all projects
│   │   ├── projects.ts              # Component class
│   │   └── projects.html            # Template
│   ├── project-new/                 # Create page — form for new project
│   │   ├── project-new.ts
│   │   └── project-new.html
│   └── project-details/             # Detail page — view/edit single project
│       ├── project-details.ts
│       └── project-details.html
├── stores/
│   └── project.store.ts             # NgRx Signal Store (providedIn: 'root')
└── services/
    └── project.service.ts           # HTTP service with typed DTOs
```

### Store Pattern

Each feature store extends `signalStore` and is decorated with `@Injectable()` with `providedIn: 'root'`. Stores use `withState`, `withComputed`, and `withMethods` to organize state, derived values, and side effects. Queries use `switchMap` for cancellation; mutations use `exhaustMap` to prevent duplicate submissions.

### Service Pattern

Each feature service returns typed DTOs wrapped in `CollectionResponse<T>` for lists and `ActionResponse` for mutations. Services inject `HttpClient` and use the `@env` alias for base URL configuration.

## Shared Layer Structure

The `shared/` directory contains reusable UI components, pipes, and utility services that can be imported by any feature or layout component. Shared must have zero imports from `core/` or `features/`.

```
shared/
├── components/                      # Reusable UI components
│   ├── confirm-dialog/              # Generic confirmation dialog
│   ├── empty-state/                 # Empty state placeholder
│   ├── loading-spinner/             # Loading indicator
│   └── stat-card/                   # Dashboard statistic card
├── pipes/                           # Custom transform pipes
│   └── time-ago.pipe.ts             # Relative time display
└── services/                        # Shared utility services
    └── storage.service.ts           # LocalStorage wrapper
```

Shared components are standalone and self-contained. They communicate exclusively through signal `input()` and `output()` bindings. They must not inject feature-specific services or stores.

## Layout Layer Structure

The `layout/` directory defines the application shell: the persistent chrome that wraps all routed content.

```
layout/
├── layout/                          # Main layout with <router-outlet>
│   ├── layout.ts                    # Orchestrates header + sidenav + footer
│   └── layout.html                  # Shell template with mat-sidenav-container
├── header/                          # Top bar with title, actions
│   ├── header.ts                    # Injects NavigationService, AuthService
│   └── header.html                  # Logo, page title, user menu
├── sidenav/                         # Navigation menu
│   ├── sidenav.ts                   # Menu items, active route tracking
│   └── sidenav.html                 # Navigation links with Material icons
└── footer/                          # Copyright, version
    ├── footer.ts                    # App version from environment
    └── footer.html                  # Copyright text, compact layout
```

The layout component uses Angular Material's `mat-sidenav-container` to provide a responsive sidebar that collapses on smaller viewports. The header communicates with the sidenav through `NavigationService` to toggle open/close state.

## Dependency Flow Rules

Strict dependency boundaries enforce clean architecture on the frontend. Violating these rules leads to circular dependencies and tightly coupled features.

```
┌────────────┐
│  Features   │ ──→ Core, Shared (ONLY)
└────────────┘
      ×
      │ Features CANNOT depend on each other
      ×
┌────────────┐
│   Layout    │ ──→ Core, Shared (ONLY)
└────────────┘
┌────────────┐
│    Core     │ ──→ Shared (ONLY)
└────────────┘
┌────────────┐
│   Shared    │ ──→ No project imports (Angular/Material/third-party only)
└────────────┘
```

**Rules in detail:**

- **Features CANNOT depend on each other.** If `projects` needs data from `tasks`, it must go through a shared service or the backend API. Cross-feature imports create hidden coupling that makes features impossible to lazy-load independently.
- **Features depend on Core and Shared only.** Features use core services (auth, navigation, notifications) and shared UI components (dialogs, pipes, stat cards).
- **Core depends on Shared only.** Core services may use shared utilities but must never import from features or layout.
- **Layout depends on Core and Shared only.** The layout shell uses core services for navigation state and auth state, and shared components for common UI elements.
- **Shared has zero project imports.** Shared components only import from `@angular/*`, `@angular/material/*`, and third-party libraries. This ensures they remain truly reusable.

## File Naming Conventions

Angular 21 introduced the convention of dropping the `.component` suffix from component files. Services, stores, guards, pipes, and models retain their suffixes for clarity.

| Type | Old Convention | New Convention | Class Name |
|------|---------------|---------------|------------|
| Component | `task-card.component.ts` | `task-card.ts` | `TaskCard` |
| Service | `project.service.ts` | `project.service.ts` | `ProjectService` |
| Store | `project.store.ts` | `project.store.ts` | `ProjectStore` |
| Guard | `auth.guard.ts` | `auth.guard.ts` | `authGuard` |
| Pipe | `time-ago.pipe.ts` | `time-ago.pipe.ts` | `TimeAgoPipe` |
| Model | `project.model.ts` | `project.model.ts` | `ProjectListDto` |

**Key points:**

- Only **components** drop the `.component` suffix in the filename. The class name also drops the `Component` suffix (e.g., `TaskCard` instead of `TaskCardComponent`).
- **Services, stores, guards, pipes, and models** keep their suffix in both the filename and class name for immediate recognition.
- Guards use `camelCase` function names (`authGuard`) since they are functional `CanActivateFn` rather than classes.
- Templates are always in separate `.html` files, never inline.

## Import Organization

All TypeScript files follow a consistent 5-group import ordering, separated by blank lines:

```typescript
// 1. Angular core
import { Component, inject, signal, computed } from '@angular/core';
import { RouterLink } from '@angular/router';

// 2. Angular Material
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

// 3. Third-party libraries
import { TranslocoDirective } from '@jsverse/transloco';
import { patchState, signalStore, withState } from '@ngrx/signals';
import { switchMap } from 'rxjs';

// 4. Project aliases
import { NotificationService } from '@core/services/notification.service';
import { StatCardComponent } from '@shared/components/stat-card/stat-card';
import { environment } from '@env/environment';

// 5. Relative imports (same feature)
import { ProjectStore } from '../stores/project.store';
import { ProjectService } from '../services/project.service';
```

**Rules:**

- Each group is separated by exactly one blank line.
- Within each group, imports are sorted alphabetically by module path.
- Barrel imports (`index.ts`) are discouraged; prefer explicit file paths for tree-shaking.
- Never mix groups (e.g., do not place a Material import between Angular core imports).

## Component Member Ordering

Within a component class, members follow a strict ordering to ensure consistency and readability:

```typescript
@Component({ ... })
export class ProjectDetails {
  // 1. Injected services
  private readonly store = inject(ProjectStore);
  private readonly route = inject(ActivatedRoute);
  private readonly notification = inject(NotificationService);

  // 2. Signal inputs
  projectId = input.required<string>();
  showActions = input(true);

  // 3. Signal outputs
  deleted = output<string>();

  // 4. Signals (local mutable state)
  isEditing = signal(false);
  formData = signal<ProjectForm | null>(null);

  // 5. Computed signals (derived state)
  project = computed(() => this.store.selectedProject());
  canEdit = computed(() => this.isEditing() && this.project() !== null);

  // 6. Lifecycle hooks
  ngOnInit(): void {
    this.store.loadProject(this.projectId());
  }

  // 7. Public methods (template-bound)
  onSave(): void {
    // ...
  }

  onCancel(): void {
    // ...
  }

  // 8. Private methods (internal logic)
  private validateForm(): boolean {
    // ...
  }
}
```

This ordering makes it easy to scan a component: dependencies at the top, inputs/outputs next, internal state, then behavior.

## Path Aliases

Path aliases are configured in `tsconfig.json` to avoid deep relative imports (e.g., `../../../core/services/`). All aliases map to `src/app/` subdirectories or `src/` directories.

| Alias | Maps To | Usage |
|-------|---------|-------|
| `@core` | `src/app/core` | Auth, services, interceptors |
| `@shared` | `src/app/shared` | Reusable components, pipes |
| `@theme` | `src/styles` | SCSS theme variables, mixins |
| `@env` | `src/environments` | Environment configuration |

**Examples:**

```typescript
import { AuthService } from '@core/authentication/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { StatCardComponent } from '@shared/components/stat-card/stat-card';
import { TimeAgoPipe } from '@shared/pipes/time-ago.pipe';
import { environment } from '@env/environment';
```

Path aliases must never be used for imports within the same feature. Use relative imports (`./` or `../`) for intra-feature references to keep features self-contained and portable.

## Backend Project Dependency Graph

The backend follows Clean Architecture with strict dependency inversion. Each project has clearly defined responsibilities and allowed references.

```
ElGuerre.Taskin.Api
├── References: Application, Infrastructure
├── Contains: Controllers, Middleware, Program.cs
└── Entry point for HTTP requests

ElGuerre.Taskin.Application
├── References: Domain
├── Contains: Commands, Queries, Handlers, Validators, DTOs
└── Orchestrates business logic

ElGuerre.Taskin.Domain
├── References: (none — zero dependencies)
├── Contains: Entities, Enums, SeedWork (Entity, TrackedEntity, IUnitOfWork)
└── Pure domain model

ElGuerre.Taskin.Infrastructure
├── References: Application, Domain
├── Contains: DbContext, EntityConfigurations, Migrations, Middleware
└── Data access and persistence

Taskin2.0.AppHost
├── References: Api (for orchestration)
├── Contains: Container definitions, service wiring
└── .NET Aspire orchestrator

Taskin2.0.ServiceDefaults
├── References: (OpenTelemetry packages)
├── Contains: Extensions.cs (AddServiceDefaults, MapDefaultEndpoints)
└── Shared observability configuration
```

### Dependency Rules (Backend)

```
          ┌───────────────┐
          │   Api Layer    │
          │  (entry point) │
          └───┬───────┬───┘
              │       │
    ┌─────────▼─┐ ┌───▼──────────────┐
    │Application│ │ Infrastructure    │
    │  (CQRS)   │ │ (EF Core, data)  │
    └─────┬─────┘ └───┬──────────────┘
          │           │
          └─────┬─────┘
                │
          ┌─────▼─────┐
          │  Domain    │
          │ (entities) │
          │ (no deps)  │
          └───────────┘
```

- **Domain** has zero NuGet package dependencies. It defines entities, enums, and interfaces only.
- **Application** references Domain and defines interfaces (`ITaskinDbContext`, `IUnitOfWork`) that Infrastructure implements.
- **Infrastructure** references both Application (for interfaces) and Domain (for entity types). It implements persistence concerns.
- **Api** references Application (for MediatR handlers) and Infrastructure (for DI registration in `Program.cs`). Controllers only call `IMediator.Send()`.
- **AppHost** references Api to add it as a project resource in the Aspire orchestrator. It does not reference Application, Domain, or Infrastructure directly.
- **ServiceDefaults** is a standalone project that provides shared OpenTelemetry and health check configuration consumed by Api through extension methods.
