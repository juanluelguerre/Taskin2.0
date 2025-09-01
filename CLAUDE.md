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

#### Form State Management Pattern

For complex forms, use dedicated state management:

```typescript
type FormState<T> = {
  data: T
  originalData: T
  isDirty: boolean
  isValid: boolean
  errors: Record<string, string>
  isSubmitting: boolean
}

@Injectable()
export class FormStateManager<T> extends signalStore(
  withState<FormState<T>>({
    data: {} as T,
    originalData: {} as T,
    isDirty: false,
    isValid: true,
    errors: {},
    isSubmitting: false
  }),
  withMethods((store) => ({
    initializeForm: (data: T) => {
      patchState(store, {
        data: { ...data },
        originalData: { ...data },
        isDirty: false,
        errors: {}
      })
    },
    
    updateField: <K extends keyof T>(field: K, value: T[K]) => {
      const currentData = store.data()
      const newData = { ...currentData, [field]: value }
      
      patchState(store, {
        data: newData,
        isDirty: !isEqual(newData, store.originalData())
      })
    },
    
    setErrors: (errors: Record<string, string>) => {
      patchState(store, {
        errors,
        isValid: Object.keys(errors).length === 0
      })
    },
    
    reset: () => {
      patchState(store, {
        data: { ...store.originalData() },
        isDirty: false,
        errors: {}
      })
    }
  }))
) {}
```

#### Custom Form Controls

Create reusable form controls:

```typescript
@Component({
  selector: 'app-project-selector',
  template: `
    <mat-form-field appearance="outline" class="w-full">
      <mat-label>{{ label() }}</mat-label>
      <mat-select 
        [value]="value()"
        [disabled]="disabled()"
        (selectionChange)="onSelectionChange($event)">
        @for (project of projects(); track project.id) {
          <mat-option [value]="project.id">{{ project.name }}</mat-option>
        }
      </mat-select>
      @if (error()) {
        <mat-error>{{ error() }}</mat-error>
      }
    </mat-form-field>
  `,
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => ProjectSelectorComponent),
    multi: true
  }]
})
export class ProjectSelectorComponent implements ControlValueAccessor {
  label = input<string>('Select Project')
  projects = input<Project[]>([])
  error = input<string>('')
  
  value = signal<string>('')
  disabled = signal<boolean>(false)
  
  private onChange = (value: string): void => {}
  private onTouched = (): void => {}
  
  onSelectionChange(event: MatSelectChange): void {
    this.value.set(event.value)
    this.onChange(event.value)
    this.onTouched()
  }
  
  writeValue(value: string): void {
    this.value.set(value || '')
  }
  
  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn
  }
  
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn
  }
  
  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled)
  }
}
```

#### Form Best Practices

