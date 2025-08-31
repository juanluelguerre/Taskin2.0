# ANGULAR COMPONENT TEMPLATES

Ready-to-use component templates for Angular 18+ applications with modern patterns.

## Table of Contents
- [Page Components](#page-components)
- [Feature Components](#feature-components)
- [Service Templates](#service-templates)
- [Store Templates](#store-templates)
- [Type Definitions](#type-definitions)

---

## Page Components

### 1. List Page Component

**File:** `features/[feature]/pages/[feature]-list/[feature]-list.component.ts`

```typescript
import { CommonModule } from '@angular/common'
import { ChangeDetectionStrategy, Component, ViewEncapsulation, inject, signal } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { FormControl, ReactiveFormsModule } from '@angular/forms'
import { MatButtonModule } from '@angular/material/button'
import { MatIconModule } from '@angular/material/icon'
import { MatInputModule } from '@angular/material/input'
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator'
import { MatTableModule } from '@angular/material/table'
import { RouterModule } from '@angular/router'
import { debounceTime, distinctUntilChanged, filter } from 'rxjs'
import { FeatureStore } from '../../shared/stores/feature.store'
import { FeatureItemComponent } from '../../components/feature-item/feature-item.component'

@Component({
  selector: 'app-feature-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatPaginatorModule,
    MatTableModule,
    ReactiveFormsModule,
    RouterModule,
    FeatureItemComponent,
  ],
  templateUrl: './feature-list.component.html',
  styles: [],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [FeatureStore],
})
export class FeatureListComponent {
  private readonly _store = inject(FeatureStore)

  // Reactive state
  items = this._store.selectSignal(state => state.items)
  loading = this._store.selectSignal(state => state.loading)
  totalCount = this._store.selectSignal(state => state.totalCount)
  currentPage = this._store.selectSignal(state => state.currentPage)
  pageSize = this._store.selectSignal(state => state.pageSize)

  // Search control
  searchControl = new FormControl('')
  
  // Table columns
  displayedColumns = signal<string[]>(['name', 'status', 'createdAt', 'actions'])

  constructor() {
    // Load initial data
    this._store.loadItems()

    // Handle search
    this.searchControl.valueChanges.pipe(
      takeUntilDestroyed(),
      debounceTime(300),
      distinctUntilChanged(),
      filter(term => term !== null)
    ).subscribe(searchTerm => {
      this._store.setSearchTerm(searchTerm || '')
    })
  }

  onPageChange(event: PageEvent): void {
    this._store.setPage({
      pageIndex: event.pageIndex,
      pageSize: event.pageSize
    })
  }

  onRefresh(): void {
    this._store.loadItems()
  }

  onDelete(id: string): void {
    if (confirm('Are you sure you want to delete this item?')) {
      this._store.deleteItem(id)
    }
  }
}
```

**Template:** `feature-list.component.html`

```html
<div class="flex flex-col gap-4">
  <!-- Header -->
  <div class="flex justify-between items-center">
    <h1 class="text-2xl font-semibold">Features</h1>
    <button mat-raised-button color="primary" routerLink="new">
      <mat-icon>add</mat-icon>
      Create New
    </button>
  </div>

  <!-- Search -->
  <mat-form-field appearance="outline" class="w-full max-w-md">
    <mat-label>Search features...</mat-label>
    <input matInput [formControl]="searchControl" placeholder="Enter search term">
    <mat-icon matSuffix>search</mat-icon>
  </mat-form-field>

  <!-- Loading State -->
  @if (loading()) {
    <div class="flex justify-center p-8">
      <mat-spinner></mat-spinner>
    </div>
  }

  <!-- Data Table -->
  @if (!loading() && items().length > 0) {
    <div class="overflow-x-auto">
      <table mat-table [dataSource]="items()" class="w-full">
        
        <!-- Name Column -->
        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Name</th>
          <td mat-cell *matCellDef="let item">
            <a [routerLink]="[item.id]" class="text-blue-600 hover:text-blue-800">
              {{ item.name }}
            </a>
          </td>
        </ng-container>

        <!-- Status Column -->
        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let item">
            <span [class]="getStatusClass(item.status)">
              {{ item.status }}
            </span>
          </td>
        </ng-container>

        <!-- Created Date Column -->
        <ng-container matColumnDef="createdAt">
          <th mat-header-cell *matHeaderCellDef>Created</th>
          <td mat-cell *matCellDef="let item">
            {{ item.createdAt | date:'short' }}
          </td>
        </ng-container>

        <!-- Actions Column -->
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Actions</th>
          <td mat-cell *matCellDef="let item">
            <button mat-icon-button [routerLink]="[item.id]" title="View">
              <mat-icon>visibility</mat-icon>
            </button>
            <button mat-icon-button [routerLink]="[item.id, 'edit']" title="Edit">
              <mat-icon>edit</mat-icon>
            </button>
            <button mat-icon-button (click)="onDelete(item.id)" title="Delete" class="text-red-600">
              <mat-icon>delete</mat-icon>
            </button>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns()"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns();"></tr>
      </table>
    </div>

    <!-- Pagination -->
    <mat-paginator 
      [length]="totalCount()"
      [pageSize]="pageSize()"
      [pageIndex]="currentPage()"
      [pageSizeOptions]="[10, 25, 50, 100]"
      (page)="onPageChange($event)">
    </mat-paginator>
  }

  <!-- Empty State -->
  @if (!loading() && items().length === 0) {
    <div class="text-center py-12">
      <mat-icon class="text-6xl text-gray-400 mb-4">inbox</mat-icon>
      <h3 class="text-xl font-medium text-gray-600 mb-2">No features found</h3>
      <p class="text-gray-500 mb-4">Get started by creating your first feature.</p>
      <button mat-raised-button color="primary" routerLink="new">
        Create Feature
      </button>
    </div>
  }
</div>
```

### 2. Details Page Component

**File:** `features/[feature]/pages/[feature]-details/[feature]-details.component.ts`

```typescript
import { CommonModule } from '@angular/common'
import { ChangeDetectionStrategy, Component, ViewEncapsulation, inject, signal } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms'
import { MatButtonModule } from '@angular/material/button'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatIconModule } from '@angular/material/icon'
import { MatInputModule } from '@angular/material/input'
import { MatSelectModule } from '@angular/material/select'
import { MatSlideToggleModule } from '@angular/material/slide-toggle'
import { MatTabsModule } from '@angular/material/tabs'
import { ActivatedRoute, RouterModule } from '@angular/router'
import { filter } from 'rxjs'
import { FeatureDetailsStore } from '../../shared/stores/feature-details.store'
import { Feature, FeatureStatus } from '../../shared/types/feature.types'

interface RouteTab {
  path: string
  label: string
  icon: string
}

@Component({
  selector: 'app-feature-details',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatTabsModule,
    ReactiveFormsModule,
    RouterModule,
  ],
  templateUrl: './feature-details.component.html',
  styles: [],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [FeatureDetailsStore],
})
export class FeatureDetailsComponent {
  private readonly _fb = inject(FormBuilder)
  private readonly _route = inject(ActivatedRoute)
  private readonly _store = inject(FeatureDetailsStore)

  // Reactive state
  feature = this._store.selectSignal(state => state.feature)
  loading = this._store.selectSignal(state => state.loading)
  saving = this._store.selectSignal(state => state.saving)

  // Navigation tabs
  tabs = signal<RouteTab[]>([
    { path: './', label: 'General', icon: 'info' },
    { path: 'settings', label: 'Settings', icon: 'settings' },
    { path: 'history', label: 'History', icon: 'history' },
  ])

  // Form
  form = this._fb.group({
    id: [''],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', Validators.maxLength(500)],
    status: [FeatureStatus.Active, Validators.required],
    enabled: [true],
  })

  // Status options
  statusOptions = Object.values(FeatureStatus)

  constructor() {
    // Load feature data
    this._route.params.pipe(
      takeUntilDestroyed()
    ).subscribe(params => {
      const id = params['id']
      if (id) {
        this._store.loadFeature(id)
      }
    })

    // Update form when feature loads
    this._store.select(state => state.feature).pipe(
      takeUntilDestroyed(),
      filter(feature => !!feature)
    ).subscribe(feature => {
      this.form.patchValue(feature)
      this.form.markAsPristine()
    })
  }

  onSave(): void {
    if (this.form.valid && this.form.dirty) {
      const formValue = this.form.value as Feature
      this._store.saveFeature(formValue)
    }
  }

  onReset(): void {
    const feature = this.feature()
    if (feature) {
      this.form.patchValue(feature)
      this.form.markAsPristine()
    }
  }

  canDeactivate(): boolean {
    return !this.form.dirty
  }
}
```

### 3. Create/Edit Page Component

**File:** `features/[feature]/pages/[feature]-form/[feature]-form.component.ts`

```typescript
import { CommonModule } from '@angular/common'
import { ChangeDetectionStrategy, Component, ViewEncapsulation, inject } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms'
import { MatButtonModule } from '@angular/material/button'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatIconModule } from '@angular/material/icon'
import { MatInputModule } from '@angular/material/input'
import { MatSelectModule } from '@angular/material/select'
import { MatSlideToggleModule } from '@angular/material/slide-toggle'
import { ActivatedRoute, Router, RouterModule } from '@angular/router'
import { filter, map } from 'rxjs'
import { FeatureStore } from '../../shared/stores/feature.store'
import { CreateFeatureRequest, FeatureStatus, UpdateFeatureRequest } from '../../shared/types/feature.types'

@Component({
  selector: 'app-feature-form',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    ReactiveFormsModule,
    RouterModule,
  ],
  templateUrl: './feature-form.component.html',
  styles: [],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [FeatureStore],
})
export class FeatureFormComponent {
  private readonly _fb = inject(FormBuilder)
  private readonly _route = inject(ActivatedRoute)
  private readonly _router = inject(Router)
  private readonly _store = inject(FeatureStore)

  // Determine if we're editing or creating
  isEditing = this._route.snapshot.params['id'] !== undefined
  featureId = this._route.snapshot.params['id']

  // Reactive state
  loading = this._store.selectSignal(state => state.loading)
  saving = this._store.selectSignal(state => state.saving)
  
  // Form
  form = this._fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', Validators.maxLength(500)],
    status: [FeatureStatus.Active, Validators.required],
    enabled: [true],
    tags: [[] as string[]],
    metadata: this._fb.group({
      priority: ['medium'],
      category: [''],
    }),
  })

  // Options
  statusOptions = Object.values(FeatureStatus)
  priorityOptions = ['low', 'medium', 'high', 'critical']

  constructor() {
    if (this.isEditing) {
      this.loadFeatureForEdit()
    }
  }

  private loadFeatureForEdit(): void {
    this._store.loadFeature(this.featureId)
    
    this._store.select(state => state.selectedFeature).pipe(
      takeUntilDestroyed(),
      filter(feature => !!feature)
    ).subscribe(feature => {
      this.form.patchValue({
        name: feature.name,
        description: feature.description,
        status: feature.status,
        enabled: feature.enabled,
        tags: feature.tags || [],
        metadata: {
          priority: feature.metadata?.priority || 'medium',
          category: feature.metadata?.category || '',
        },
      })
      this.form.markAsPristine()
    })
  }

  onSubmit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched()
      return
    }

    const formValue = this.form.value

    if (this.isEditing) {
      const updateRequest: UpdateFeatureRequest = {
        id: this.featureId,
        ...formValue as any,
      }
      this._store.updateFeature(updateRequest)
    } else {
      const createRequest: CreateFeatureRequest = formValue as any
      this._store.createFeature(createRequest)
    }

    // Navigate back on success
    this._store.select(state => ({ saving: state.saving, error: state.error })).pipe(
      takeUntilDestroyed(),
      filter(({ saving, error }) => !saving && !error)
    ).subscribe(() => {
      this._router.navigate(['../'], { relativeTo: this._route })
    })
  }

  onCancel(): void {
    this._router.navigate(['../'], { relativeTo: this._route })
  }

  canDeactivate(): boolean {
    return !this.form.dirty
  }

  getFieldError(fieldName: string): string {
    const field = this.form.get(fieldName)
    if (field?.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`
      if (field.errors['maxlength']) return `${fieldName} is too long`
      if (field.errors['email']) return `Invalid email format`
    }
    return ''
  }
}
```

---

## Feature Components

### 1. Reusable Display Component

**File:** `features/[feature]/components/[feature]-card/[feature]-card.component.ts`

```typescript
import { CommonModule } from '@angular/common'
import { ChangeDetectionStrategy, Component, ViewEncapsulation, input, output } from '@angular/core'
import { MatButtonModule } from '@angular/material/button'
import { MatCardModule } from '@angular/material/card'
import { MatIconModule } from '@angular/material/icon'
import { MatMenuModule } from '@angular/material/menu'
import { RouterModule } from '@angular/router'
import { Feature, FeatureStatus } from '../../shared/types/feature.types'

@Component({
  selector: 'app-feature-card',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatMenuModule,
    RouterModule,
  ],
  templateUrl: './feature-card.component.html',
  styles: [`
    .status-active { @apply bg-green-100 text-green-800; }
    .status-inactive { @apply bg-red-100 text-red-800; }
    .status-pending { @apply bg-yellow-100 text-yellow-800; }
  `],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeatureCardComponent {
  // Inputs
  feature = input.required<Feature>()
  showActions = input<boolean>(true)
  compact = input<boolean>(false)

  // Outputs
  edit = output<Feature>()
  delete = output<Feature>()
  toggle = output<Feature>()

  getStatusClass(status: FeatureStatus): string {
    switch (status) {
      case FeatureStatus.Active: return 'status-active'
      case FeatureStatus.Inactive: return 'status-inactive'
      case FeatureStatus.Pending: return 'status-pending'
      default: return ''
    }
  }

  onEdit(): void {
    this.edit.emit(this.feature())
  }

  onDelete(): void {
    this.delete.emit(this.feature())
  }

  onToggle(): void {
    this.toggle.emit(this.feature())
  }
}
```

### 2. Form Input Component

**File:** `shared/components/form-field/form-field.component.ts`

```typescript
import { CommonModule } from '@angular/common'
import { ChangeDetectionStrategy, Component, ViewEncapsulation, input, signal } from '@angular/core'
import { ControlValueAccessor, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatIconModule } from '@angular/material/icon'

@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    ReactiveFormsModule,
  ],
  template: `
    <mat-form-field [appearance]="appearance()" class="w-full">
      <mat-label>{{ label() }}</mat-label>
      
      @if (prefixIcon()) {
        <mat-icon matPrefix>{{ prefixIcon() }}</mat-icon>
      }
      
      <input 
        matInput
        [type]="type()"
        [placeholder]="placeholder()"
        [value]="value()"
        [disabled]="disabled()"
        [required]="required()"
        (input)="onInput($event)"
        (blur)="onBlur()"
        (focus)="onFocus()"
      />
      
      @if (suffixIcon()) {
        <mat-icon matSuffix>{{ suffixIcon() }}</mat-icon>
      }
      
      @if (error()) {
        <mat-error>{{ error() }}</mat-error>
      }
      
      @if (hint()) {
        <mat-hint>{{ hint() }}</mat-hint>
      }
    </mat-form-field>
  `,
  styles: [],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: FormFieldComponent,
      multi: true
    }
  ]
})
export class FormFieldComponent implements ControlValueAccessor {
  // Inputs
  label = input<string>('')
  placeholder = input<string>('')
  type = input<string>('text')
  appearance = input<'fill' | 'outline'>('outline')
  prefixIcon = input<string>('')
  suffixIcon = input<string>('')
  required = input<boolean>(false)
  error = input<string>('')
  hint = input<string>('')

