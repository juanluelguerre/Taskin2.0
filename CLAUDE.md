# CLAUDE.md

This file provides guidance to Claude Code when working with this repository.

## Context7 Integration

**IMPORTANT**: Always use Context7 MCP for up-to-date library documentation and patterns.

### Key Context7 Commands
```bash
# Angular ecosystem
Context7: @angular/core signals and dependency injection
Context7: @angular/material components and theming
Context7: @ngrx/signals state management

# .NET ecosystem
Context7: Microsoft.EntityFrameworkCore relationships and queries
Context7: MediatR CQRS implementation patterns
Context7: ASP.NET Core middleware and dependency injection

# Infrastructure
Context7: Docker containerization patterns
Context7: Azure deployment strategies
```

### Reference Files
- **ANGULAR_PATTERNS_REFERENCE.md**: Architectural patterns
- **ANGULAR_CLAUDE_GUIDE.md**: Development workflows
- **ANGULAR_COMPONENT_TEMPLATES.md**: Component templates

## Project Overview

Taskin 2.0 is a full-stack productivity application that implements the Pomodoro Technique for task management. The project consists of:

- **Backend**: ASP.NET Core Web API (.NET 9) following Clean Architecture
- **Frontend**: Angular 19 application with Angular Material and standalone components

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

Run commands from `/ui/src/taskin-angular/` directory:

```bash
# Start development server (http://localhost:4200)
npm start  # or ng serve --port 4200

# Build for production
npm run build  # or ng build

# Run unit tests
npm test  # or ng test

# Build with watch mode
npm run watch  # or ng build --watch --configuration development
```

## Component Generation