- **Prefer Reactive forms instead of Template-driven ones**
- **Use custom form controls for reusability**
- **Implement proper validation strategies**
- **Handle form state with signals**

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
  readonly isDirty = signal(false);
  
  constructor() {
    // React to form changes
    effect(() => {
      this.isValid.set(this.form.valid);
      this.isDirty.set(this.form.dirty);
    });
  }
  
  getFieldError(fieldName: string): string {
    const field = this.form.get(fieldName)
    if (field?.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`
      if (field.errors['minlength']) return `${fieldName} is too short`
    }
    return ''
  }
  
  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value as ProjectFormData
      // Handle submission
    } else {
      this.form.markAllAsTouched()
    }
  }
}
```

### Authentication & Authorization

#### JWT Token Management

```typescript
@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly TOKEN_KEY = 'auth_token'
  private readonly REFRESH_TOKEN_KEY = 'refresh_token'
  
  setTokens(accessToken: string, refreshToken?: string): void {
    localStorage.setItem(this.TOKEN_KEY, accessToken)
    if (refreshToken) {
      localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken)
    }
  }
  
  getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY)
  }
  
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY)
  }
  
  removeTokens(): void {
    localStorage.removeItem(this.TOKEN_KEY)
    localStorage.removeItem(this.REFRESH_TOKEN_KEY)
  }
  
  isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      const exp = payload.exp * 1000
      return Date.now() >= exp
    } catch {
      return true
    }
  }
}
```

#### Route Guards

```typescript
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService)
  const router = inject(Router)
  
  return authService.isAuthenticated().pipe(
    map(isAuth => {
      if (isAuth) {
        return true
      } else {
        router.navigate(['/login'])
        return false
      }
    })
  )
}

export const permissionGuard = (permission: string): CanActivateFn => {
  return () => {
    const permissionService = inject(PermissionService)
    const router = inject(Router)
    
    return permissionService.hasPermission(permission).pipe(
      map(hasPermission => {
        if (hasPermission) {
          return true
        } else {
          router.navigate(['/forbidden'])
          return false
        }
      })
    )
  }
}

// Usage in routes
const routes: Routes = [
  {
    path: 'admin',
    canActivate: [authGuard, permissionGuard('admin:access')],
    loadChildren: () => import('./admin/admin.routes').then(m => m.routes)
  }
]
```

### Error Handling

#### Global Error Handler

```typescript
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private notificationService = inject(NotificationService)
  private loggingService = inject(LoggingService)
  
  handleError(error: Error): void {
    console.error('Global error caught:', error)
    
    this.loggingService.logError(error)
    
    if (error instanceof HttpErrorResponse) {
      this.handleHttpError(error)
    } else {
      this.notificationService.showError(
        'An unexpected error occurred. Please try again.'
      )
    }
  }
  
  private handleHttpError(error: HttpErrorResponse): void {
    let message = 'An error occurred'
    
    switch (error.status) {
      case 401:
        message = 'You are not authorized. Please log in.'
        break
      case 403:
        message = 'You do not have permission to perform this action.'
        break
      case 404:
        message = 'The requested resource was not found.'
        break
      case 500:
        message = 'A server error occurred. Please try again later.'
        break
      default:
        message = error.error?.message || error.message
    }
    
    this.notificationService.showError(message)
  }
}
```

#### HTTP Error Interceptor

```typescript
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private authService = inject(AuthService)
  private notificationService = inject(NotificationService)
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.authService.logout()
        } else if (error.status >= 500) {
          this.notificationService.showError('Server error. Please try again later.')
        }
        
        return throwError(() => error)
      })
    )
  }
}
```

### Performance Optimization

#### OnPush Change Detection

```typescript
@Component({
  selector: 'app-project-list',
  template: `
    @for (project of projects(); track project.id) {
      <app-project-item
        [project]="project"
        (projectChanged)="onProjectChanged($event)">
      </app-project-item>
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectListComponent {
  projects = input<Project[]>([])
  projectChanged = output<Project>()
  
  onProjectChanged(project: Project): void {
    this.projectChanged.emit(project)
  }
}
```

#### Virtual Scrolling

```typescript
@Component({
  selector: 'app-virtual-project-list',
  template: `
    <cdk-virtual-scroll-viewport itemSize="60" class="viewport">
      @for (project of projects(); track project.id) {
        <app-project-row [project]="project"></app-project-row>
      }
    </cdk-virtual-scroll-viewport>
  `,
  styles: [`
    .viewport { height: 400px; width: 100%; }
  `]
})
export class VirtualProjectListComponent {
  projects = input<Project[]>([])
}
```

#### Lazy Loading

```typescript
// Feature route lazy loading
const routes: Routes = [
  {
    path: 'projects',
    loadChildren: () => import('./features/projects/projects.routes').then(m => m.routes)
  },
  {
    path: 'tasks',
    loadChildren: () => import('./features/tasks/tasks.routes').then(m => m.routes)
  }
]

// Component lazy loading
@Component({
  template: `
    @if (showAdvanced()) {
      <ng-container *ngComponentOutlet="advancedComponent"></ng-container>
    }
  `
})
export class MainComponent {
  showAdvanced = signal(false)
  advancedComponent: Type<any> | null = null
  
  async loadAdvancedComponent(): Promise<void> {
    if (!this.advancedComponent) {
      const { AdvancedComponent } = await import('./advanced/advanced.component')
      this.advancedComponent = AdvancedComponent
    }
    this.showAdvanced.set(true)
  }
}
```

### Testing Patterns

#### Component Testing

```typescript
describe('ProjectListComponent', () => {
  let component: ProjectListComponent
  let fixture: ComponentFixture<ProjectListComponent>

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectListComponent]
    }).compileComponents()

    fixture = TestBed.createComponent(ProjectListComponent)
    component = fixture.componentInstance
  })

  it('should display projects', () => {
    const mockProjects: Project[] = [
      { id: '1', name: 'Project 1', description: 'Test project' },
      { id: '2', name: 'Project 2', description: 'Another test project' }
    ]

    fixture.componentRef.setInput('projects', mockProjects)
    fixture.detectChanges()

    const projectElements = fixture.debugElement.queryAll(By.css('.project-item'))
    expect(projectElements.length).toBe(2)
  })

  it('should emit projectSelected when project is clicked', () => {
    spyOn(component.projectSelected, 'emit')
    const project: Project = { id: '1', name: 'Test Project', description: 'Test' }

    component.onProjectSelected(project)

    expect(component.projectSelected.emit).toHaveBeenCalledWith(project)
  })
})
```

#### Service Testing

```typescript
describe('ProjectService', () => {
  let service: ProjectService
  let httpMock: HttpTestingController

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProjectService]
    })

    service = TestBed.inject(ProjectService)
    httpMock = TestBed.inject(HttpTestingController)
  })

  afterEach(() => {
    httpMock.verify()
  })

  it('should fetch projects', () => {
    const mockProjects: Project[] = [
      { id: '1', name: 'Project 1', description: 'Test' }
    ]

    service.getProjects().subscribe(projects => {
      expect(projects).toEqual(mockProjects)
    })

    const req = httpMock.expectOne('/api/projects')
    expect(req.request.method).toBe('GET')
    req.flush(mockProjects)
  })
})
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