  // Internal state
  value = signal<string>('')
  disabled = signal<boolean>(false)
  touched = signal<boolean>(false)

  private onChange = (value: string): void => {}
  private onTouched = (): void => {}

  // ControlValueAccessor implementation
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

  // Event handlers
  onInput(event: Event): void {
    const target = event.target as HTMLInputElement
    const value = target.value
    this.value.set(value)
    this.onChange(value)
  }

  onBlur(): void {
    this.touched.set(true)
    this.onTouched()
  }

  onFocus(): void {
    // Handle focus events if needed
  }
}
```

---

## Service Templates

### 1. HTTP Data Service

**File:** `shared/services/data.service.ts`

```typescript
import { HttpClient, HttpParams } from '@angular/common/http'
import { Injectable, inject } from '@angular/core'
import { Observable } from 'rxjs'

export interface PaginatedRequest {
  page: number
  size: number
  search?: string
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  size: number
  totalPages: number
}

export interface IDataService<T, TCreate, TUpdate> {
  getAll(request: PaginatedRequest): Observable<PaginatedResponse<T>>
  getById(id: string): Observable<T>
  create(item: TCreate): Observable<T>
  update(id: string, item: TUpdate): Observable<T>
  delete(id: string): Observable<void>
  search(query: string): Observable<T[]>
}