When creating new Angular components, use standalone mode (note: do NOT include `--standalone` flag as it's default):

```bash
ng g c features/projects/pages/component-name --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
```

## TypeScript Best Practices

- **Use strict type checking** - Enable strict mode in tsconfig.json
- **Prefer type inference** when the type is obvious
- **Avoid `any` type** - Use `unknown` when type is uncertain

```typescript
// ✅ CORRECT - Type inference and strict typing
const projects = signal<Project[]>([]);
const loading = signal(false);

// ✅ CORRECT - Use unknown instead of any
function handleApiResponse(data: unknown): Project[] {
  return data as Project[];
}

// ❌ INCORRECT - Avoid any
function handleApiResponse(data: any): Project[] {
  return data;
}
```

## Angular 19 Best Practices

### Architecture Patterns

#### Smart vs Dumb Components
- **Smart Components**: Manage state and business logic
- **Dumb Components**: Pure presentation with inputs/outputs

```typescript
// Smart component example
@Component({
  selector: 'app-project-container',
  template: `<app-project-list [projects]="projects()" (projectSelected)="onSelect($event)">`,
  providers: [ProjectStore]
})
export class ProjectContainerComponent {
  private store = inject(ProjectStore)
  projects = this.store.selectSignal(state => state.projects)
  
  onSelect(project: Project): void {
    this.store.selectProject(project)
  }
}
```

### Components

- **Use standalone components** (default in Angular 19)
- **Use signals for state management**
- **Use `NgOptimizedImage`** for static images
- **Use `host` object instead of `@HostBinding`/`@HostListener`**
- **Follow Smart/Dumb pattern**

```typescript
@Component({
  selector: 'app-project-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.active]': 'isActive()',
    '(click)': 'handleClick()'
  },
  template: `
    <div class="project-card">
      @if (project(); as proj) {
        <img [ngSrc]="proj.image" [alt]="proj.name" width="200" height="100">
        <h3>{{ proj.name }}</h3>
      }
    </div>
  `,
  imports: [NgOptimizedImage]
})
export class ProjectCardComponent {
  project = input.required<Project>();
  cardClick = output<Project>();
  
  readonly isActive = computed(() => this.project().status === 'active');
  
  handleClick(): void {
    this.cardClick.emit(this.project());
  }
}
```

### Templates

- **Use native control flow** (`@if`, `@for`, `@switch`)
- **Use `class` bindings instead of `ngClass`**
- **Use `style` bindings instead of `ngStyle`**

```html
<div [class.active]="isActive()" [style.opacity]="isDisabled() ? '0.5' : '1'">
  @if (projects().length > 0) {
    @for (project of projects(); track project.id) {
      <app-project-card [project]="project" (cardClick)="selectProject($event)" />
    } @empty {
      <p>No projects found</p>
    }
  } @else {
    <app-loading-spinner />
  }
  
  @switch (status()) {
    @case ('loading') { <app-spinner /> }
    @case ('error') { <app-error-message /> }
    @default { <app-project-list /> }
  }
</div>
```

### State Management

#### NgRx Signal Store Pattern

```typescript
@Injectable()
export class ProjectStore extends signalStore(
  { providedIn: 'root' },
  withState({
    projects: [] as Project[],
    selectedProject: null as Project | null,
    loading: false,
    error: null as string | null
  }),
  withComputed((state) => ({
    activeProjects: computed(() => state.projects().filter(p => p.status === 'active'))
  })),
  withMethods((store, projectService = inject(ProjectService)) => ({
    loadProjects: rxMethod<void>(pipe(
      switchMap(() => {
        patchState(store, { loading: true, error: null })
        return projectService.getProjects().pipe(
          tapResponse({
            next: (projects) => patchState(store, { projects, loading: false }),
            error: () => patchState(store, { error: 'Failed to load projects', loading: false })
          })
        )
      })
    ))
  }))
) {}
```

#### Local Component State

```typescript
export class ProjectsComponent {
  private readonly projects = signal<Project[]>([]);
  private readonly selectedId = signal<string | null>(null);
  
  readonly selectedProject = computed(() => {
    const id = this.selectedId();
    return this.projects().find(p => p.id === id) ?? null;
  });
  
  addProject(project: Project): void {
    this.projects.update(current => [...current, project]);
  }
}
```

### Services

```typescript
@Injectable({ providedIn: 'root' })
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/projects';
  
  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.baseUrl);
  }
  
  createProject(request: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>(this.baseUrl, request);
  }
}
```

### Forms

Use Reactive Forms with signal integration:

```typescript
export class ProjectFormComponent {
  private readonly fb = inject(FormBuilder);
  
  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    description: [''],
    status: ['active', Validators.required]
  });
  
  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value as ProjectFormData;
      // Handle submission
    }
  }
}
```

### Authentication & Authorization

```typescript
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService)
  const router = inject(Router)
  
  return authService.isAuthenticated().pipe(
    map(isAuth => isAuth || router.navigate(['/login']))
  )
}
```

### Error Handling

```typescript
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private notificationService = inject(NotificationService)
  
  handleError(error: Error): void {
    if (error instanceof HttpErrorResponse) {
      const message = error.status === 401 ? 'Please log in' : 'An error occurred'
      this.notificationService.showError(message)
    }
  }
}
```

### Performance Optimization

- Use `OnPush` change detection
- Implement lazy loading for routes
- Use virtual scrolling for large lists

```typescript
const routes: Routes = [
  {
    path: 'projects',
    loadChildren: () => import('./features/projects/projects.routes').then(m => m.routes)
  }
]
```

### Testing

Use Karma/Jasmine for unit tests:

```bash
npm test
```

## Architecture

### Project Structure

- **Core Module** (`src/app/core/`): Authentication, services, interceptors, bootstrap logic
- **Features** (`src/app/features/`): Domain-organized modules (dashboard, projects, tasks, pomodoros)
- **Layout** (`src/app/layout/`): Responsive layout with Material Design
- **Shared** (`src/app/shared/`): Reusable components, directives, pipes, services

### Key Technologies

- **Angular 19**: Standalone components with signal-based inputs/outputs
- **Angular Material 19**: UI component library  
- **NgRx Signals**: State management
- **Transloco**: Internationalization (en-US, es-ES, zh-CN, zh-TW)
- **TailwindCSS**: Utility-first styling

### Path Aliases

Configured in `tsconfig.json`:
- `@core` → `src/app/core`
- `@shared` → `src/app/shared`
- `@theme` → `src/app/theme`
- `@env` → `src/environments`

### Feature Structure

```
features/
├── projects/
│   ├── components/     # Reusable components
│   └── pages/         # Route components
│       ├── projects/  # List view
│       ├── project-details/  # Detail view
│       └── project-new/      # Create view
```

## Environment Configuration

- Development: `src/environments/environment.ts`
- Production: `src/environments/environment.prod.ts`

## Testing

- Unit tests with Karma/Jasmine: `npm test`
- E2E tests (configured but needs implementation)

---

# Backend (.NET 9)

## Development Commands

Run commands from `/back/src/Taskin.Api/` directory:

```bash
# Build solution
dotnet build

# Run API server (typically https://localhost:5001)
dotnet run --project ElGuerre.Taskin.Api

# Run tests
dotnet test

# Database migrations
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations

# Update database
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

## Architecture (.NET Core Clean Architecture)

The backend follows Clean Architecture with clear separation of concerns:

1. **API Layer** (`ElGuerre.Taskin.Api`): Controllers, middleware, and startup configuration
2. **Application Layer** (`ElGuerre.Taskin.Application`): Business logic, CQRS patterns with MediatR
3. **Domain Layer** (`ElGuerre.Taskin.Domain`): Core entities and domain logic
4. **Infrastructure Layer** (`ElGuerre.Taskin.Infrastructure`): Data access with Entity Framework Core

### Key Patterns

- **CQRS**: Commands and Queries separated using MediatR
- **Validation**: FluentValidation for command validation
- **Entity Framework**: Code-first approach with SQL Server
- **Error Handling**: Global exception middleware
- **Logging**: Serilog integration

### Command/Query Structure

```
Application/
├── Projects/
│   ├── Commands/
│   │   ├── CreateProjectCommand.cs
│   │   ├── CreateProjectCommandHandler.cs
│   │   └── CreateProjectCommandValidator.cs
│   └── Queries/
│       ├── GetProjectsQuery.cs
│       └── GetProjectsQueryHandler.cs
```

## API Endpoints

Base URL: `https://localhost:5001/api`

### Projects
- `GET /Projects` - List projects
- `GET /Projects/{id}` - Get project by ID
- `POST /Projects` - Create project
- `PUT /Projects/{id}` - Update project
- `DELETE /Projects/{id}` - Delete project

### Tasks
- `GET /Tasks?projectId={projectId}` - Get tasks by project
- `GET /Tasks/{id}` - Get task by ID
- `POST /Tasks` - Create task
- `PUT /Tasks/{id}` - Update task
- `DELETE /Tasks/{id}` - Delete task

### Pomodoros
- `GET /Pomodoros?taskId={taskId}` - Get pomodoros by task
- `GET /Pomodoros/{id}` - Get pomodoro by ID
- `POST /Pomodoros` - Create pomodoro
- `PUT /Pomodoros/{id}` - Update pomodoro
- `DELETE /Pomodoros/{id}` - Delete pomodoro

## Database Configuration

The application uses Entity Framework Core with SQL Server. Connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=server_name;Database=TaskinDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Database Schema Changes

Always use Entity Framework migrations:

```bash
# From back/src/Taskin.Api/ directory
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

## Development Guidelines

- Use CQRS pattern for all business operations
- Implement proper validation in command validators
- Follow Clean Architecture dependency rules
- Use Entity Framework migrations for schema changes
- Implement proper error handling and logging

## Environment Configuration

- Development: `appsettings.Development.json`
- Production: `appsettings.json`

## Testing

- Unit tests using xUnit (when implemented)
- Integration tests for API endpoints

## Technology Stack

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- MediatR (CQRS)
- Serilog (Logging)
- Swagger/OpenAPI

---

## Common Development Tasks

### Adding New Features

#### Frontend:
1. Generate feature module structure in `src/app/features/`
2. Create pages using standalone components with signal-based inputs
3. Implement routing in feature routing module
4. Add navigation menu items
5. Implement data services for API communication

#### Backend:
1. Create domain entity in `Domain/Entities/`
2. Add Entity Framework configuration in `Infrastructure/EntityConfigurations/`
3. Create commands/queries in `Application/{FeatureName}/`
4. Implement handlers with proper validation
5. Add controller in `Api/Controllers/`
6. Create and apply database migration

## Notes

- The Angular application has a comprehensive CLAUDE.md file at `ui/src/taskin-angular/CLAUDE.md` with detailed frontend-specific guidance
- Backend uses .NET 9 with nullable reference types enabled
- Frontend is fully functional with all major features implemented
- Both applications are ready for production deployment with proper build configurations

## Database & Caching

- **SQL Server**: Use Entity Framework Core with repository pattern
- **Redis**: Implement distributed caching with StackExchange.Redis
- **MongoDB**: For analytics and document storage (future enhancement)

```bash
# Context7 commands for database work
Context7: Microsoft.EntityFrameworkCore performance optimization
Context7: StackExchange.Redis caching strategies
Context7: MongoDB.Driver document operations
```

## Docker & Infrastructure

- **Containerization**: Multi-stage Dockerfiles for .NET API and Angular
- **Orchestration**: Docker Compose for development environment
- **Cloud Deployment**: Azure Container Apps for production

```bash
# Context7 commands for infrastructure
Context7: Docker multi-stage builds optimization
Context7: Azure Container Apps deployment patterns
Context7: Azure Bicep infrastructure as code
```


## DevOps & Monitoring

- **CI/CD**: GitHub Actions with automated testing and deployment
- **Monitoring**: Azure Application Insights with health checks
- **Security**: Automated vulnerability scanning with Trivy

```bash
# Context7 commands for DevOps
Context7: GitHub Actions workflow optimization
Context7: Azure Application Insights configuration
Context7: .NET health checks implementation
```

## Development Workflow

### Context7 Usage
1. **Before new features**: Get latest patterns and best practices
2. **When debugging**: Get troubleshooting guides and solutions
3. **During code reviews**: Reference architectural guidance
4. **When updating dependencies**: Check migration guides
5. **For optimization**: Get latest optimization techniques

**Important**: HTML templates must always be in separate files, never embedded.