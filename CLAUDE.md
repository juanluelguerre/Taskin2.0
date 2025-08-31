# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Context7 Integration

**IMPORTANT**: Always use Context7 MCP for library documentation and best practices. Context7 provides up-to-date documentation and patterns for all technologies used in this project.

### When to Use Context7

1. **Before implementing new features** - Get latest patterns and best practices
2. **When working with unfamiliar libraries** - Get comprehensive documentation
3. **For debugging complex issues** - Get troubleshooting guides and solutions
4. **When updating dependencies** - Check for breaking changes and migration guides

### Context7 Usage by Technology

#### Angular & Frontend
```bash
# Angular framework and ecosystem
Context7: @angular/core signals and dependency injection
Context7: @angular/material components and theming
Context7: @ngrx/signals state management
Context7: @angular/cdk accessibility and layout
Context7: rxjs operators and patterns
Context7: transloco internationalization
```

#### .NET Backend
```bash
# .NET ecosystem and frameworks
Context7: Microsoft.EntityFrameworkCore relationships and queries
Context7: MediatR CQRS implementation patterns
Context7: FluentValidation validation patterns
Context7: Serilog structured logging
Context7: ASP.NET Core middleware and dependency injection
```

#### Databases & Caching
```bash
# Database technologies
Context7: Entity Framework Core SQL Server patterns
Context7: MongoDB.Driver document operations
Context7: StackExchange.Redis caching strategies
```

#### Infrastructure & DevOps
```bash
# Infrastructure and deployment
Context7: Docker containerization patterns
Context7: Azure deployment strategies
Context7: CI/CD pipeline best practices
```

### Reference Files

This project includes three comprehensive Angular reference files that should be consulted alongside Context7:

- **ANGULAR_PATTERNS_REFERENCE.md**: Comprehensive architectural patterns and best practices
- **ANGULAR_CLAUDE_GUIDE.md**: Development workflows and project structure guidelines
- **ANGULAR_COMPONENT_TEMPLATES.md**: Ready-to-use component templates and examples

These files provide project-specific patterns that complement Context7's general library documentation.

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

#### Smart vs Dumb Components Pattern

Always separate components into smart (container) and dumb (presentational) components:

**Smart Components (Containers):**
```typescript
// Container manages state and business logic
@Component({
  selector: 'app-project-container',
  template: `
    <app-project-list
      [projects]="projects()"
      [loading]="loading()"
      (projectSelected)="onProjectSelected($event)"
      (projectDeleted)="onProjectDeleted($event)">
    </app-project-list>
  `,
  providers: [ProjectStore]
})
export class ProjectContainerComponent {
  private store = inject(ProjectStore)
  
  projects = this.store.selectSignal(state => state.projects)
  loading = this.store.selectSignal(state => state.loading)

  onProjectSelected(project: Project): void {
    this.store.selectProject(project)
  }

  onProjectDeleted(projectId: string): void {
    this.store.deleteProject(projectId)
  }
}
```

**Dumb Components (Presentational):**
```typescript
// Pure presentation with inputs/outputs
@Component({
  selector: 'app-project-list',
  template: `
    @if (loading()) {
      <app-loading-spinner></app-loading-spinner>
    } @else {
      @for (project of projects(); track project.id) {
        <app-project-card
          [project]="project"
          (selected)="projectSelected.emit(project)"
          (deleted)="projectDeleted.emit(project.id)">
        </app-project-card>
      }
    }
  `
})
export class ProjectListComponent {
  projects = input<Project[]>([])
  loading = input<boolean>(false)
  
  projectSelected = output<Project>()
  projectDeleted = output<string>()
}
```

#### Component Composition Pattern

Use content projection for flexible, reusable components:

```typescript
@Component({
  selector: 'app-base-list',
  template: `
    <div class="list-container">
      <div class="list-header">
        <ng-content select="[slot=header]"></ng-content>
      </div>
      <div class="list-content">
        <ng-content select="[slot=content]"></ng-content>
      </div>
      <div class="list-footer">
        <ng-content select="[slot=footer]"></ng-content>
      </div>
    </div>
  `
})
export class BaseListComponent {}

// Usage:
// <app-base-list>
//   <div slot="header"><h2>Projects</h2></div>
//   <div slot="content"><app-project-table></app-project-table></div>
//   <div slot="footer"><app-pagination></app-pagination></div>
// </app-base-list>
```

### Components