@Injectable({
  providedIn: 'root'
})
export class DataService<T, TCreate = T, TUpdate = T> implements IDataService<T, TCreate, TUpdate> {
  protected readonly _http = inject(HttpClient)
  protected readonly baseUrl = '/api'

  constructor(protected readonly endpoint: string) {}

  getAll(request: PaginatedRequest): Observable<PaginatedResponse<T>> {
    let params = new HttpParams()
      .set('page', request.page.toString())
      .set('size', request.size.toString())

    if (request.search) params = params.set('search', request.search)
    if (request.sortBy) params = params.set('sortBy', request.sortBy)
    if (request.sortDirection) params = params.set('sortDirection', request.sortDirection)

    return this._http.get<PaginatedResponse<T>>(`${this.baseUrl}/${this.endpoint}`, { params })
  }

  getById(id: string): Observable<T> {
    return this._http.get<T>(`${this.baseUrl}/${this.endpoint}/${id}`)
  }

  create(item: TCreate): Observable<T> {
    return this._http.post<T>(`${this.baseUrl}/${this.endpoint}`, item)
  }

  update(id: string, item: TUpdate): Observable<T> {
    return this._http.put<T>(`${this.baseUrl}/${this.endpoint}/${id}`, item)
  }

  delete(id: string): Observable<void> {
    return this._http.delete<void>(`${this.baseUrl}/${this.endpoint}/${id}`)
  }