---

# Database Strategies

## SQL Server with Entity Framework Core

### Entity Framework Patterns

#### Repository Pattern with EF Core

```csharp
public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey id);
    Task<int> SaveChangesAsync();
}

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> 
    where TEntity : class
{
    protected readonly TaskinDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(TaskinDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
```

#### Query Optimization Patterns

```csharp
public class ProjectRepository : Repository<Project, string>
{
    public ProjectRepository(TaskinDbContext context) : base(context) { }

    // Optimized queries with includes
    public async Task<Project?> GetProjectWithTasksAsync(string projectId)
    {
        return await _dbSet
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Pomodoros)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    // Projection for performance
    public async Task<IEnumerable<ProjectSummaryDto>> GetProjectSummariesAsync()
    {
        return await _dbSet
            .Select(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                TaskCount = p.Tasks.Count(),
                CompletedTaskCount = p.Tasks.Count(t => t.IsCompleted),
                TotalPomodoros = p.Tasks.SelectMany(t => t.Pomodoros).Count()
            })
            .ToListAsync();
    }

    // Efficient filtering and paging
    public async Task<PagedResult<Project>> GetProjectsPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null,
        bool? isActive = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || 
                                   p.Description.Contains(searchTerm));
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Project>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // Bulk operations
    public async Task BulkUpdateProjectStatusAsync(IEnumerable<string> projectIds, bool isActive)
    {
        await _dbSet
            .Where(p => projectIds.Contains(p.Id))
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsActive, isActive));
    }
}
```

#### Database Context Configuration

```csharp
public class TaskinDbContext : DbContext
{
    public TaskinDbContext(DbContextOptions<TaskinDbContext> options) : base(options) { }

    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<Pomodoro> Pomodoros { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskinDbContext).Assembly);

        // Global query filters for soft delete
        modelBuilder.Entity<Project>()
            .HasQueryFilter(p => !p.IsDeleted);
            
        modelBuilder.Entity<TaskItem>()
            .HasQueryFilter(t => !t.IsDeleted);
    }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => new { p.IsActive, p.CreatedAt });
    }
}
```

### Context7 Usage for SQL Server

```bash
# Get Entity Framework Core best practices
Context7: Microsoft.EntityFrameworkCore performance optimization
Context7: Entity Framework Core relationships and navigation
Context7: EF Core migrations and database management
Context7: Entity Framework Core query optimization
```

---

## MongoDB Integration

### MongoDB Document Patterns

#### Document Repository Pattern

```csharp
public interface IMongoRepository<TDocument, TKey> where TDocument : IDocument<TKey>
{
    Task<IEnumerable<TDocument>> GetAllAsync();
    Task<TDocument?> GetByIdAsync(TKey id);
    Task<TDocument> CreateAsync(TDocument document);
    Task<TDocument> UpdateAsync(TDocument document);
    Task DeleteAsync(TKey id);
    Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter);
}

public class MongoRepository<TDocument, TKey> : IMongoRepository<TDocument, TKey> 
    where TDocument : class, IDocument<TKey>
{
    protected readonly IMongoCollection<TDocument> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<TDocument>(collectionName);
    }

    public async Task<IEnumerable<TDocument>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<TDocument?> GetByIdAsync(TKey id)
    {
        return await _collection.Find(doc => doc.Id.Equals(id)).FirstOrDefaultAsync();
    }

    public async Task<TDocument> CreateAsync(TDocument document)
    {
        await _collection.InsertOneAsync(document);
        return document;
    }

    public async Task<TDocument> UpdateAsync(TDocument document)
    {
        await _collection.ReplaceOneAsync(doc => doc.Id.Equals(document.Id), document);
        return document;
    }

    public async Task DeleteAsync(TKey id)
    {
        await _collection.DeleteOneAsync(doc => doc.Id.Equals(id));
    }

    public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }
}
```

#### Aggregation Pipeline Patterns

