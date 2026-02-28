# CODEX.md — Taskin 2.0

> This file provides guidance to Codex CLI working with this repository.

## Documentation Structure

| Document | Description |
|----------|-------------|
| `CLAUDE.md` (this file) | Root guide — architecture, conventions, agent delegation |
| `ui/src/CLAUDE.md` | Frontend patterns — Angular 21, MD3, TW4, NgRx Signals |
| `back/src/CLAUDE.md` | Backend patterns — Clean Architecture, CQRS, EF Core |
| `docs/architecture/project-structure.md` | Full repository structure with diagrams |
| `docs/patterns/code-style.md` | TypeScript + C# code style conventions |
| `docs/patterns/state-management.md` | NgRx Signal Store + RxJS interop |
| `docs/patterns/internationalization.md` | Transloco i18n patterns (en, es) |
| `docs/patterns/error-handling.md` | Full-stack error handling patterns |
| `docs/refactoring-guide.md` | Migration patterns and refactoring checklists |

---

## Purpose & Context

Taskin 2.0 is a **full-stack Pomodoro task management application**. Users organize work into Projects, break Projects into Tasks, and track focused work sessions as Pomodoros.

**Core domain:** `Project` → has many `Task` → has many `Pomodoro`

---

## Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend Framework | Angular | 21.1.3 |
| UI Components | Angular Material (MD3) | 21.1.3 |
| CSS Framework | Tailwind CSS | 4.1.18 |
| State Management | NgRx Signals | 21.0.1 |
| i18n | Transloco | 8.x |
| TypeScript | TypeScript | 5.9.3 |
| Backend Framework | ASP.NET Core | .NET 10 |
| ORM | Entity Framework Core | 10.0.2 |
| CQRS | MediatR | 14.0.0 |
| Validation | FluentValidation | Latest |
| Logging | Serilog | 10.0.0 |
| API Docs | Scalar (OpenAPI) | 2.12.36 |
| Orchestration | .NET Aspire | Latest |
| Database | SQL Server | Latest |
| Cache | Redis | Latest |
| Telemetry | OpenTelemetry | 1.15.0 |
| Metrics | Prometheus | v2.45.0 |
| Tracing | Grafana Tempo | 2.3.1 |
| Logs | Grafana Loki | 3.0.0 |
| Dashboards | Grafana | 10.3.3 |

**Zoneless** — no zone.js. Uses `@angular/build` builder.

---

## Languages

| Code | Language | Default |
|------|----------|---------|
| `en` | English | Yes |
| `es` | Spanish | No |

Translation files: `ui/src/src/assets/i18n/en.json`, `ui/src/src/assets/i18n/es.json`

---

## Development Commands

### Frontend (`ui/src/`)

```bash
npm start                    # Dev server at http://localhost:4200
npm run build                # Production build
npx ng g c features/X/pages/Y --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
```

Do NOT include `--standalone` flag (default in Angular 21).

### Backend (`back/src/`)

```bash
dotnet build                 # Build solution
dotnet run --project ElGuerre.Taskin.Api  # API at https://localhost:6001
dotnet test                  # Run tests
```

### EF Core Migrations

```bash
# From back/src/ directory
dotnet ef migrations add MigrationName \
  --startup-project ElGuerre.Taskin.Api \
  --project ElGuerre.Taskin.Infrastructure \
  -o EntityFramework/Migrations

dotnet ef database update \
  --startup-project ElGuerre.Taskin.Api \
  --project ElGuerre.Taskin.Infrastructure
```

### API Documentation

- OpenAPI spec: `https://localhost:6001/openapi/v1.json`
- Scalar UI: `https://localhost:6001/scalar/v1`

---

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
| GET | `/Tasks/{id}` | Get task details |
| POST | `/Tasks` | Create task |
| PUT | `/Tasks/{id}` | Update task |
| DELETE | `/Tasks/{id}` | Delete task |
| GET | `/Pomodoros?taskId={id}` | Pomodoros by task |
| GET | `/Pomodoros/{id}` | Get pomodoro details |
| POST | `/Pomodoros` | Create pomodoro |
| PUT | `/Pomodoros/{id}` | Update pomodoro |
| DELETE | `/Pomodoros/{id}` | Delete pomodoro |

