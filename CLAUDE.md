# CLAUDE.md

This file provides comprehensive guidance to Claude Code when working with this repository. For new implementation guidance or doubts, consult external documentation using appropriate tools.

## Project Overview

Taskin 2.0 is a full-stack productivity application implementing the Pomodoro Technique for task management:

- **Backend**: ASP.NET Core Web API (.NET 9) following Clean Architecture with CQRS
- **Frontend**: Angular 19 application with Angular Material and standalone components
- **Database**: SQL Server with Entity Framework Core
- **State Management**: NgRx Signal Store for Angular frontend

## Repository Structure

```
taskin2.0/
├── back/                          # Backend (ASP.NET Core)
│   └── src/Taskin.Api/           # Main backend solution
│       ├── ElGuerre.Taskin.Api/          # Web API layer
│       ├── ElGuerre.Taskin.Application/  # Application layer (CQRS)
│       ├── ElGuerre.Taskin.Domain/       # Domain entities
│       └── ElGuerre.Taskin.Infrastructure/ # Data access layer
└── ui/                           # Frontend (Angular)
    └── src/taskin-angular/       # Angular application
```

## Core Domain Entities

1. **Project**: Top-level container for organizing work
2. **Task**: Individual work items within projects
3. **Pomodoro**: Time tracking sessions associated with tasks

### Entity Relationships:

- `Project` → has many `Task`
- `Task` → belongs to `Project`, has many `Pomodoro`
- `Pomodoro` → belongs to `Task`

---

# Frontend (Angular 19)

## Development Commands

Run from `/ui/src/taskin-angular/` directory:

```bash
# Start development server (http://localhost:4200)
npm start

# Build for production
npm run build

# Run unit tests
npm test

# Build with watch mode
npm run watch
```

## Technology Stack

- **Angular 19**: Standalone components with signal-based architecture
- **Angular Material 19**: UI component library
- **NgRx Signals**: State management
- **Transloco**: Internationalization (en-US, es-ES, zh-CN, zh-TW)
- **TailwindCSS**: Utility-first styling
- **Luxon**: Date/time manipulation

## Architecture Patterns

### Feature-Based Structure

```
src/app/
├── core/                  # Singleton services, guards, interceptors
│   ├── auth/
│   ├── guards/
│   ├── interceptors/
│   └── services/
├── shared/               # Shared components, pipes, directives
│   ├── components/
│   ├── pipes/
│   ├── directives/
│   └── services/
├── features/            # Feature modules
│   ├── dashboard/
│   ├── projects/
│   ├── tasks/
│   └── pomodoros/
└── layout/              # Layout components
    ├── header/
    ├── sidebar/
    └── footer/
```

### Feature Module Structure

```
features/projects/
├── pages/              # Route components
│   ├── projects/       # List view
│   ├── project-details/ # Detail view
│   └── project-new/    # Create view
├── components/         # Feature-specific components
│   ├── project-card/
│   └── project-form/
├── shared/            # Feature services and types
│   ├── services/
│   ├── stores/
│   ├── types/
│   └── guards/
└── projects.routes.ts  # Feature routing
```

## Angular 19 Best Practices

### Component Generation

```bash
ng g c features/[feature]/pages/[component-name] --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
```

### Component Structure Template

```typescript
import { CommonModule } from "@angular/common";
import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation,
  inject,
} from "@angular/core";

@Component({
  selector: "app-component-name",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./component-name.component.html",
  styles: [],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ComponentNameComponent {
  private readonly _service = inject(ServiceName);

  // Use signals for reactive state
  data = signal<DataType[]>([]);
  loading = signal<boolean>(false);
}
```

### Smart vs Dumb Components

**Smart Components (Containers):**