- **Always use standalone components** (default in Angular 19)
- **DO NOT set `standalone: true`** inside decorators (it's default)
- **Use signals for state management**
- **Implement lazy loading** for feature routes
- **Use `NgOptimizedImage`** for all static images
- **DO NOT use `@HostBinding` and `@HostListener`** - Use `host` object instead
- **Follow Smart/Dumb component pattern**
- **Use component composition with content projection**

```typescript
// ✅ CORRECT - Modern Angular 19 component
import { Component, ChangeDetectionStrategy, input, output, signal, computed, inject } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';

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
        <img [ngSrc]="proj.image" [alt]="proj.name" width="200" height="100" priority>
        <h3>{{ proj.name }}</h3>
        @if (showDescription()) {
          <p>{{ proj.description }}</p>
        }
      }
    </div>
  `,
  imports: [NgOptimizedImage]
})
export class ProjectCardComponent {
  // Signal-based inputs
  project = input.required<Project>();
  showDescription = input<boolean>(true);
  
  // Signal-based outputs
  cardClick = output<Project>();
  
  // Internal signals
  private readonly isLoading = signal(false);
  
  // Computed values
  readonly isActive = computed(() => 
    this.project().status === 'active'
  );
  
  // Use inject() instead of constructor injection
  private readonly projectService = inject(ProjectService);
  
  handleClick(): void {
    this.cardClick.emit(this.project());
  }
}
```

### Templates

- **Keep templates simple** and avoid complex logic
- **Use native control flow** (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- **Use async pipe** to handle observables
- **DO NOT use `ngClass`** - Use `class` bindings instead
- **DO NOT use `ngStyle`** - Use `style` bindings instead

```html
<!-- ✅ CORRECT - Native control flow and direct bindings -->
<div 
  [class.active]="isActive()"
  [class.loading]="isLoading()"
  [style.opacity]="isDisabled() ? '0.5' : '1'">
  
  @if (projects().length > 0) {
    @for (project of projects(); track project.id) {
      <app-project-card 
        [project]="project"
        (cardClick)="selectProject($event)" />
    } @empty {
      <p>No projects found</p>
    }
  } @else {
    <app-loading-spinner />
  }
  
  @switch (status()) {
    @case ('loading') {
      <app-spinner />
    }
    @case ('error') {
      <app-error-message />
    }
    @default {
      <app-project-list />
    }
  }
</div>

<!-- ❌ INCORRECT - Old structural directives and ngClass/ngStyle -->
<div 
  [ngClass]="{ active: isActive(), loading: isLoading() }"
  [ngStyle]="{ opacity: isDisabled() ? '0.5' : '1' }">
  
  <app-project-card 
    *ngFor="let project of projects(); trackBy: trackProject"
    [project]="project" />
</div>
```

### State Management

#### NgRx Signal Store Pattern

For complex feature state, use NgRx Signal Store:

```typescript
import { Injectable } from '@angular/core'
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals'
import { rxMethod } from '@ngrx/signals/rxjs-interop'
import { tapResponse } from '@ngrx/operators'
import { computed } from '@angular/core'
import { switchMap, exhaustMap } from 'rxjs'

type ProjectState = {
  projects: Project[]
  selectedProject: Project | null
  loading: boolean
  saving: boolean
  error: string | null
  filters: ProjectFilters
}

const initialState: ProjectState = {
  projects: [],
  selectedProject: null,
  loading: false,
  saving: false,
  error: null,
  filters: { status: null, search: '' }
}

@Injectable()
export class ProjectStore extends signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((state) => ({
    filteredProjects: computed(() => 
      applyFilters(state.projects(), state.filters())
    ),
    activeProjectsCount: computed(() => 
      state.projects().filter(p => p.status === 'active').length
    ),
  })),
  withMethods((store, projectService = inject(ProjectService)) => ({
    
    loadProjects: rxMethod<void>(
      pipe(
        switchMap(() => {
          patchState(store, { loading: true, error: null })
          
          return projectService.getProjects().pipe(
            tapResponse({
              next: (projects) => patchState(store, { projects, loading: false }),
              error: (error) => patchState(store, { 
                error: 'Failed to load projects', 
                loading: false 
              })
            })
          )
        })
      )
    ),

    createProject: rxMethod<CreateProjectRequest>(
      pipe(
        exhaustMap((request) => {
          patchState(store, { saving: true, error: null })
          
          return projectService.createProject(request).pipe(
            tapResponse({
              next: (newProject) => patchState(store, {
                projects: [newProject, ...store.projects()],
                saving: false
              }),
              error: (error) => patchState(store, { 
                error: 'Failed to create project', 
                saving: false 
              })
            })
          )
        })
      )
    ),

    setFilters: (filters: ProjectFilters) => 
      patchState(store, { filters }),
    
    clearError: () => patchState(store, { error: null }),
  }))
) {}
```

#### Local Component State

- **Use signals for local component state**
- **Use `computed()` for derived state**
- **Keep state transformations pure and predictable**
- **DO NOT use `mutate` on signals** - Use `update()` or `set()` instead

```typescript
// ✅ CORRECT - Signal state management
export class ProjectsComponent {
  // Local state with signals
  private readonly projects = signal<Project[]>([]);
  private readonly loading = signal(false);
  private readonly selectedId = signal<string | null>(null);
  