```csharp
public class ProjectAnalyticsRepository
{
    private readonly IMongoCollection<Project> _projects;
    private readonly IMongoCollection<TaskItem> _tasks;

    public ProjectAnalyticsRepository(IMongoDatabase database)
    {
        _projects = database.GetCollection<Project>("projects");
        _tasks = database.GetCollection<TaskItem>("tasks");
    }

    public async Task<IEnumerable<ProjectStatistics>> GetProjectStatisticsAsync()
    {
        var pipeline = new[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                ["from"] = "tasks",
                ["localField"] = "_id",
                ["foreignField"] = "projectId",
                ["as"] = "tasks"
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                ["totalTasks"] = new BsonDocument("$size", "$tasks"),
                ["completedTasks"] = new BsonDocument("$size", 
                    new BsonDocument("$filter", new BsonDocument
                    {
                        ["input"] = "$tasks",
                        ["cond"] = new BsonDocument("$eq", new BsonArray { "$$this.isCompleted", true })
                    })
                )
            }),
            new BsonDocument("$project", new BsonDocument
            {
                ["_id"] = 1,
                ["name"] = 1,
                ["totalTasks"] = 1,
                ["completedTasks"] = 1,
                ["completionRate"] = new BsonDocument("$cond", new BsonDocument
                {
                    ["if"] = new BsonDocument("$gt", new BsonArray { "$totalTasks", 0 }),
                    ["then"] = new BsonDocument("$divide", new BsonArray { "$completedTasks", "$totalTasks" }),
                    ["else"] = 0
                })
            })
        };

        return await _projects.Aggregate<ProjectStatistics>(pipeline).ToListAsync();
    }

    public async Task<IEnumerable<PomodoroTrendData>> GetPomodoroTrendsAsync(DateTime startDate, DateTime endDate)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                ["createdAt"] = new BsonDocument
                {
                    ["$gte"] = startDate,
                    ["$lte"] = endDate
                }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                ["from"] = "pomodoros",
                ["localField"] = "_id",
                ["foreignField"] = "taskId",
                ["as"] = "pomodoros"
            }),
            new BsonDocument("$unwind", "$pomodoros"),
            new BsonDocument("$group", new BsonDocument
            {
                ["_id"] = new BsonDocument("$dateToString", new BsonDocument
                {
                    ["format"] = "%Y-%m-%d",
                    ["date"] = "$pomodoros.completedAt"
                }),
                ["count"] = new BsonDocument("$sum", 1),
                ["totalDuration"] = new BsonDocument("$sum", "$pomodoros.duration")
            }),
            new BsonDocument("$sort", new BsonDocument("_id", 1))
        };

        return await _tasks.Aggregate<PomodoroTrendData>(pipeline).ToListAsync();
    }
}
```

#### MongoDB Configuration

```csharp
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ProjectsCollectionName { get; set; } = "projects";
    public string TasksCollectionName { get; set; } = "tasks";
    public string PomodorosCollectionName { get; set; } = "pomodoros";
}

public static class MongoDbServiceExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
        
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        
        services.AddSingleton<IMongoClient>(provider =>
        {
            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped(provider =>
        {
            var client = provider.GetService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        // Register repositories
        services.AddScoped<IMongoRepository<Project, string>, MongoRepository<Project, string>>();
        services.AddScoped<IMongoRepository<TaskItem, string>, MongoRepository<TaskItem, string>>();
        services.AddScoped<ProjectAnalyticsRepository>();

        return services;
    }
}
```

### Context7 Usage for MongoDB

```bash
# Get MongoDB best practices
Context7: MongoDB.Driver CRUD operations
Context7: MongoDB aggregation pipeline patterns
Context7: MongoDB indexing and performance optimization
Context7: MongoDB document design patterns
```

---

## Redis Caching Strategies

### Distributed Caching Patterns

#### Redis Service Implementation

```csharp
public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task RemoveAsync(string key);
    Task RemovePatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
    Task<long> IncrementAsync(string key, long value = 1);
    Task<double> IncrementAsync(string key, double value);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly IServer _server;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
        _server = redis.GetServer(redis.GetEndPoints().First());
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var value = await _database.StringGetAsync(key);
        
        if (!value.HasValue)
            return null;

        return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        await _database.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RemovePatternAsync(string pattern)
    {
        var keys = _server.Keys(pattern: pattern);
        var keyArray = keys.ToArray();
        
        if (keyArray.Any())
        {
            await _database.KeyDeleteAsync(keyArray);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        return await _database.StringIncrementAsync(key, value);
    }

    public async Task<double> IncrementAsync(string key, double value)
    {
        return await _database.StringIncrementAsync(key, value);
    }
}
```

#### Caching Decorators Pattern