  search(query: string): Observable<T[]> {
    const params = new HttpParams().set('q', query)
    return this._http.get<T[]>(`${this.baseUrl}/${this.endpoint}/search`, { params })
  }
}
```

### 2. Specific Feature Service

**File:** `features/[feature]/shared/services/[feature].service.ts`

```typescript
import { Injectable } from '@angular/core'
import { Observable } from 'rxjs'
import { DataService } from '../../../../shared/services/data.service'
import { Feature, CreateFeatureRequest, UpdateFeatureRequest } from '../types/feature.types'

export interface IFeatureService {
  getFeatures(request: PaginatedRequest): Observable<PaginatedResponse<Feature>>
  getFeature(id: string): Observable<Feature>
  createFeature(request: CreateFeatureRequest): Observable<Feature>
  updateFeature(id: string, request: UpdateFeatureRequest): Observable<Feature>
  deleteFeature(id: string): Observable<void>
  toggleFeature(id: string): Observable<Feature>
  getFeatureStats(): Observable<FeatureStats>
}

@Injectable({
  providedIn: 'root'
})
export class FeatureService extends DataService<Feature, CreateFeatureRequest, UpdateFeatureRequest> implements IFeatureService {

  constructor() {
    super('features')
  }

  getFeatures = this.getAll
  getFeature = this.getById
  createFeature = this.create
  updateFeature = this.update
  deleteFeature = this.delete