---

## Project Architecture

### Repository Structure

```
taskin2.0/
├── back/src/                                    # Backend (.NET 10 Clean Architecture)
│   ├── ElGuerre.Taskin.Api/                     # Web API Layer
│   │   ├── Controllers/                         # REST controllers (primary constructor + IMediator)
│   │   ├── Middleware/                           # ErrorHandlingMiddleware
│   │   ├── Program.cs                           # App startup + DI registration
│   │   └── appsettings.json                     # Configuration
│   ├── ElGuerre.Taskin.Application/             # Application Layer (CQRS)
│   │   ├── Projects/                            # Feature: Projects
│   │   │   ├── Commands/                        # Create, Update, Delete + Handlers + Validators
│   │   │   ├── Queries/                         # GetAll, GetById, GetStats + Handlers
│   │   │   └── DTOs/                            # ListDto, DetailsDto
│   │   ├── Tasks/                               # Feature: Tasks
│   │   ├── Pomodoros/                           # Feature: Pomodoros
│   │   ├── Data/                                # ITaskinDbContext interface
│   │   └── Observability/                       # TaskinMetrics, TelemetryConstants
│   ├── ElGuerre.Taskin.Domain/                  # Domain Layer (zero dependencies)
│   │   ├── Entities/                            # Project, Task, Pomodoro (sealed)
│   │   ├── Enums/                               # ProjectStatus, TaskStatus, etc.
│   │   └── SeedWork/                            # Entity, TrackedEntity, IUnitOfWork
│   ├── ElGuerre.Taskin.Infrastructure/          # Infrastructure Layer
│   │   ├── EntityFramework/
│   │   │   ├── EntityConfigurations/            # IEntityTypeConfiguration<T>
│   │   │   ├── Migrations/                      # EF Core migrations
│   │   │   └── TaskinDbContext.cs               # DbContext implementation
│   │   └── Middleware/                          # ErrorHandlingMiddleware
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
│   │   │   ├── features/                        # Feature areas (lazy-loaded)
│   │   │   │   ├── dashboard/                   # Dashboard + analytics
│   │   │   │   ├── projects/                    # Project CRUD
│   │   │   │   │   ├── pages/                   # Route page components
│   │   │   │   │   │   ├── projects/            # List view (plural)
│   │   │   │   │   │   ├── project-details/     # Detail view (-details suffix)
│   │   │   │   │   │   └── project-new/         # Create/edit form (-new suffix)
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

### Backend Dependency Graph

```
Api (Controllers, Middleware)
 └── Application (Commands, Queries, DTOs, Validators)
      └── Domain (Entities, Enums, SeedWork) ← Zero dependencies
 └── Infrastructure (EF Core, DbContext, Configs)
      └── Domain
```

**Dependency rule**: Inner layers NEVER reference outer layers. Domain has zero external dependencies.

### Frontend Dependency Rules

```
Features → Core, Shared (NEVER other Features)
Layout   → Core, Shared
Core     → Shared (environment, types)
Shared   → (no project dependencies)
```

---

## Angular 21 Best Practices

### File Naming Convention

**NEW files** use no-suffix convention. Existing files keep their current names.

| Type | File Name | Class Name |
|------|-----------|------------|
| Component | `task-card.ts` + `task-card.html` | `TaskCard` |
| Page component | `projects.ts` + `projects.html` | `Projects` |
| Service | `project.service.ts` | `ProjectService` |
| Store | `project.store.ts` | `ProjectStore` |
| Guard | `auth.guard.ts` | `authGuard` (function) |
| Interceptor | `api.interceptor.ts` | `apiInterceptor` (function) |
| Pipe | `time-ago.pipe.ts` | `TimeAgoPipe` |
| Interface/Type | `project.model.ts` | `ProjectListDto` |

### Page Naming Convention (STRICT)

| Page Type | Folder Name | Example |
|-----------|-------------|---------|
| List page | Plural entity name | `projects/projects.ts` |
| Create/New page | Entity + `-new` | `project-new/project-new.ts` |
| Detail/Edit page | Entity + `-details` | `project-details/project-details.ts` |

### Component Pattern

```typescript
import { Component, ChangeDetectionStrategy, input, output, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-task-card',
  templateUrl: './task-card.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.completed]': 'isCompleted()',
    '(click)': 'handleClick()',
  },
  imports: [MatButtonModule, MatIconModule, TranslocoDirective],
})
export class TaskCard {
  // Signal inputs
  task = input.required<TaskListDto>();
  disabled = input(false);

