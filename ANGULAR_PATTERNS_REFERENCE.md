# ANGULAR PATTERNS REFERENCE

Comprehensive guide to Angular architecture patterns, best practices, and design principles for scalable applications.

## Table of Contents
- [Architecture Overview](#architecture-overview)
- [Component Patterns](#component-patterns)
- [State Management Patterns](#state-management-patterns)
- [Service Patterns](#service-patterns)
- [Form Patterns](#form-patterns)
- [Authentication Patterns](#authentication-patterns)
- [Error Handling Patterns](#error-handling-patterns)
- [Performance Patterns](#performance-patterns)
- [Testing Patterns](#testing-patterns)
- [Code Organization](#code-organization)

---

## Architecture Overview

### 1. Feature-Based Architecture

Organize your application by features rather than by file types:

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
│   ├── users/
│   └── settings/
└── layout/              # Layout components
    ├── header/
    ├── sidebar/
    └── footer/
```

### 2. Module Structure Pattern

Each feature follows a consistent structure:

```
features/users/
├── pages/              # Route components
│   ├── user-list/
│   ├── user-details/
│   └── user-form/
├── components/         # Feature-specific components
│   ├── user-card/
│   └── user-avatar/
├── shared/            # Feature services and types
│   ├── services/
│   ├── stores/
│   ├── types/
│   └── guards/
└── users.routes.ts    # Feature routing
```

### 3. Dependency Injection Patterns

**Service Hierarchy:**
```typescript
// Root level services (app-wide singletons)
@Injectable({ providedIn: 'root' })
export class GlobalService {}

// Feature level services
@Injectable()
export class FeatureService {}

// Component level services
@Component({
  providers: [ComponentService]
})
export class MyComponent {}
```

**Injection Token Pattern:**
```typescript
// Define injection token
export const API_CONFIG = new InjectionToken<ApiConfig>('api.config')

// Provide in main.ts
bootstrapApplication(AppComponent, {
  providers: [
    { provide: API_CONFIG, useValue: { baseUrl: '/api' } }
  ]
})

// Inject in service
constructor(@Inject(API_CONFIG) private config: ApiConfig) {}
```

---

## Component Patterns

### 1. Smart vs Dumb Components Pattern

**Smart Components (Containers):**
```typescript
@Component({
  selector: 'app-user-list-container',
  template: `
    <app-user-list
      [users]="users()"
      [loading]="loading()"
      (userSelected)="onUserSelected($event)"
      (userDeleted)="onUserDeleted($event)">
    </app-user-list>
  `,
  providers: [UserStore]
})
export class UserListContainerComponent {
  private store = inject(UserStore)
  
  users = this.store.selectSignal(state => state.users)
  loading = this.store.selectSignal(state => state.loading)

  onUserSelected(user: User): void {
    this.store.selectUser(user)
  }

  onUserDeleted(userId: string): void {
    this.store.deleteUser(userId)
  }
}
```

**Dumb Components (Presentational):**
```typescript
@Component({
  selector: 'app-user-list',
  template: `
    <div class="user-list">
      @if (loading()) {
        <app-loading-spinner></app-loading-spinner>
      } @else {
        @for (user of users(); track user.id) {
          <app-user-card
            [user]="user"
            (selected)="userSelected.emit(user)"
            (deleted)="userDeleted.emit(user.id)">
          </app-user-card>
        }
      }
    </div>
  `
})
export class UserListComponent {
  // Inputs
  users = input<User[]>([])
  loading = input<boolean>(false)

  // Outputs
  userSelected = output<User>()
  userDeleted = output<string>()
}
```

### 2. Component Composition Pattern

**Base Component:**
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
```

**Usage:**
```html
<app-base-list>
  <div slot="header">
    <h2>Users</h2>
    <button (click)="addUser()">Add User</button>
  </div>
  <div slot="content">
    <app-user-table [users]="users()"></app-user-table>
  </div>
  <div slot="footer">
    <app-pagination [currentPage]="page()" [total]="total()"></app-pagination>
  </div>
</app-base-list>
```

### 3. Signal-Based Component Pattern

```typescript
@Component({
  selector: 'app-user-profile',
  template: `
    <div class="profile">
      <h2>{{ fullName() }}</h2>
      <p>{{ user().email }}</p>
      <div class="stats">
        <span>Posts: {{ postCount() }}</span>
        <span>Followers: {{ followerCount() }}</span>
      </div>
    </div>
  `
})
export class UserProfileComponent {
  private userService = inject(UserService)
  
  // Input signals
  userId = input.required<string>()
  
  // Computed signals
  user = computed(() => this.userService.getUser(this.userId()))
  fullName = computed(() => {
    const user = this.user()
    return user ? `${user.firstName} ${user.lastName}` : ''
  })
  
  // Derived data
  postCount = computed(() => this.user()?.stats?.posts ?? 0)
  followerCount = computed(() => this.user()?.stats?.followers ?? 0)
}
```

---

## State Management Patterns

### 1. NgRx Signal Store Pattern

**Feature Store:**
```typescript
type UserState = {
  users: User[]
  selectedUser: User | null
  loading: boolean
  error: string | null
  filters: UserFilters
  pagination: PaginationState
}

@Injectable()
export class UserStore extends signalStore(
  { providedIn: 'root' },
  withState<UserState>(initialState),
  withComputed((state) => ({
    filteredUsers: computed(() => 
      applyFilters(state.users(), state.filters())
    ),
    hasUsers: computed(() => state.users().length > 0),
    isLoading: computed(() => state.loading()),
  })),
  withMethods((store, userService = inject(UserService)) => ({
    loadUsers: rxMethod<UserFilters>(
      pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((filters) => {
          patchState(store, { loading: true, filters })
          return userService.getUsers(filters).pipe(
            tapResponse({
              next: (users) => patchState(store, { users, loading: false }),
              error: (error) => patchState(store, { error: error.message, loading: false })
            })
          )
        })
      )
    ),
    
    selectUser: (user: User) => patchState(store, { selectedUser: user }),
    
    updateUser: rxMethod<User>(
      pipe(
        exhaustMap((user) => 
          userService.updateUser(user).pipe(
            tapResponse({
              next: (updatedUser) => patchState(store, {
                users: store.users().map(u => u.id === user.id ? updatedUser : u),
                selectedUser: updatedUser
              }),
              error: (error) => patchState(store, { error: error.message })
            })
          )
        )
      )
    )
  }))
) {}
```

### 2. Local Component State Pattern

**Simple Component Store:**
```typescript
@Component({
  selector: 'app-search',
  template: `
    <input 
      [value]="searchTerm()"
      (input)="onSearchInput($event)"
      placeholder="Search...">
    
    @if (isSearching()) {
      <mat-spinner diameter="20"></mat-spinner>
    }
    
    @if (results().length > 0) {
      <div class="results">
        @for (result of results(); track result.id) {
          <div class="result-item">{{ result.name }}</div>
        }
      </div>
    }
  `
})
export class SearchComponent {
  private searchService = inject(SearchService)
  
  // Local state signals
  searchTerm = signal('')
  isSearching = signal(false)
  results = signal<SearchResult[]>([])
  
  constructor() {
    // Auto-search effect
    effect(() => {
      const term = this.searchTerm()
      if (term.length >= 3) {
        this.performSearch(term)
      } else {
        this.results.set([])
      }
    })
  }
  
  onSearchInput(event: Event): void {
    const target = event.target as HTMLInputElement
    this.searchTerm.set(target.value)
  }
  
  private performSearch(term: string): void {
    this.isSearching.set(true)
    this.searchService.search(term)
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: (results) => {
          this.results.set(results)
          this.isSearching.set(false)
        },
        error: () => {
          this.results.set([])
          this.isSearching.set(false)
        }
      })
  }
}
```

### 3. State Synchronization Pattern

**Cross-Component Communication:**
```typescript
// Shared state service
@Injectable({ providedIn: 'root' })
export class AppStateService {
  private _selectedUser = signal<User | null>(null)
  private _theme = signal<Theme>('light')
  
  readonly selectedUser = this._selectedUser.asReadonly()
  readonly theme = this._theme.asReadonly()
  
  setSelectedUser(user: User | null): void {
    this._selectedUser.set(user)
  }
  
  setTheme(theme: Theme): void {
    this._theme.set(theme)
    document.body.className = `theme-${theme}`
  }
}

// Component usage
@Component({
  selector: 'app-user-actions',
  template: `
    @if (selectedUser(); as user) {
      <button (click)="editUser(user)">Edit</button>
      <button (click)="deleteUser(user)">Delete</button>
    }
  `
})
export class UserActionsComponent {
  private appState = inject(AppStateService)
  
  selectedUser = this.appState.selectedUser
  
  editUser(user: User): void {
    // Handle edit
  }
  
  deleteUser(user: User): void {
    // Handle delete
  }
}
```

---

## Service Patterns

### 1. Repository Pattern

**Abstract Repository:**
```typescript
export abstract class Repository<T, TCreate = T, TUpdate = T> {
  abstract getAll(): Observable<T[]>
  abstract getById(id: string): Observable<T>
  abstract create(item: TCreate): Observable<T>
  abstract update(id: string, item: TUpdate): Observable<T>
  abstract delete(id: string): Observable<void>
}
```

**HTTP Repository Implementation:**
```typescript
@Injectable()
export class HttpUserRepository extends Repository<User, CreateUserRequest, UpdateUserRequest> {
  private http = inject(HttpClient)
  private baseUrl = '/api/users'
  
  getAll(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl)
  }
  
  getById(id: string): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/${id}`)
  }
  
  create(request: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.baseUrl, request)
  }
  
  update(id: string, request: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${this.baseUrl}/${id}`, request)
  }
  
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
  }
}
```

### 2. Facade Pattern

**Service Facade:**
```typescript
@Injectable({ providedIn: 'root' })
export class UserFacade {
  private userRepository = inject(HttpUserRepository)
  private userStore = inject(UserStore)
  private notificationService = inject(NotificationService)
  
  // Expose store selectors
  users$ = this.userStore.users$
  selectedUser$ = this.userStore.selectedUser$
  loading$ = this.userStore.loading$
  
  async loadUsers(): Promise<void> {
    try {
      this.userStore.setLoading(true)
      const users = await firstValueFrom(this.userRepository.getAll())
      this.userStore.setUsers(users)
    } catch (error) {
      this.notificationService.showError('Failed to load users')
    } finally {
      this.userStore.setLoading(false)
    }
  }
  
  async createUser(request: CreateUserRequest): Promise<void> {
    try {
      const user = await firstValueFrom(this.userRepository.create(request))
      this.userStore.addUser(user)
      this.notificationService.showSuccess('User created successfully')
    } catch (error) {
      this.notificationService.showError('Failed to create user')
      throw error
    }
  }
}
```

### 3. Strategy Pattern

**Validation Strategy:**
```typescript
interface ValidationStrategy {
  validate(value: any): ValidationResult
}

class EmailValidationStrategy implements ValidationStrategy {
  validate(value: string): ValidationResult {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    return {
      isValid: emailRegex.test(value),
      message: emailRegex.test(value) ? '' : 'Invalid email format'
    }
  }
}

class RequiredValidationStrategy implements ValidationStrategy {
  validate(value: any): ValidationResult {
    const isValid = value !== null && value !== undefined && value !== ''
    return {
      isValid,
      message: isValid ? '' : 'This field is required'
    }
  }
}

@Injectable()
export class ValidationService {
  private strategies = new Map<string, ValidationStrategy>([
    ['email', new EmailValidationStrategy()],
    ['required', new RequiredValidationStrategy()]
  ])
  
  validate(value: any, strategyName: string): ValidationResult {
    const strategy = this.strategies.get(strategyName)
    if (!strategy) {
      throw new Error(`Validation strategy '${strategyName}' not found`)
    }
    return strategy.validate(value)
  }
}
```

---

## Form Patterns

### 1. Reactive Form Builder Pattern

**Form Configuration:**
```typescript
interface FormFieldConfig {
  key: string
  label: string
  type: 'text' | 'email' | 'password' | 'select' | 'textarea'
  validators?: ValidatorFn[]
  options?: { value: any; label: string }[]
  placeholder?: string
  required?: boolean
}

@Injectable()
export class DynamicFormService {
  private fb = inject(FormBuilder)
  
  createFormGroup(config: FormFieldConfig[]): FormGroup {
    const group: Record<string, FormControl> = {}
    
    config.forEach(field => {
      const validators = field.validators || []
      if (field.required) {
        validators.push(Validators.required)
      }
      
      group[field.key] = this.fb.control('', validators)
    })
    
    return this.fb.group(group)
  }
  
  generateFormComponent(config: FormFieldConfig[]): Type<any> {
    // Dynamic component generation logic
    return class DynamicFormComponent {
      form = this.createFormGroup(config)
      config = config
      
      onSubmit(): void {
        if (this.form.valid) {
          // Handle form submission
        }
      }
    } as any
  }
}
```

### 2. Custom Form Control Pattern

**Reusable Form Control:**
```typescript
@Component({
  selector: 'app-rating-input',
  template: `
    <div class="rating-input">
      @for (star of stars; track $index) {
        <button
          type="button"
          class="star"
          [class.active]="$index < value()"
          [disabled]="disabled()"
          (click)="setValue($index + 1)">
          ★
        </button>
      }
    </div>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RatingInputComponent),
      multi: true
    }
  ]
})
export class RatingInputComponent implements ControlValueAccessor {
  stars = Array(5).fill(0)
  value = signal(0)
  disabled = signal(false)
  
  private onChange = (value: number): void => {}
  private onTouched = (): void => {}
  
  setValue(value: number): void {
    if (!this.disabled()) {
      this.value.set(value)
      this.onChange(value)
      this.onTouched()
    }
  }
  
  writeValue(value: number): void {
    this.value.set(value || 0)
  }
  
  registerOnChange(fn: (value: number) => void): void {
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

### 3. Form State Management Pattern

**Complex Form Store:**
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

---

## Authentication Patterns

### 1. JWT Token Management Pattern

**Token Service:**
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
  
  getTokenPayload<T>(token: string): T | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      return payload as T
    } catch {
      return null
    }
  }
}
```

### 2. Permission-Based Access Pattern

**Permission Service:**
```typescript
type Permission = string
type Role = {
  id: string
  name: string
  permissions: Permission[]
}

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private authService = inject(AuthService)
  
  hasPermission(permission: Permission): Observable<boolean> {
    return this.authService.currentUser$.pipe(
      map(user => user?.roles?.some(role => 
        role.permissions.includes(permission)
      ) ?? false)
    )
  }
  
  hasAnyPermission(permissions: Permission[]): Observable<boolean> {
    return this.authService.currentUser$.pipe(
      map(user => permissions.some(permission =>
        user?.roles?.some(role => 
          role.permissions.includes(permission)
        ) ?? false
      ))
    )
  }
  
  hasRole(roleName: string): Observable<boolean> {
    return this.authService.currentUser$.pipe(
      map(user => user?.roles?.some(role => 
        role.name === roleName
      ) ?? false)
    )
  }
}
```

### 3. Route Guard Pattern

**Functional Guards:**
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

---

## Error Handling Patterns

### 1. Global Error Handler Pattern

**Error Handler Service:**
```typescript
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private notificationService = inject(NotificationService)
  private loggingService = inject(LoggingService)
  
  handleError(error: Error): void {
    console.error('Global error caught:', error)
    
    // Log error to external service
    this.loggingService.logError(error)
    
    // Show user-friendly message
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

### 2. HTTP Error Interceptor Pattern

**Error Interceptor:**
```typescript
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private authService = inject(AuthService)
  private notificationService = inject(NotificationService)
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.handle401Error()
        } else if (error.status === 403) {
          this.handle403Error()
        } else if (error.status >= 500) {
          this.handle5xxError(error)
        }
        
        return throwError(() => error)
      })
    )
  }
  
  private handle401Error(): void {
    this.authService.logout()
    this.notificationService.showError('Session expired. Please log in again.')
  }
  
  private handle403Error(): void {
    this.notificationService.showError('Access denied.')
  }
  
  private handle5xxError(error: HttpErrorResponse): void {
    this.notificationService.showError('Server error. Please try again later.')
  }
}
```

### 3. Retry Pattern

**HTTP Retry Service:**
```typescript
@Injectable({ providedIn: 'root' })
export class HttpRetryService {
  
  withRetry<T>(source: Observable<T>, maxRetries = 3): Observable<T> {
    return source.pipe(
      retry({
        count: maxRetries,
        delay: (error, retryCount) => {
          // Only retry on server errors or network issues
          if (error instanceof HttpErrorResponse && error.status >= 500) {
            // Exponential backoff
            return timer(Math.pow(2, retryCount) * 1000)
          }
          // Don't retry for client errors
          return throwError(() => error)
        }
      })
    )
  }
  
  withRetryWhen<T>(
    source: Observable<T>, 
    shouldRetry: (error: any, attempt: number) => boolean,
    delay = 1000
  ): Observable<T> {
    return source.pipe(
      retryWhen(errors =>
        errors.pipe(
          scan((acc, error) => ({ count: acc.count + 1, error }), { count: 0, error: null }),
          delayWhen(({ count, error }) => {
            if (shouldRetry(error, count)) {
              return timer(delay * count)
            }
            return throwError(() => error)
          })
        )
      )
    )
  }
}
```

---

## Performance Patterns

### 1. OnPush Change Detection Pattern

**Optimized Component:**
```typescript
@Component({
  selector: 'app-user-list',
  template: `
    @for (user of users(); track user.id) {
      <app-user-item
        [user]="user"
        (userChanged)="onUserChanged($event)">
      </app-user-item>
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent {
  users = input<User[]>([])
  userChanged = output<User>()
  
  onUserChanged(user: User): void {
    this.userChanged.emit(user)
  }
}
```

### 2. Virtual Scrolling Pattern

**Virtual List Component:**
```typescript
@Component({
  selector: 'app-virtual-user-list',
  template: `
    <cdk-virtual-scroll-viewport
      itemSize="60"
      class="viewport">
      @for (user of users(); track user.id) {
        <app-user-row [user]="user"></app-user-row>
      }
    </cdk-virtual-scroll-viewport>
  `,
  styles: [`
    .viewport {
      height: 400px;
      width: 100%;
    }
  `]
})
export class VirtualUserListComponent {
  users = input<User[]>([])
}
```

### 3. Lazy Loading Pattern

**Lazy Module Loading:**
```typescript
const routes: Routes = [
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.routes').then(m => m.routes)
  },
  {
    path: 'users',
    loadChildren: () => import('./users/users.routes').then(m => m.routes)
  }
]
```

**Component Lazy Loading:**
```typescript
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

---

## Testing Patterns

### 1. Component Testing Pattern

**Component Test Setup:**
```typescript
describe('UserListComponent', () => {
  let component: UserListComponent
  let fixture: ComponentFixture<UserListComponent>
  let mockUserService: jasmine.SpyObj<UserService>

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', ['getUsers', 'deleteUser'])

    await TestBed.configureTestingModule({
      imports: [UserListComponent],
      providers: [
        { provide: UserService, useValue: spy }
      ]
    }).compileComponents()

    fixture = TestBed.createComponent(UserListComponent)
    component = fixture.componentInstance
    mockUserService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>
  })

  it('should display users', () => {
    const mockUsers: User[] = [
      { id: '1', name: 'John Doe', email: 'john@example.com' },
      { id: '2', name: 'Jane Smith', email: 'jane@example.com' }
    ]

    component.users.set(mockUsers)
    fixture.detectChanges()

    const userElements = fixture.debugElement.queryAll(By.css('.user-item'))
    expect(userElements.length).toBe(2)
  })

  it('should emit userDeleted when delete button is clicked', () => {
    spyOn(component.userDeleted, 'emit')
    const user: User = { id: '1', name: 'John Doe', email: 'john@example.com' }

    component.onDeleteUser(user)

    expect(component.userDeleted.emit).toHaveBeenCalledWith(user.id)
  })
})
```

### 2. Service Testing Pattern

**Service with HTTP Testing:**
```typescript
describe('UserService', () => {
  let service: UserService
  let httpMock: HttpTestingController

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    })

    service = TestBed.inject(UserService)
    httpMock = TestBed.inject(HttpTestingController)
  })

  afterEach(() => {
    httpMock.verify()
  })

  it('should fetch users', () => {
    const mockUsers: User[] = [
      { id: '1', name: 'John Doe', email: 'john@example.com' }
    ]

    service.getUsers().subscribe(users => {
      expect(users).toEqual(mockUsers)
    })

    const req = httpMock.expectOne('/api/users')
    expect(req.request.method).toBe('GET')
    req.flush(mockUsers)
  })

  it('should handle error responses', () => {
    service.getUsers().subscribe({
      next: () => fail('should have failed'),
      error: (error) => {
        expect(error.status).toBe(500)
      }
    })

    const req = httpMock.expectOne('/api/users')
    req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' })
  })
})
```

### 3. Integration Testing Pattern

**Page Object Model:**
```typescript
class UserListPageObject {
  constructor(private fixture: ComponentFixture<UserListComponent>) {}

  get addButton(): HTMLButtonElement {
    return this.fixture.nativeElement.querySelector('[data-testid="add-user"]')
  }

  get userRows(): NodeListOf<HTMLElement> {
    return this.fixture.nativeElement.querySelectorAll('[data-testid="user-row"]')
  }

  get searchInput(): HTMLInputElement {
    return this.fixture.nativeElement.querySelector('[data-testid="search-input"]')
  }

  async search(term: string): Promise<void> {
    this.searchInput.value = term
    this.searchInput.dispatchEvent(new Event('input'))
    this.fixture.detectChanges()
    await this.fixture.whenStable()
  }

  async clickAddButton(): Promise<void> {
    this.addButton.click()
    this.fixture.detectChanges()
    await this.fixture.whenStable()
  }

  getUserNameAt(index: number): string {
    const row = this.userRows[index]
    return row.querySelector('[data-testid="user-name"]')?.textContent?.trim() || ''
  }
}

describe('UserListComponent Integration', () => {
  let component: UserListComponent
  let fixture: ComponentFixture<UserListComponent>
  let pageObject: UserListPageObject

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserListComponent]
    }).compileComponents()

    fixture = TestBed.createComponent(UserListComponent)
    component = fixture.componentInstance
    pageObject = new UserListPageObject(fixture)
  })

  it('should filter users when searching', async () => {
    const users: User[] = [
      { id: '1', name: 'John Doe', email: 'john@example.com' },
      { id: '2', name: 'Jane Smith', email: 'jane@example.com' }
    ]
    component.users.set(users)
    fixture.detectChanges()

    await pageObject.search('John')

    expect(pageObject.userRows.length).toBe(1)
    expect(pageObject.getUserNameAt(0)).toBe('John Doe')
  })
})
```

---

## Code Organization

### 1. Barrel Exports Pattern

**Feature Index File:**
```typescript
// features/users/index.ts
export * from './pages/user-list/user-list.component'
export * from './pages/user-details/user-details.component'
export * from './components/user-card/user-card.component'
export * from './shared/services/user.service'
export * from './shared/stores/user.store'
export * from './shared/types/user.types'
export { routes as userRoutes } from './users.routes'
```

**Usage:**
```typescript
import { 
  UserListComponent, 
  UserService, 
  User, 
  userRoutes 
} from './features/users'
```

### 2. Configuration Pattern

**App Configuration:**
```typescript
// config/app.config.ts
export interface AppConfig {
  api: {
    baseUrl: string
    timeout: number
  }
  auth: {
    clientId: string
    redirectUri: string
  }
  features: {
    enableNotifications: boolean
    enableAnalytics: boolean
  }
}

export const APP_CONFIG = new InjectionToken<AppConfig>('app.config')

// Provide in main.ts
bootstrapApplication(AppComponent, {
  providers: [
    {
      provide: APP_CONFIG,
      useValue: {
        api: {
          baseUrl: environment.apiUrl,
          timeout: 30000
        },
        auth: {
          clientId: environment.authClientId,
          redirectUri: environment.authRedirectUri
        },
        features: {
          enableNotifications: true,
          enableAnalytics: environment.production
        }
      }
    }
  ]
})
```

### 3. Constants and Enums Pattern

**Constants File:**
```typescript
// shared/constants/app.constants.ts
export const API_ENDPOINTS = {
  USERS: '/api/users',
  ROLES: '/api/roles',
  PERMISSIONS: '/api/permissions'
} as const

export const VALIDATION_MESSAGES = {
  REQUIRED: 'This field is required',
  EMAIL: 'Please enter a valid email address',
  MIN_LENGTH: 'Minimum length is {min} characters',
  MAX_LENGTH: 'Maximum length is {max} characters'
} as const

export const DATE_FORMATS = {
  SHORT: 'MM/dd/yyyy',
  LONG: 'MMMM dd, yyyy',
  TIME: 'HH:mm:ss'
} as const

// Enums file
export enum UserRole {
  ADMIN = 'admin',
  USER = 'user',
  MODERATOR = 'moderator'
}

export enum UserStatus {
  ACTIVE = 'active',
  INACTIVE = 'inactive',
  PENDING = 'pending',
  SUSPENDED = 'suspended'
}

export enum ApiStatus {
  IDLE = 'idle',
  LOADING = 'loading',
  SUCCESS = 'success',
  ERROR = 'error'
}
```

These patterns provide a solid foundation for building scalable, maintainable Angular applications. Each pattern addresses specific architectural concerns and can be combined to create robust solutions.