  toggleFeature(id: string): Observable<Feature> {
    return this._http.post<Feature>(`${this.baseUrl}/${this.endpoint}/${id}/toggle`, {})
  }

  getFeatureStats(): Observable<FeatureStats> {
    return this._http.get<FeatureStats>(`${this.baseUrl}/${this.endpoint}/stats`)
  }
}
```

---

## Store Templates

### 1. NgRx Signal Store

**File:** `features/[feature]/shared/stores/[feature].store.ts`

```typescript
import { Injectable, inject } from '@angular/core'
import { Router } from '@angular/router'
import { tapResponse } from '@ngrx/operators'
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals'
import { rxMethod } from '@ngrx/signals/rxjs-interop'
import { computed } from '@angular/core'
import { pipe, switchMap, exhaustMap, debounceTime, distinctUntilChanged } from 'rxjs'
import { FeatureService } from '../services/feature.service'
import { Feature, CreateFeatureRequest, UpdateFeatureRequest, FeatureStatus } from '../types/feature.types'
import { PaginatedRequest } from '../../../../shared/services/data.service'

type FeatureState = {
  // Data
  items: Feature[]
  selectedItem: Feature | null
  
  // Loading states
  loading: boolean
  saving: boolean
  deleting: boolean
  
  // Pagination
  currentPage: number
  pageSize: number
  totalCount: number
  
  // Filters
  searchTerm: string
  statusFilter: FeatureStatus | null
  
  // Error handling
  error: string | null
}