  // Signal outputs
  taskClick = output<TaskListDto>();

  // Computed
  readonly isCompleted = computed(() => this.task().status === 'Completed');

  handleClick(): void {
    if (!this.disabled()) {
      this.taskClick.emit(this.task());
    }
  }
}
```

### Template Control Flow

```html
<div *transloco="let t">
  @if (store.loading()) {
    <mat-spinner diameter="40" />
  } @else {
    @for (task of store.tasks(); track task.id) {
      <app-task-card [task]="task" (taskClick)="onSelect($event)" />
    } @empty {
      <p class="text-slate-500">{{ t('common.noResults') }}</p>
    }
  }

  @switch (task().status) {
    @case ('Active') { <span class="text-blue-600">{{ t('status.active') }}</span> }
    @case ('Completed') { <span class="text-green-600">{{ t('status.completed') }}</span> }
    @default { <span class="text-gray-500">{{ t('status.unknown') }}</span> }
  }

  @defer (on viewport) {
    <app-task-history [taskId]="task().id" />
  } @placeholder {
    <div class="h-20 bg-gray-100 animate-pulse rounded-lg"></div>
  }

  @let taskName = task().name;
  <h2>{{ taskName }}</h2>
</div>
```

**Critical**: Always use `track` with `@for`. Prefer `track item.id`.

### Material Design 3 Buttons

```html
<!-- MD3 variants (preferred) -->
<button matButton="filled">Primary Action</button>
<button matButton="elevated">Elevated</button>
<button matButton="outlined">Outlined</button>
<button matButton="tonal">Tonal</button>

<!-- Icon buttons -->
<button mat-icon-button><mat-icon>edit</mat-icon></button>
<button mat-icon-button><mat-icon svgIcon="mdi:pencil" /></mat-icon></button>

<!-- FAB variants -->
<button mat-fab><mat-icon>add</mat-icon></button>
<button mat-mini-fab><mat-icon>add</mat-icon></button>
```

### Tailwind CSS v4 Patterns

```html
<!-- Layout: Always flex + gap (NEVER space-y / space-x) -->
<div class="flex flex-col gap-4">
  <div class="flex items-center gap-2">...</div>
</div>

<!-- Responsive (use standard breakpoints) -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">

<!-- Shadows: shadow-xs (NOT shadow-sm) -->
<div class="rounded-xl shadow-xs border border-gray-100">

<!-- Dark sidebar design system -->
<nav class="bg-slate-900 text-slate-300">
  <a class="hover:bg-slate-800 active:bg-blue-600">
</nav>
```

### NgRx Signal Store Pattern

```typescript
@Injectable()
export class ProjectStore extends signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    activeProjects: computed(() => store.projects().filter(p => p.status === 'Active')),
  })),
  withMethods((
    store,
    service = inject(ProjectService),
    notification = inject(NotificationService),
  ) => ({
    // Queries: switchMap (cancels previous request)
    loadProjects: rxMethod<void>(pipe(
      switchMap(() => {
        patchState(store, { loading: true, error: null });
        return service.getProjects().pipe(
          tap({
            next: (response) => patchState(store, { projects: response.data, loading: false }),
            error: () => {
              patchState(store, { loading: false, error: 'Failed' });
              notification.notifyError('projects.errors.loadFailed');
            },
          })
        );
      })
    )),

    // Mutations: exhaustMap (ignores while busy)
    createProject: rxMethod<CreateProjectCommand>(pipe(
      exhaustMap((cmd) => {
        patchState(store, { saving: true });
        return service.create(cmd).pipe(
          tap({
            next: () => { patchState(store, { saving: false }); notification.notifySuccess('projects.messages.created'); },
            error: () => { patchState(store, { saving: false }); notification.notifyError('projects.errors.createFailed'); },
          })
        );
      })
    )),
  }))
) {}
```

---

## .NET 10 Best Practices

### Entity Pattern

```csharp
// Entity → TrackedEntity → sealed domain entity
public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public abstract class TrackedEntity : Entity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class Project : TrackedEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public ICollection<Task> Tasks { get; init; } = new List<Task>();
}
```

Rules: `required` on mandatory, `init` on immutable, `string?` for optional, `sealed` on concrete.

### CQRS Pattern (Command + Handler + Validator)

```csharp
// Command
public class CreateProjectCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

