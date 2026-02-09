# State Management Patterns — Taskin 2.0

## Overview

Taskin 2.0 uses **NgRx Signal Store** for state management, combined with Angular Signals for local component state and RxJS for async operations.

## Decision Matrix

| Scenario | Solution |
|----------|----------|
| Feature-wide state (projects, tasks) | NgRx Signal Store |
| Local component UI state | `signal()` / `computed()` |
| Derived synchronous values | `computed()` |
| HTTP calls + state updates | `rxMethod` with RxJS operators |
| Cross-component events | `Subject<void>` → `toSignal()` |
| Form state | Reactive Forms (`FormGroup`) |

## NgRx Signal Store Pattern

### Store Definition

The project uses the class-extends-signalStore pattern:

```typescript
import { Injectable, inject, computed } from '@angular/core';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, exhaustMap, tap } from 'rxjs';

// 1. Define explicit state type
type ProjectState = {
  projects: ProjectListDto[];
  selectedProject: ProjectDetailsDto | null;
  loading: boolean;
  saving: boolean;
  deleting: boolean;
  currentPage: number;
  pageSize: number;
  totalCount: number;
  searchTerm: string;
  statusFilter: ProjectStatus | null;
  error: string | null;
};

// 2. Define initial state
const initialState: ProjectState = {
  projects: [],
  selectedProject: null,
  loading: false,
  saving: false,
  deleting: false,
  currentPage: 1,
  pageSize: 12,
  totalCount: 0,
  searchTerm: '',
  statusFilter: null,
  error: null,
};

// 3. Create store with class-extends pattern
@Injectable()
export class ProjectStore extends signalStore(
  { providedIn: 'root' },
  withState(initialState),

  // 4. Computed values
  withComputed((store) => ({
    filteredProjects: computed(() => {
      let filtered = store.projects();
      const search = store.searchTerm().toLowerCase().trim();
      if (search) {
        filtered = filtered.filter(p =>
          p.name.toLowerCase().includes(search)
        );
      }
      return filtered;
    }),
    totalPages: computed(() =>
      Math.ceil(store.totalCount() / store.pageSize())
    ),
  })),

  // 5. Methods with DI
  withMethods((
    store,
    service = inject(ProjectService),
    notification = inject(NotificationService),
    router = inject(Router),
  ) => ({
    // Queries: switchMap (cancels stale requests)
    loadProjects: rxMethod<void>(
      pipe(
        switchMap(() => {
          patchState(store, { loading: true, error: null });
          return service.getProjects({
            page: store.currentPage(),
            size: store.pageSize(),
            search: store.searchTerm() || undefined,
          }).pipe(
            tap({
              next: (response) => patchState(store, {
                projects: response.data,
                totalCount: response.total,
                loading: false,
              }),
              error: () => {
                patchState(store, { loading: false, error: 'Failed' });
                notification.notifyError('projects.errors.loadFailed');
              },
            })
          );
        })
      )
    ),

    // Mutations: exhaustMap (ignores while saving)
    createProject: rxMethod<CreateProjectCommand>(
      pipe(
        exhaustMap((command) => {
          patchState(store, { saving: true, error: null });
          return service.createProject(command).pipe(
            tap({
              next: (response) => {
                patchState(store, { saving: false });
                notification.notifySuccess('projects.messages.created');
                router.navigate(['/projects']);
              },
              error: () => {
                patchState(store, { saving: false, error: 'Failed' });
                notification.notifyError('projects.errors.createFailed');
              },
            })
          );
        })
      )
    ),

    // Synchronous state updates
    setSearchTerm: (term: string) =>
      patchState(store, { searchTerm: term, currentPage: 1 }),
    setPage: (page: number) =>
      patchState(store, { currentPage: page }),
    clearError: () =>
      patchState(store, { error: null }),
  }))
) {}
```

### Using the Store in Components

```typescript
@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [ProjectStore],
})
export class ProjectsComponent implements OnInit {
  readonly store = inject(ProjectStore);

  ngOnInit(): void {
    this.store.loadProjects();
    this.store.loadProjectStats();
  }

  onSearch(term: string): void {
    this.store.searchProjects(term);
  }

  onDelete(id: string): void {
    this.store.deleteProject(id);
  }
}
```

## RxJS Operator Selection

### switchMap — For Queries

Cancels previous request when new one arrives. Use for:
- Loading lists
- Search/typeahead
- Loading details by ID

```typescript
loadItems: rxMethod<void>(
  pipe(
    switchMap(() => service.getItems().pipe(
      tap({ next: (data) => patchState(store, { items: data }) })
    ))
  )
)
```

### exhaustMap — For Mutations

Ignores new triggers while current is in progress. Use for:
- Create / Update / Delete operations
- Form submissions
- Login actions

```typescript
createItem: rxMethod<CreateCommand>(
  pipe(
    exhaustMap((cmd) => service.create(cmd).pipe(
      tap({ next: () => notification.notifySuccess('Created') })
    ))
  )
)
```

### debounceTime + distinctUntilChanged — For Search

```typescript
searchItems: rxMethod<string>(
  pipe(
    debounceTime(300),
    distinctUntilChanged(),
    switchMap((term) => service.search(term).pipe(
      tap({ next: (results) => patchState(store, { items: results }) })
    ))
  )
)
```

## RxJS + Signals Interop

### Observable → Signal

Convert once at field level. Never inside `computed()` or `effect()`.

```typescript
readonly userSig = toSignal(this.userService.user$, { initialValue: undefined });
readonly permissions = computed(() => derivePerms(this.userSig()));
```

### Signal → Observable

Only at boundary layers (e.g., feeding to a third-party library).

```typescript
readonly obs$ = toObservable(this.mySignal);
```

### Signal-Friendly Service Pattern

```typescript
@Injectable({ providedIn: 'root' })
export class ItemsService {
  private readonly reload$ = new Subject<void>();

  private readonly items$ = this.reload$.pipe(
    startWith(void 0),
    switchMap(() => this.http.get<Item[]>('/api/items')),
    shareReplay({ bufferSize: 1, refCount: true })
  );

  // Public signal for templates
  readonly items = toSignal(this.items$, { initialValue: [] as Item[] });

  refresh(): void {
    this.reload$.next();
  }
}
```

### Subject Selection

| Need | Use |
|------|-----|
| Event trigger (no value) | `Subject<void>` |
| Current value needed | `BehaviorSubject<T>` |
| Replay past values | `ReplaySubject<T>(n)` |

Prefer `signal()` over `BehaviorSubject` for local state. Use `BehaviorSubject` only for cross-service reactive streams.

## Anti-Patterns

1. **`toSignal()` inside `computed()`** — causes NG0602 error
2. **Multiple `toSignal()` on same cold Observable** — duplicates HTTP requests
3. **`mergeMap` for request-response** — use `switchMap` for cancellation
4. **`BehaviorSubject` + `signal()` for same state** — pick one canonical source
5. **Manual `subscribe()` in components** — prefer `toSignal()` or `rxMethod`
6. **`shareReplay` without refresh** — may serve stale data; add explicit triggers

## Local Component State

For simple UI state that doesn't need a store:

```typescript
export class SimpleComponent {
  private readonly items = signal<Item[]>([]);
  private readonly selectedId = signal<string | null>(null);

  readonly selectedItem = computed(() => {
    const id = this.selectedId();
    return this.items().find(i => i.id === id) ?? null;
  });

  selectItem(id: string): void {
    this.selectedId.set(id);
  }

  addItem(item: Item): void {
    this.items.update(current => [...current, item]);
  }
}
```