const initialState: FeatureState = {
  items: [],
  selectedItem: null,
  loading: false,
  saving: false,
  deleting: false,
  currentPage: 0,
  pageSize: 25,
  totalCount: 0,
  searchTerm: '',
  statusFilter: null,
  error: null,
}

@Injectable()
export class FeatureStore extends signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed(({ items, statusFilter, searchTerm }) => ({
    filteredItems: computed(() => {
      let filtered = items()
      
      if (statusFilter()) {
        filtered = filtered.filter(item => item.status === statusFilter())
      }
      
      if (searchTerm()) {
        const term = searchTerm().toLowerCase()
        filtered = filtered.filter(item => 
          item.name.toLowerCase().includes(term) ||
          item.description?.toLowerCase().includes(term)
        )
      }
      
      return filtered
    }),
    
    activeItemsCount: computed(() => 
      items().filter(item => item.status === FeatureStatus.Active).length
    ),
    
    totalPages: computed(({ pageSize, totalCount }) => 
      Math.ceil(totalCount() / pageSize())
    ),
  })),
  withMethods((
    store,
    featureService = inject(FeatureService),
    router = inject(Router)
  ) => ({
    
    // Load methods
    loadItems: rxMethod<void>(
      pipe(
        switchMap(() => {
          patchState(store, { loading: true, error: null })
          
          const request: PaginatedRequest = {
            page: store.currentPage(),
            size: store.pageSize(),
            search: store.searchTerm() || undefined,
          }
          
          return featureService.getFeatures(request).pipe(
            tapResponse({
              next: (response) => {
                patchState(store, {
                  items: response.items,
                  totalCount: response.totalCount,
                  loading: false
                })
              },
              error: (error) => {
                patchState(store, {
                  loading: false,
                  error: 'Failed to load features'
                })
                console.error('Load features error:', error)
              }
            })
          )
        })
      )
    ),

    loadItem: rxMethod<string>(
      pipe(
        switchMap((id) => {
          patchState(store, { loading: true, error: null })
          
          return featureService.getFeature(id).pipe(
            tapResponse({
              next: (item) => {
                patchState(store, {
                  selectedItem: item,
                  loading: false
                })
              },
              error: (error) => {
                patchState(store, {
                  loading: false,
                  error: 'Failed to load feature'
                })
                router.navigate(['/404'], { skipLocationChange: true })
              }
            })
          )
        })
      )
    ),

    // CRUD methods
    createItem: rxMethod<CreateFeatureRequest>(
      pipe(
        exhaustMap((request) => {
          patchState(store, { saving: true, error: null })
          
          return featureService.createFeature(request).pipe(
            tapResponse({
              next: (newItem) => {
                patchState(store, {
                  items: [newItem, ...store.items()],
                  saving: false
                })
              },
              error: (error) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to create feature'
                })
                console.error('Create feature error:', error)
              }
            })
          )
        })
      )
    ),

    updateItem: rxMethod<{ id: string; request: UpdateFeatureRequest }>(
      pipe(
        exhaustMap(({ id, request }) => {
          patchState(store, { saving: true, error: null })
          
          return featureService.updateFeature(id, request).pipe(
            tapResponse({
              next: (updatedItem) => {
                patchState(store, {
                  items: store.items().map(item => 
                    item.id === id ? updatedItem : item
                  ),
                  selectedItem: store.selectedItem()?.id === id ? updatedItem : store.selectedItem(),
                  saving: false
                })
              },
              error: (error) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to update feature'
                })
                console.error('Update feature error:', error)
              }
            })
          )
        })
      )
    ),

    deleteItem: rxMethod<string>(
      pipe(
        exhaustMap((id) => {
          patchState(store, { deleting: true, error: null })
          
          return featureService.deleteFeature(id).pipe(
            tapResponse({
              next: () => {
                patchState(store, {
                  items: store.items().filter(item => item.id !== id),
                  selectedItem: store.selectedItem()?.id === id ? null : store.selectedItem(),
                  deleting: false
                })
              },
              error: (error) => {
                patchState(store, {
                  deleting: false,
                  error: 'Failed to delete feature'
                })
                console.error('Delete feature error:', error)
              }
            })
          )
        })
      )
    ),

    // Filter and pagination methods
    setSearchTerm: (searchTerm: string) => {
      patchState(store, { searchTerm, currentPage: 0 })
      store.loadItems()
    },

    setStatusFilter: (statusFilter: FeatureStatus | null) => {
      patchState(store, { statusFilter, currentPage: 0 })
      store.loadItems()
    },

    setPage: ({ pageIndex, pageSize }: { pageIndex: number; pageSize: number }) => {
      patchState(store, { 
        currentPage: pageIndex,
        pageSize: pageSize !== store.pageSize() ? pageSize : store.pageSize()
      })
      store.loadItems()
    },

    // Utility methods
    clearError: () => patchState(store, { error: null }),
    
    clearSelection: () => patchState(store, { selectedItem: null }),

    refreshItems: () => {
      store.loadItems()
    },
  }))
) {}
```

---

## Type Definitions

### 1. Feature Types

**File:** `features/[feature]/shared/types/[feature].types.ts`

```typescript
// Base entity interface
export interface BaseEntity {
  id: string
  createdAt: Date
  updatedAt: Date
  createdBy?: string
  updatedBy?: string
}