  // Computed derived state
  readonly selectedProject = computed(() => {
    const id = this.selectedId();
    return this.projects().find(p => p.id === id) ?? null;
  });
  
  readonly activeProjects = computed(() => 
    this.projects().filter(p => p.status === 'active')
  );
  
  // Proper signal updates
  addProject(project: Project): void {
    this.projects.update(current => [...current, project]);
  }
  
  updateProject(id: string, updates: Partial<Project>): void {
    this.projects.update(current => 
      current.map(p => p.id === id ? { ...p, ...updates } : p)
    );
  }
  
  // ❌ INCORRECT - Don't use mutate
  // addProject(project: Project): void {
  //   this.projects.mutate(current => current.push(project));
  // }
}
```

#### State Synchronization Pattern

For cross-component communication:

```typescript
@Injectable({ providedIn: 'root' })
export class AppStateService {
  private _selectedProject = signal<Project | null>(null)
  private _theme = signal<Theme>('light')
  
  readonly selectedProject = this._selectedProject.asReadonly()
  readonly theme = this._theme.asReadonly()
  
  setSelectedProject(project: Project | null): void {
    this._selectedProject.set(project)
  }
  
  setTheme(theme: Theme): void {
    this._theme.set(theme)
    document.body.className = `theme-${theme}`
  }
}
```

### Services

#### Repository Pattern

Abstract data access with repository pattern:

```typescript
export abstract class Repository<T, TCreate = T, TUpdate = T> {
  abstract getAll(): Observable<T[]>
  abstract getById(id: string): Observable<T>
  abstract create(item: TCreate): Observable<T>
  abstract update(id: string, item: TUpdate): Observable<T>
  abstract delete(id: string): Observable<void>
}

@Injectable({ providedIn: 'root' })
export class ProjectRepository extends Repository<Project, CreateProjectRequest, UpdateProjectRequest> {
  private readonly http = inject(HttpClient)
  private readonly baseUrl = '/api/projects'
  
  getAll(): Observable<Project[]> {
    return this.http.get<Project[]>(this.baseUrl)
  }
  
  getById(id: string): Observable<Project> {
    return this.http.get<Project>(`${this.baseUrl}/${id}`)
  }
  
  create(request: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>(this.baseUrl, request)
  }
  
  update(id: string, request: UpdateProjectRequest): Observable<Project> {
    return this.http.put<Project>(`${this.baseUrl}/${id}`, request)
  }
  
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
  }
}
```

#### Facade Pattern

Combine multiple services behind a single interface:

```typescript
@Injectable({ providedIn: 'root' })
export class ProjectFacade {
  private readonly projectRepository = inject(ProjectRepository)
  private readonly projectStore = inject(ProjectStore)
  private readonly notificationService = inject(NotificationService)
  
  // Expose store selectors
  projects$ = this.projectStore.projects$
  selectedProject$ = this.projectStore.selectedProject$
  loading$ = this.projectStore.loading$
  
  async loadProjects(): Promise<void> {
    try {
      this.projectStore.setLoading(true)
      const projects = await firstValueFrom(this.projectRepository.getAll())
      this.projectStore.setProjects(projects)
    } catch (error) {
      this.notificationService.showError('Failed to load projects')
    } finally {
      this.projectStore.setLoading(false)
    }
  }
  
  async createProject(request: CreateProjectRequest): Promise<void> {
    try {
      const project = await firstValueFrom(this.projectRepository.create(request))
      this.projectStore.addProject(project)
      this.notificationService.showSuccess('Project created successfully')
    } catch (error) {
      this.notificationService.showError('Failed to create project')
      throw error
    }
  }
}
```

#### Service Best Practices

- **Design services around single responsibility**
- **Use `providedIn: 'root'` for singleton services**
- **Use `inject()` function instead of constructor injection**
- **Implement repository pattern for data access**
- **Use facade pattern for complex operations**

```typescript
// ✅ CORRECT - Modern service with inject()
@Injectable({ providedIn: 'root' })
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(API_BASE_URL);
  
  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.apiUrl}/projects`);
  }
  
  // ❌ INCORRECT - Constructor injection
  // constructor(
  //   private http: HttpClient,
  //   @Inject(API_BASE_URL) private apiUrl: string
  // ) {}
}
```

### Forms

- **Prefer Reactive forms instead of Template-driven ones**

```typescript
// ✅ CORRECT - Reactive forms with signals
export class ProjectFormComponent {
  private readonly fb = inject(FormBuilder);
  
  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    description: [''],
    status: ['active', Validators.required]
  });
  
  readonly isValid = signal(false);
  
  constructor() {
    // React to form changes
    effect(() => {
      this.isValid.set(this.form.valid);
    });
  }
}
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