# ANGULAR_CLAUDE_GUIDE.md

This file provides comprehensive guidance to Claude Code when working with Angular 18+ applications. Use this as a template for your project's CLAUDE.md file.

## Project Overview Template

This is an Angular 18+ application using modern patterns including:
- **Standalone Components** with signal-based architecture
- **NgRx Signal Stores** for state management
- **Angular Material** with custom theming
- **Tailwind CSS** for utility-first styling
- **Transloco** for internationalization
- **GraphQL/Apollo** for API communication (optional)

## Development Commands

### Essential Commands
- `npm install` - Install dependencies
- `npm run start` - Development server (typically http://localhost:4200)
- `npm run build` - Production build
- `npm run build:dev` - Development build
- `npm run test` - Run unit tests with Karma
- `npm run lint` - Run ESLint linting
- `npm run e2e` - Run end-to-end tests (if configured)

### Testing & Quality
- `npm run test:coverage` - Run tests with coverage report
- `npm run lint:fix` - Fix linting issues automatically

### Internationalization (if using Transloco)
- `npm run i18n:extract` - Extract translation keys
- `npm run i18n:find` - Find unused translation keys

## Architecture Guidelines

### Module Structure
The application follows a feature-module architecture:
```
src/app/
├── core/                 # Core services, guards, interceptors
│   ├── auth/            # Authentication services and guards
│   ├── guards/          # Route guards
│   └── services/        # Singleton services
├── shared/              # Shared components, pipes, directives
│   ├── components/      # Reusable UI components
│   ├── pipes/          # Custom pipes
│   └── services/       # Shared services
├── features/           # Feature modules
│   └── [feature-name]/
│       ├── pages/      # Route components
│       ├── components/ # Feature-specific components
│       └── shared/     # Feature services, stores, types
└── layout/             # Application layout components
```

### Component Architecture
- **Pages** - Route-level components (e.g., `features/users/pages/user-list`)
- **Components** - Reusable UI components (e.g., `features/users/components/user-card`)
- **Shared** - Services, stores, types (e.g., `features/users/shared/user.service.ts`)

## Code Conventions

### Component Generation
Always create standalone components with these flags:
```bash
ng generate component features/[feature]/pages/[component-name] --standalone --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
```

### Component Structure Template
```typescript
import { CommonModule } from '@angular/common'
import { ChangeDetectionStrategy, Component, ViewEncapsulation, inject } from '@angular/core'

@Component({
  selector: 'app-component-name',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './component-name.component.html',
  styles: [],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ComponentNameComponent {
  // Use inject() for dependency injection
  private readonly _service = inject(ServiceName)
  
  // Prefer signals for reactive state
  data = signal<DataType[]>([])
  loading = signal<boolean>(false)
}
```

### File Organization Rules
- Use **kebab-case** for file and folder names
- Use **PascalCase** for class names
- Use **camelCase** for variables and methods
- Follow the pattern: `features/[feature-name]/pages|components/[component-name]/`
- Store logic in dedicated `.store.ts` files using NgRx Component Store
- Service interfaces should be prefixed with `I` (e.g., `IUserService`)

### ESLint Configuration
```json
{
  "rules": {
    "@angular-eslint/component-selector": ["error", {
      "type": "element",
      "prefix": "app",
      "style": "kebab-case"
    }],
    "@typescript-eslint/explicit-function-return-type": "error",
    "max-len": ["error", { "code": 120 }],
    "quotes": ["error", "single"],
    "semi": ["error", "never"]
  }
}
```

## State Management

### NgRx Signal Store Pattern
```typescript
import { Injectable } from '@angular/core'
import { patchState, signalStore, withState } from '@ngrx/signals'
import { rxMethod } from '@ngrx/signals/rxjs-interop'

type FeatureState = {
  loading: boolean
  data: DataType[]
  selectedItem: DataType | null
}

const initialState: FeatureState = {
  loading: false,
  data: [],
  selectedItem: null,
}

@Injectable()
export class FeatureStore extends signalStore(withState(initialState)) {
  
  readonly loadData = rxMethod<void>(
    pipe(
      switchMap(() => {
        patchState(this, { loading: true })
        return this._service.getData().pipe(
          tapResponse({
            next: (data) => patchState(this, { data, loading: false }),
            error: () => patchState(this, { loading: false }),
          })
        )
      })
    )
  )
}
```

### Service Interface Pattern
```typescript
export interface IDataService {
  getData(): Observable<DataType[]>
  getById(id: string): Observable<DataType>
  create(item: CreateDataType): Observable<DataType>
  update(id: string, item: UpdateDataType): Observable<DataType>
  delete(id: string): Observable<void>
}

@Injectable()
export class DataService implements IDataService {
  private readonly _http = inject(HttpClient)
  
  getData(): Observable<DataType[]> {
    return this._http.get<DataType[]>('/api/data')
  }
}
```

## Form Patterns

### Reactive Forms with Validation
```typescript
export class FormComponent {
  private readonly _fb = inject(FormBuilder)
  
  form = this._fb.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    active: [true]
  })
  
  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value as FormDataType
      // Handle form submission
    }
  }
}
```

## Authentication & Permissions

### Route Guards
```typescript
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService)
  return authService.isAuthenticated()
}

export const permissionGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService)
  const requiredPermission = route.data['permission']
  return authService.hasPermission(requiredPermission)
}
```

### Permission-based UI
```typescript
export class ComponentWithPermissions {
  private readonly _authService = inject(AuthService)
  
  canEdit = computed(() => 
    this._authService.hasPermission('resource:edit')
  )
  
  canDelete = computed(() => 
    this._authService.hasPermission('resource:delete')
  )
}
```

## Routing Configuration

### Feature Routes
```typescript
export const routes: Routes = [
  {
    path: '',
    component: FeatureListComponent,
    title: 'Feature List'
  },
  {
    path: 'new',
    component: FeatureCreateComponent,
    title: 'Create Feature',
    canActivate: [authGuard],
    data: { permission: 'feature:create' }
  },
  {
    path: ':id',
    component: FeatureDetailComponent,
    title: 'Feature Details',
    canActivate: [authGuard],
    canDeactivate: [pendingChangesGuard]
  }
]
```

## Best Practices

### Performance
- Use `OnPush` change detection strategy
- Implement `trackBy` functions for `*ngFor`
- Use `async` pipe for observables
- Prefer signals over observables for component state

### Security
- Never expose sensitive data in client code
- Use proper authentication guards
- Validate permissions on both client and server
- Sanitize user inputs

### Accessibility
- Use semantic HTML elements
- Include proper ARIA attributes
- Ensure keyboard navigation
- Maintain proper contrast ratios

### Testing
- Write unit tests for components, services, and stores
- Use Angular Testing Library for component testing
- Mock external dependencies
- Test user interactions and edge cases

## Error Handling

### Global Error Handler
```typescript
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  handleError(error: Error): void {
    console.error('Global error:', error)
    // Send to logging service
  }
}
```

### HTTP Error Interceptor
```typescript
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle HTTP errors
        return throwError(() => error)
      })
    )
  }
}
```

## Important Reminders

- **Always use standalone components** with proper imports
- **Prefer signals** over observables for component state
- **Use inject()** instead of constructor injection
- **Follow the established folder structure** for consistency
- **Write explicit return types** for all methods
- **Use proper TypeScript types** instead of `any`
- **Never commit sensitive data** or API keys
- **Test components thoroughly** before submitting changes
- **Follow the single responsibility principle** for components and services