// Enums
export enum FeatureStatus {
  Active = 'active',
  Inactive = 'inactive',
  Pending = 'pending',
  Archived = 'archived',
}

export enum FeaturePriority {
  Low = 'low',
  Medium = 'medium',
  High = 'high',
  Critical = 'critical',
}

// Main entity
export interface Feature extends BaseEntity {
  name: string
  description?: string
  status: FeatureStatus
  priority: FeaturePriority
  enabled: boolean
  tags: string[]
  metadata: FeatureMetadata
  statistics: FeatureStatistics
}

// Related interfaces
export interface FeatureMetadata {
  category: string
  version: string
  author: string
  lastModifiedBy: string
  customFields: Record<string, any>
}

export interface FeatureStatistics {
  usageCount: number
  lastUsed?: Date
  averageResponseTime?: number
  errorRate?: number
}

// Request/Response types
export interface CreateFeatureRequest {
  name: string
  description?: string
  status: FeatureStatus
  priority: FeaturePriority
  enabled: boolean
  tags?: string[]
  metadata?: Partial<FeatureMetadata>
}

export interface UpdateFeatureRequest extends Partial<CreateFeatureRequest> {
  id: string
}

export interface FeatureListResponse {
  items: Feature[]
  totalCount: number
  page: number
  size: number
}

export interface FeatureStats {
  totalFeatures: number
  activeFeatures: number
  inactiveFeatures: number
  pendingFeatures: number
  featuresCreatedThisMonth: number
  averageUsagePerFeature: number
}

// Filter and search types
export interface FeatureFilters {
  status?: FeatureStatus
  priority?: FeaturePriority
  enabled?: boolean
  tags?: string[]
  category?: string
  createdDateRange?: {
    start: Date
    end: Date
  }
}

export interface FeatureSearchRequest {
  query: string
  filters?: FeatureFilters
  sortBy?: keyof Feature
  sortDirection?: 'asc' | 'desc'
  page: number
  size: number
}