```csharp
public class CachedProjectService : IProjectService
{
    private readonly IProjectService _projectService;
    private readonly IRedisCacheService _cacheService;
    private readonly ILogger<CachedProjectService> _logger;
    private readonly TimeSpan _defaultCacheTime = TimeSpan.FromMinutes(15);

    public CachedProjectService(
        IProjectService projectService,
        IRedisCacheService cacheService,
        ILogger<CachedProjectService> logger)
    {
        _projectService = projectService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        const string cacheKey = "projects:all";
        
        var cachedProjects = await _cacheService.GetAsync<IEnumerable<Project>>(cacheKey);
        if (cachedProjects != null)
        {
            _logger.LogInformation("Projects retrieved from cache");
            return cachedProjects;
        }

        var projects = await _projectService.GetProjectsAsync();
        await _cacheService.SetAsync(cacheKey, projects, _defaultCacheTime);
        
        _logger.LogInformation("Projects cached for {Duration} minutes", _defaultCacheTime.TotalMinutes);
        return projects;
    }

    public async Task<Project?> GetProjectByIdAsync(string id)
    {
        var cacheKey = $"project:{id}";
        
        var cachedProject = await _cacheService.GetAsync<Project>(cacheKey);
        if (cachedProject != null)
        {
            return cachedProject;
        }

        var project = await _projectService.GetProjectByIdAsync(id);
        if (project != null)
        {
            await _cacheService.SetAsync(cacheKey, project, _defaultCacheTime);
        }

        return project;
    }

    public async Task<Project> CreateProjectAsync(CreateProjectRequest request)
    {
        var project = await _projectService.CreateProjectAsync(request);
        
        // Invalidate list cache
        await _cacheService.RemoveAsync("projects:all");
        
        // Cache the new project
        var cacheKey = $"project:{project.Id}";
        await _cacheService.SetAsync(cacheKey, project, _defaultCacheTime);
        
        return project;
    }

    public async Task<Project> UpdateProjectAsync(string id, UpdateProjectRequest request)
    {
        var project = await _projectService.UpdateProjectAsync(id, request);
        
        // Update cache
        var cacheKey = $"project:{id}";
        await _cacheService.SetAsync(cacheKey, project, _defaultCacheTime);
        
        // Invalidate list cache
        await _cacheService.RemoveAsync("projects:all");
        
        return project;
    }

    public async Task DeleteProjectAsync(string id)
    {
        await _projectService.DeleteProjectAsync(id);
        
        // Remove from cache
        var cacheKey = $"project:{id}";
        await _cacheService.RemoveAsync(cacheKey);
        
        // Invalidate list cache
        await _cacheService.RemoveAsync("projects:all");
    }
}
```

#### Session Management with Redis

```csharp
public class RedisSessionService
{
    private readonly IRedisCacheService _cacheService;
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(2);

    public RedisSessionService(IRedisCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<UserSession?> GetSessionAsync(string sessionId)
    {
        var cacheKey = $"session:{sessionId}";
        var session = await _cacheService.GetAsync<UserSession>(cacheKey);
        
        if (session != null)
        {
            // Extend session timeout on access
            await _cacheService.SetAsync(cacheKey, session, _sessionTimeout);
        }
        
        return session;
    }

    public async Task SetSessionAsync(string sessionId, UserSession session)
    {
        var cacheKey = $"session:{sessionId}";
        await _cacheService.SetAsync(cacheKey, session, _sessionTimeout);
    }

    public async Task RemoveSessionAsync(string sessionId)
    {
        var cacheKey = $"session:{sessionId}";
        await _cacheService.RemoveAsync(cacheKey);
    }

    public async Task<long> GetActiveUserCountAsync()
    {
        // Use Redis sets to track active users
        return await _cacheService.IncrementAsync("stats:active_users", 0);
    }
}
```

#### Redis Configuration

```csharp
public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "TaskinApp";
        });

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            return ConnectionMultiplexer.Connect(connectionString);
        });

        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<RedisSessionService>();

        // Decorate services with caching
        services.Decorate<IProjectService, CachedProjectService>();

        return services;
    }
}
```

### Context7 Usage for Redis

```bash
# Get Redis best practices
Context7: StackExchange.Redis configuration and connection management
Context7: Redis caching patterns and strategies
Context7: Redis pub/sub messaging patterns
Context7: Redis performance optimization and monitoring
```

---

# Infrastructure & Future Technologies

## Containerization with Docker

### Docker Configuration Patterns