```typescript
@Component({
  selector: "app-project-container",
  template: `
    <app-project-list
      [projects]="projects()"
      [loading]="loading()"
      (projectSelected)="onSelect($event)">
    </app-project-list>
  `,
  providers: [ProjectStore],
})
export class ProjectContainerComponent {
  private store = inject(ProjectStore);
  projects = this.store.selectSignal((state) => state.projects);
  loading = this.store.selectSignal((state) => state.loading);

  onSelect(project: Project): void {
    this.store.selectProject(project);
  }
}
```

**Dumb Components (Presentational):**

```typescript
@Component({
  selector: "app-project-list",
  template: `
    @if (loading()) {
    <app-loading-spinner></app-loading-spinner>
    } @else { @for (project of projects(); track project.id) {
    <app-project-card
      [project]="project"
      (selected)="projectSelected.emit(project)">
    </app-project-card>
    } }
  `,
})
export class ProjectListComponent {
  projects = input<Project[]>([]);
  loading = input<boolean>(false);
  projectSelected = output<Project>();
}
```

### Template Best Practices

```html
<!-- Use native control flow -->
@if (projects().length > 0) { @for (project of projects(); track project.id) {
<app-project-card [project]="project" />
} @empty {
<p>No projects found</p>
} }

<!-- Use class/style bindings -->
<div
  [class.active]="isActive()"
  [style.opacity]="isDisabled() ? '0.5' : '1'"></div>

<!-- Switch statements -->
@switch (status()) { @case ('loading') { <app-spinner /> } @case ('error') {
<app-error-message /> } @default { <app-project-list /> } }
```

### State Management with NgRx Signal Store

```typescript
type ProjectState = {
  projects: Project[];
  selectedProject: Project | null;
  loading: boolean;
  error: string | null;
};

@Injectable()
export class ProjectStore extends signalStore(
  { providedIn: "root" },
  withState<ProjectState>(initialState),
  withComputed((state) => ({
    activeProjects: computed(() =>
      state.projects().filter((p) => p.status === "active")
    ),
  })),
  withMethods((store, projectService = inject(ProjectService)) => ({
    loadProjects: rxMethod<void>(
      pipe(
        switchMap(() => {
          patchState(store, { loading: true, error: null });
          return projectService.getProjects().pipe(
            tapResponse({
              next: (projects) =>
                patchState(store, { projects, loading: false }),
              error: (error) =>
                patchState(store, {
                  error: "Failed to load projects",
                  loading: false,
                }),
            })
          );
        })
      )
    ),
  }))
) {}
```

### Services Pattern

```typescript
@Injectable({ providedIn: "root" })
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = "/api/projects";

  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.baseUrl);
  }

  createProject(request: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>(this.baseUrl, request);
  }
}
```

### Form Patterns

```typescript
export class ProjectFormComponent {
  private readonly fb = inject(FormBuilder);

  readonly form = this.fb.group({
    name: ["", [Validators.required, Validators.minLength(3)]],
    description: [""],
    status: ["active", Validators.required],
  });

  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value as ProjectFormData;
      // Handle submission
    }
  }
}
```

### Route Guards

```typescript
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService
    .isAuthenticated()
    .pipe(map((isAuth) => isAuth || router.navigate(["/login"])));
};
```

### Error Handling

```typescript
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private notificationService = inject(NotificationService);

  handleError(error: Error): void {
    if (error instanceof HttpErrorResponse) {
      const message =
        error.status === 401 ? "Please log in" : "An error occurred";
      this.notificationService.showError(message);
    }
  }
}
```

### Path Aliases

Configured in `tsconfig.json`:

- `@core` → `src/app/core`
- `@shared` → `src/app/shared`
- `@theme` → `src/app/theme`
- `@env` → `src/environments`

---

# Backend (.NET 9)

## Development Commands

Run from `/back/src/Taskin.Api/` directory:

```bash
# Build solution
dotnet build

# Run API server (https://localhost:5001)
dotnet run --project ElGuerre.Taskin.Api

# Run tests
dotnet test

# Database migrations
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations

# Update database
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

## Architecture (Clean Architecture)

### Layer Structure

1. **API Layer** (`ElGuerre.Taskin.Api`): Controllers, middleware, startup configuration
2. **Application Layer** (`ElGuerre.Taskin.Application`): Business logic, CQRS with MediatR
3. **Domain Layer** (`ElGuerre.Taskin.Domain`): Core entities and domain logic
4. **Infrastructure Layer** (`ElGuerre.Taskin.Infrastructure`): Data access with EF Core

### Technology Stack

- **.NET 9**: Latest framework version
- **ASP.NET Core Web API**: RESTful API services
- **Entity Framework Core**: ORM for SQL Server
- **MediatR**: CQRS implementation
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **Health Checks**: Application monitoring

### CQRS Pattern

```csharp
// Command structure
public record CreateProjectCommand(string Name, string Description) : IRequest<Project>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
{
    public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// Query structure
public record GetProjectsQuery : IRequest<List<Project>>;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<Project>>
{
    public async Task<List<Project>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### API Endpoints

Base URL: `https://localhost:5001/api`

**Projects**

- `GET /Projects` - List projects
- `GET /Projects/{id}` - Get project by ID
- `POST /Projects` - Create project
- `PUT /Projects/{id}` - Update project
- `DELETE /Projects/{id}` - Delete project

**Tasks**

- `GET /Tasks?projectId={projectId}` - Get tasks by project
- `GET /Tasks/{id}` - Get task by ID
- `POST /Tasks` - Create task
- `PUT /Tasks/{id}` - Update task
- `DELETE /Tasks/{id}` - Delete task

**Pomodoros**

- `GET /Pomodoros?taskId={taskId}` - Get pomodoros by task
- `GET /Pomodoros/{id}` - Get pomodoro by ID
- `POST /Pomodoros` - Create pomodoro
- `PUT /Pomodoros/{id}` - Update pomodoro
- `DELETE /Pomodoros/{id}` - Delete pomodoro

### Database Configuration

Connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskinDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Entity Framework Migrations

```bash
# Add migration
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations

# Update database
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

---

## Development Workflow

### Adding New Features

#### Frontend:

1. Generate feature structure in `src/app/features/`
2. Create pages using standalone components with signals
3. Implement NgRx Signal Store for state management
4. Add routing configuration
5. Create services for API communication

#### Backend:

1. Create domain entity in `Domain/Entities/`
2. Add Entity Framework configuration in `Infrastructure/EntityConfigurations/`
3. Create commands/queries in `Application/{FeatureName}/`
4. Implement handlers with validation
5. Add controller in `Api/Controllers/`
6. Create and apply database migration

## Best Practices

### Frontend

- Use `OnPush` change detection strategy
- Prefer signals over observables for component state
- Use `inject()` instead of constructor injection
- Follow Smart/Dumb component pattern
- Write explicit return types for methods
- Use proper TypeScript types instead of `any`

### Backend

- Follow Clean Architecture dependency rules
- Use CQRS pattern for business operations
- Implement proper validation in command validators
- Use Entity Framework migrations for schema changes
- Implement comprehensive error handling and logging

### Security

- Never expose sensitive data in client code
- Use proper authentication guards
- Validate permissions on both client and server
- Sanitize user inputs
- Never commit secrets or API keys

### Testing

- Write unit tests for components, services, and stores
- Use proper mocking for external dependencies
- Test user interactions and edge cases
- Maintain good test coverage

## Environment Configuration

- **Frontend Development**: `src/environments/environment.ts`
- **Frontend Production**: `src/environments/environment.prod.ts`
- **Backend Development**: `appsettings.Development.json`
- **Backend Production**: `appsettings.json`

## Important Notes

- Frontend uses Angular 19 with standalone components and signals
- Backend uses .NET 9 with nullable reference types enabled
- Database uses SQL Server with Entity Framework Core
- Both applications support production deployment
- CORS configured for Angular development (localhost:4200)
- Health checks available at `/health` endpoint
- Never include comments if they are not really necessary and only in complex parts of the code