// Handler (primary constructor DI)
public class CreateProjectCommandHandler(
    ITaskinDbContext context,
    IUnitOfWork unitOfWork,
    TaskinMetrics metrics) : IRequestHandler<CreateProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var entity = new Project { Name = request.Name, Description = request.Description };
        context.Projects.Add(entity);
        await unitOfWork.SaveChangesAsync(ct);
        metrics.RecordProjectCreated();
        return entity.Id;
    }
}

// Validator
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
```

### Controller Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CollectionResponse<ProjectListDto>>> GetProjects(
        [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetProjectsQuery { Page = page, Size = size, Search = search }));

    [HttpPost]
    public async Task<ActionResult<ActionResponse>> CreateProject([FromBody] CreateProjectCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetProject), new { id }, new ActionResponse(id, "Created"));
    }
}
```

Rules: Primary constructor with `IMediator`, POST → `CreatedAtAction` (201), `{id:guid}` route constraints.

---

## i18n — Transloco

- **Languages**: `en.json` (default), `es.json`
- **Location**: `ui/src/src/assets/i18n/`
- **Directive**: Always `*transloco="let t"` (never pipe, never `TranslocoModule`)
- **Import**: `TranslocoDirective` from `@jsverse/transloco`
- **Both files**: Always add/update keys in BOTH `en.json` and `es.json`

```html
<div *transloco="let t">
  <h1>{{ t('projects.title') }}</h1>
  <button matButton="filled">{{ t('common.save') }}</button>
</div>
```

---

## Path Aliases

| Alias | Maps To |
|-------|---------|
| `@core` | `src/app/core` |
| `@shared` | `src/app/shared` |
| `@theme` | `src/styles` |
| `@env` | `src/environments` |

Always use aliases — never relative `../../` imports.

---

## Code Style Summary

### TypeScript
- **Semicolons**: Required
- **Quotes**: Single quotes
- **Line length**: 120 characters max
- **Indentation**: 2 spaces
- **No `any`**: Use `unknown` when type is uncertain
- **Trailing commas**: Yes in multiline

### C#
- **File-scoped namespaces**: `namespace X;`
- **Primary constructors**: Handlers + Controllers
- **Indentation**: 4 spaces
- **`required`**: On mandatory properties
- **`sealed`**: On concrete entities

### Templates
- **Native control flow**: `@if`, `@for`, `@switch`, `@defer`, `@let`
- **Track**: Required in every `@for`
- **Separate files**: Templates always in `.html` files

---

## Design System

- **Dark sidebar**: `bg-slate-900`, `text-slate-300` links, `bg-blue-600` active state
- **Page headers**: `text-2xl font-bold text-slate-900 tracking-tight` (no card wrapper)
- **Stat cards**: `rounded-xl`, `border-gray-100`, colored left border
- **Labels**: `text-xs font-medium text-slate-500 uppercase tracking-wider`
- **Section headers**: `text-sm font-semibold text-slate-900 uppercase tracking-wider`
- **Footer**: `text-xs text-slate-400`, compact `py-3`

---

## Git Workflow

- **Main branch**: `main` (target for PRs)
- **Feature branches**: `feature/{description}`
- **Conventional commits**: `feat:`, `fix:`, `refactor:`, `docs:`, `chore:`