// Form types
export interface FeatureFormData {
  name: string
  description: string
  status: FeatureStatus
  priority: FeaturePriority
  enabled: boolean
  tags: string[]
  category: string
}

// Validation schemas
export const FEATURE_VALIDATION = {
  name: {
    required: true,
    minLength: 2,
    maxLength: 100,
  },
  description: {
    maxLength: 500,
  },
  tags: {
    maxItems: 10,
    maxLength: 50, // per tag
  },
} as const

// Type guards
export function isFeature(obj: any): obj is Feature {
  return obj && typeof obj.id === 'string' && typeof obj.name === 'string'
}

export function isCreateFeatureRequest(obj: any): obj is CreateFeatureRequest {
  return obj && typeof obj.name === 'string' && Object.values(FeatureStatus).includes(obj.status)
}

// Utility types
export type FeatureKeys = keyof Feature
export type FeatureSortableFields = 'name' | 'createdAt' | 'updatedAt' | 'status' | 'priority'
export type FeatureTableColumn = {
  key: FeatureKeys
  label: string
  sortable: boolean
  width?: string
}
```

### 2. Common Types

**File:** `shared/types/common.types.ts`

```typescript
// API Response types
export interface ApiResponse<T = any> {
  data: T
  success: boolean
  message?: string
  errors?: string[]
  timestamp: string
}

export interface PaginatedApiResponse<T> extends ApiResponse<T[]> {
  pagination: {
    page: number
    size: number
    totalCount: number
    totalPages: number
    hasNext: boolean
    hasPrevious: boolean
  }
}

// Error types
export interface ApiError {
  code: string
  message: string
  details?: Record<string, any>
  timestamp: string
}

export interface ValidationError {
  field: string
  message: string
  code: string
}

// Loading states
export type LoadingState = 'idle' | 'loading' | 'success' | 'error'

export interface AsyncState<T> {
  data: T | null
  loading: boolean
  error: string | null
  lastUpdated?: Date
}

// Form types
export interface FormField {
  name: string
  label: string
  type: 'text' | 'email' | 'password' | 'number' | 'select' | 'textarea' | 'checkbox' | 'date'
  required: boolean
  placeholder?: string
  options?: SelectOption[]
  validation?: ValidationRule[]
}

export interface SelectOption {
  value: any
  label: string
  disabled?: boolean
}

export interface ValidationRule {
  type: 'required' | 'minLength' | 'maxLength' | 'pattern' | 'email' | 'min' | 'max'
  value?: any
  message: string
}

// UI types
export interface MenuItem {
  id: string
  label: string
  icon?: string
  route?: string
  action?: () => void
  children?: MenuItem[]
  disabled?: boolean
  visible?: boolean
}

export interface TableColumn<T = any> {
  key: keyof T | string
  label: string
  sortable?: boolean
  width?: string
  align?: 'left' | 'center' | 'right'
  type?: 'text' | 'number' | 'date' | 'boolean' | 'action'
  format?: (value: any) => string
}

export interface TableAction<T = any> {
  icon: string
  label: string
  action: (item: T) => void
  visible?: (item: T) => boolean
  disabled?: (item: T) => boolean
}

// Notification types
export interface NotificationConfig {
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message: string
  duration?: number
  action?: {
    label: string
    handler: () => void
  }
}

// Route types
export interface RouteData {
  title?: string
  breadcrumb?: string
  permissions?: string[]
  layout?: 'default' | 'minimal' | 'fullscreen'
}

// Theme types
export interface ThemeConfig {
  primary: string
  accent: string
  warn: string
  isDark: boolean
  customCss?: Record<string, string>
}

// User preference types
export interface UserPreferences {
  theme: 'light' | 'dark' | 'auto'
  language: string
  timezone: string
  dateFormat: string
  pageSize: number
  notifications: {
    email: boolean
    push: boolean
    inApp: boolean
  }
}
```

These templates provide a comprehensive foundation for building Angular 18+ applications with modern patterns, proper typing, and best practices. All references to specific business logic have been removed, making them reusable across different projects.