#### Multi-Stage Dockerfile for .NET API

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["ElGuerre.Taskin.Api/ElGuerre.Taskin.Api.csproj", "ElGuerre.Taskin.Api/"]
COPY ["ElGuerre.Taskin.Application/ElGuerre.Taskin.Application.csproj", "ElGuerre.Taskin.Application/"]
COPY ["ElGuerre.Taskin.Domain/ElGuerre.Taskin.Domain.csproj", "ElGuerre.Taskin.Domain/"]
COPY ["ElGuerre.Taskin.Infrastructure/ElGuerre.Taskin.Infrastructure.csproj", "ElGuerre.Taskin.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "ElGuerre.Taskin.Api/ElGuerre.Taskin.Api.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/ElGuerre.Taskin.Api"
RUN dotnet build "ElGuerre.Taskin.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ElGuerre.Taskin.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 dotnet
RUN adduser --system --uid 1001 --gid 1001 dotnet

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R dotnet:dotnet /app
USER dotnet

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ElGuerre.Taskin.Api.dll"]
```

#### Multi-Stage Dockerfile for Angular

```dockerfile
# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm ci --only=production

# Copy source and build
COPY . .
RUN npm run build

# Production stage
FROM nginx:alpine AS production

# Copy custom nginx config
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built application
COPY --from=build /app/dist/taskin-angular /usr/share/nginx/html

# Create non-root user
RUN addgroup -g 1001 -S nodejs
RUN adduser -S nextjs -u 1001
RUN chown -R nextjs:nodejs /usr/share/nginx/html
RUN chown -R nextjs:nodejs /var/cache/nginx
RUN chown -R nextjs:nodejs /var/log/nginx
RUN chown -R nextjs:nodejs /etc/nginx/conf.d
RUN touch /var/run/nginx.pid
RUN chown -R nextjs:nodejs /var/run/nginx.pid

USER nextjs

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

#### Docker Compose for Development

```yaml
version: '3.8'

services:
  # SQL Server Database
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - taskin-network

  # Redis Cache
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    command: redis-server --appendonly yes
    networks:
      - taskin-network

  # MongoDB (for future analytics)
  mongodb:
    image: mongo:7
    ports:
      - "27017:27017"
    environment:
      - MONGO_INITDB_ROOT_USERNAME=admin
      - MONGO_INITDB_ROOT_PASSWORD=password123
    volumes:
      - mongodb_data:/data/db
    networks:
      - taskin-network

  # .NET API
  api:
    build:
      context: ./back/src/Taskin.Api
      dockerfile: Dockerfile
    ports:
      - "5001:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=TaskinDB;User Id=sa;Password=YourStrong@Passw0rd;Encrypt=false;TrustServerCertificate=true;
      - ConnectionStrings__Redis=redis:6379
      - ConnectionStrings__MongoDB=mongodb://admin:password123@mongodb:27017
    depends_on:
      - sqlserver
      - redis
      - mongodb
    networks:
      - taskin-network

  # Angular Frontend
  frontend:
    build:
      context: ./ui/src/taskin-angular
      dockerfile: Dockerfile
    ports:
      - "4200:80"
    depends_on:
      - api
    networks:
      - taskin-network

  # nginx reverse proxy
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf
      - ./nginx/ssl:/etc/ssl/certs
    depends_on:
      - frontend
      - api
    networks:
      - taskin-network

volumes:
  sqlserver_data:
  redis_data:
  mongodb_data:

networks:
  taskin-network:
    driver: bridge
```

### Context7 Usage for Docker

```bash
# Get Docker best practices
Context7: Docker multi-stage builds optimization
Context7: Docker security best practices
Context7: Docker compose orchestration patterns
Context7: Dockerfile optimization techniques
```

## Cloud Deployment Strategies

### Azure Deployment Patterns

#### Azure Container Apps Configuration

```yaml
# azure-container-app.yml
apiVersion: containerapp/v1alpha1
kind: ContainerApp
metadata:
  name: taskin-api
spec:
  template:
    containers:
    - image: taskinregistry.azurecr.io/taskin-api:latest
      name: taskin-api
      env:
      - name: ASPNETCORE_ENVIRONMENT
        value: "Production"
      - name: ConnectionStrings__DefaultConnection
        secretRef: sql-connection-string
      - name: ConnectionStrings__Redis
        secretRef: redis-connection-string
      resources:
        cpu: 1.0
        memory: 2Gi
    scale:
      minReplicas: 2
      maxReplicas: 10
      rules:
      - name: http-rule
        http:
          metadata:
            concurrentRequests: 50
  configuration:
    ingress:
      external: true
      targetPort: 8080
      traffic:
      - weight: 100
        latestRevision: true
```

#### Infrastructure as Code with Bicep

```bicep
// main.bicep
@description('The name of the application')
param applicationName string = 'taskin'

@description('The environment name')
param environment string = 'dev'

@description('The location for all resources')
param location string = resourceGroup().location

// Variables
var commonTags = {
  application: applicationName
  environment: environment
}

var resourceNames = {
  containerRegistry: '${applicationName}${environment}registry'
  containerApp: '${applicationName}-${environment}-app'
  sqlServer: '${applicationName}-${environment}-sql'
  redisCache: '${applicationName}-${environment}-redis'
  logAnalytics: '${applicationName}-${environment}-logs'
}

// Log Analytics Workspace
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: resourceNames.logAnalytics
  location: location
  tags: commonTags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Container Registry
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' = {
  name: resourceNames.containerRegistry
  location: location
  tags: commonTags
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

// SQL Server and Database
resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: resourceNames.sqlServer
  location: location
  tags: commonTags
  properties: {
    administratorLogin: 'sqladmin'
    administratorLoginPassword: 'P@ssw0rd123!'
    version: '12.0'
    publicNetworkAccess: 'Enabled'
  }

  resource database 'databases' = {
    name: '${applicationName}DB'
    location: location
    tags: commonTags
    sku: {
      name: 'S0'
      tier: 'Standard'
    }
    properties: {
      collation: 'SQL_Latin1_General_CP1_CI_AS'
    }
  }

  resource firewallRule 'firewallRules' = {
    name: 'AllowAzureServices'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
}

// Redis Cache
resource redisCache 'Microsoft.Cache/Redis@2023-08-01' = {
  name: resourceNames.redisCache
  location: location
  tags: commonTags
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
  }
}

// Container App Environment
resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: '${applicationName}-${environment}-env'
  location: location
  tags: commonTags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

// Container App
resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: resourceNames.containerApp
  location: location
  tags: commonTags
  properties: {
    managedEnvironmentId: containerAppEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        allowInsecure: false
        traffic: [
          {
            weight: 100
            latestRevision: true
          }
        ]
      }
      secrets: [
        {
          name: 'sql-connection-string'
          value: 'Server=${sqlServer.properties.fullyQualifiedDomainName};Database=${sqlServer::database.name};User Id=sqladmin;Password=P@ssw0rd123!;Encrypt=true;TrustServerCertificate=false;'
        }
        {
          name: 'redis-connection-string'
          value: '${redisCache.properties.hostName}:${redisCache.properties.sslPort},password=${redisCache.listKeys().primaryKey},ssl=True,abortConnect=False'
        }
      ]
    }
    template: {
      containers: [
        {
          image: '${containerRegistry.properties.loginServer}/taskin-api:latest'
          name: 'taskin-api'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'ConnectionStrings__DefaultConnection'
              secretRef: 'sql-connection-string'
            }
            {
              name: 'ConnectionStrings__Redis'
              secretRef: 'redis-connection-string'
            }
          ]
          resources: {
            cpu: json('1.0')
            memory: '2Gi'
          }
        }
      ]
      scale: {
        minReplicas: 2
        maxReplicas: 10
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '50'
              }
            }
          }
        ]
      }
    }
  }
}

// Outputs
output containerRegistryLoginServer string = containerRegistry.properties.loginServer
output containerAppUrl string = containerApp.properties.configuration.ingress.fqdn
output sqlServerName string = sqlServer.properties.fullyQualifiedDomainName
output redisCacheName string = redisCache.properties.hostName
```

### Context7 Usage for Cloud Deployment

```bash
# Get Azure deployment best practices
Context7: Azure Container Apps deployment patterns
Context7: Azure Bicep infrastructure as code
Context7: Azure DevOps CI/CD pipelines
Context7: Azure monitoring and logging strategies
```

## CI/CD Pipeline Strategies

### GitHub Actions Workflow

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  REGISTRY: taskinregistry.azurecr.io
  API_IMAGE_NAME: taskin-api
  FRONTEND_IMAGE_NAME: taskin-frontend

jobs:
  # Backend Tests and Build
  backend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: ./back/src/Taskin.Api

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Test
      run: dotnet test --no-build --configuration Release --verbosity normal --collect:"XPlat Code Coverage"

    - name: Upload coverage reports
      uses: codecov/codecov-action@v4
      with:
        file: ./coverage.cobertura.xml
        flags: backend

    - name: Build Docker image
      if: github.ref == 'refs/heads/main'
      run: docker build -t ${{ env.REGISTRY }}/${{ env.API_IMAGE_NAME }}:${{ github.sha }} -f Dockerfile .

    - name: Log in to Azure Container Registry
      if: github.ref == 'refs/heads/main'
      uses: azure/docker-login@v1
      with:
        login-server: ${{ env.REGISTRY }}
        username: ${{ secrets.ACR_USERNAME }}
        password: ${{ secrets.ACR_PASSWORD }}

    - name: Push Docker image
      if: github.ref == 'refs/heads/main'
      run: docker push ${{ env.REGISTRY }}/${{ env.API_IMAGE_NAME }}:${{ github.sha }}

  # Frontend Tests and Build
  frontend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: ./ui/src/taskin-angular

    steps:
    - uses: actions/checkout@v4

    - name: Setup Node.js
      uses: actions/setup-node@v4
      with:
        node-version: '20'
        cache: 'npm'
        cache-dependency-path: './ui/src/taskin-angular/package-lock.json'

    - name: Install dependencies
      run: npm ci

    - name: Lint
      run: npm run lint

    - name: Test
      run: npm run test:ci

    - name: Build
      run: npm run build

    - name: Upload coverage reports
      uses: codecov/codecov-action@v4
      with:
        file: ./coverage/lcov.info
        flags: frontend

    - name: Build Docker image
      if: github.ref == 'refs/heads/main'
      run: docker build -t ${{ env.REGISTRY }}/${{ env.FRONTEND_IMAGE_NAME }}:${{ github.sha }} -f Dockerfile .

    - name: Push Docker image
      if: github.ref == 'refs/heads/main'
      run: docker push ${{ env.REGISTRY }}/${{ env.FRONTEND_IMAGE_NAME }}:${{ github.sha }}

  # Security Scanning
  security:
    runs-on: ubuntu-latest
    needs: [backend, frontend]
    if: github.ref == 'refs/heads/main'

    steps:
    - uses: actions/checkout@v4

    - name: Run Trivy vulnerability scanner
      uses: aquasecurity/trivy-action@master
      with:
        image-ref: '${{ env.REGISTRY }}/${{ env.API_IMAGE_NAME }}:${{ github.sha }}'
        format: 'sarif'
        output: 'trivy-results.sarif'

    - name: Upload Trivy scan results to GitHub Security
      uses: github/codeql-action/upload-sarif@v3
      with:
        sarif_file: 'trivy-results.sarif'

  # Deploy to Azure
  deploy:
    runs-on: ubuntu-latest
    needs: [backend, frontend, security]
    if: github.ref == 'refs/heads/main'
    environment: production

    steps:
    - uses: actions/checkout@v4

    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Deploy to Azure Container Apps
      uses: azure/container-apps-deploy-action@v1
      with:
        containerAppName: taskin-prod-app
        resourceGroup: taskin-prod-rg
        imageToDeploy: ${{ env.REGISTRY }}/${{ env.API_IMAGE_NAME }}:${{ github.sha }}
        environmentVariables: |
          ASPNETCORE_ENVIRONMENT=Production
          ConnectionStrings__DefaultConnection=secretref:sql-connection-string
          ConnectionStrings__Redis=secretref:redis-connection-string

    - name: Update frontend deployment
      run: |
        az containerapp update \
          --name taskin-frontend-prod \
          --resource-group taskin-prod-rg \
          --image ${{ env.REGISTRY }}/${{ env.FRONTEND_IMAGE_NAME }}:${{ github.sha }}
```

### Context7 Usage for CI/CD

```bash
# Get CI/CD best practices
Context7: GitHub Actions workflow optimization
Context7: Azure DevOps pipeline patterns
Context7: Docker security scanning in CI/CD
Context7: Automated testing strategies in pipelines
```

## Monitoring & Observability

### Application Insights Configuration

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);

// Add custom telemetry
builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddApplicationInsights();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// Custom telemetry initializer
public class CustomTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.GlobalProperties["Application"] = "Taskin.Api";
        telemetry.Context.GlobalProperties["Version"] = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    }
}
```

### Health Checks Implementation

```csharp
// Health checks configuration
builder.Services.AddHealthChecks()
    .AddDbContextCheck<TaskinDbContext>("database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, "redis")
    .AddCheck<ApiHealthCheck>("api", HealthStatus.Degraded, tags: new[] { "ready" });

// Custom health check
public class ApiHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;

    public ApiHealthCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskinDbContext>();
            
            await dbContext.Projects.CountAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("API is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("API health check failed", ex);
        }
    }
}

// Configure health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

### Context7 Usage for Monitoring

```bash
# Get monitoring best practices
Context7: Azure Application Insights configuration
Context7: .NET health checks implementation
Context7: Azure Monitor alerting strategies
Context7: Application performance monitoring patterns
```

## Future Technology Integration Points

### Message Queue Integration (Future)

```csharp
// RabbitMQ/Azure Service Bus integration placeholder
public interface IMessageQueueService
{
    Task PublishAsync<T>(T message, string queue) where T : class;
    Task SubscribeAsync<T>(string queue, Func<T, Task> handler) where T : class;
}

// Event-driven architecture preparation
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    public string EventId { get; protected set; } = Guid.NewGuid().ToString();
}

public class ProjectCreatedEvent : DomainEvent
{
    public string ProjectId { get; }
    public string ProjectName { get; }
    
    public ProjectCreatedEvent(string projectId, string projectName)
    {
        ProjectId = projectId;
        ProjectName = projectName;
    }
}
```

### Context7 Usage for Future Technologies

```bash
# Get patterns for future integrations
Context7: RabbitMQ .NET integration patterns
Context7: Azure Service Bus messaging patterns
Context7: Microservices communication strategies
Context7: Event-driven architecture patterns
Context7: Kubernetes deployment strategies
Context7: gRPC service implementation
```

## Development Workflow Integration

### Context7 in Development Process

1. **Before starting new features**: Use Context7 to get latest patterns and best practices
2. **When encountering issues**: Use Context7 for troubleshooting guides and solutions
3. **During code reviews**: Reference Context7 for architectural guidance
4. **When updating dependencies**: Check Context7 for migration guides and breaking changes
5. **For performance optimization**: Use Context7 for latest optimization techniques
- Add to memory. The html templante must be always located in is own file not embebed.