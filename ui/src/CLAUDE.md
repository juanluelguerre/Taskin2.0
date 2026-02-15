# Frontend CLAUDE.md — Angular 21

## Stack

Angular 21.1.3 | Material 21 (MD3) | Tailwind CSS 4 | NgRx Signals 21 | Transloco 8 | TypeScript 5.9 | Zoneless

## Rules

1. **Standalone components only** — no NgModules
2. **OnPush change detection** — always
3. **Zoneless** — no zone.js dependency
4. **Signal-based inputs/outputs** — `input()`, `input.required()`, `output()`
5. **Separate template files** — never inline HTML
6. **Semicolons required** — in all TypeScript files
7. **No `any` type** — use `unknown` when uncertain
8. **Path aliases** — `@core`, `@shared`, `@theme`, `@env`

## Component Generation

```bash
npx ng g c features/X/pages/Y --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
```

Do NOT include `--standalone` flag (it's the default in Angular 21).

## Material Design 3 Syntax

MD3 button variants (Angular Material 21):
```html
<button mat-button>Basic</button>
<button mat-raised-button>Raised (legacy)</button>
<button mat-flat-button>Flat (legacy)</button>
<!-- MD3 variants -->
<button matButton="filled">Filled</button>
<button matButton="elevated">Elevated</button>
<button matButton="outlined">Outlined</button>
<button matButton="tonal">Tonal</button>
<button mat-icon-button>
  <mat-icon>edit</mat-icon>
</button>
<button mat-fab>
  <mat-icon>add</mat-icon>
</button>
<button mat-mini-fab>
  <mat-icon>add</mat-icon>
</button>
```

## Tailwind CSS v4 Patterns

### Setup
- `styles.css`: `@import 'tailwindcss'` + `@theme { }` + `@source` directive
- Component SCSS using `@apply`: Add `@reference "tailwindcss";` at top
- Class renames: `shadow-sm` → `shadow-xs`
- Media queries: `@media (width >= 768px) {}` (NOT `@screen md {}`)

### CSS Cascade Fix (Material vs Tailwind)
Tailwind v4 wraps utilities in `@layer utilities` — unlayered CSS always wins.
Material's unlayered styles beat Tailwind utility classes.

**Fix**: Add unlayered overrides in `styles.css`:
```css
mat-icon.text-blue-600 { color: var(--color-blue-600); }
app-sidebar nav a { color: var(--color-slate-300); }
```

### Layout Preferences
- Use `flex` + `gap-*` (never `space-y` / `space-x`)
- Use `rounded-xl`, `border-gray-100` for cards
- Use `tracking-tight` / `tracking-wider` for typography

## Component Pattern

```typescript
import { Component, ChangeDetectionStrategy, input, output, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-feature-card',
  templateUrl: './feature-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.active]': 'isActive()',
    '(click)': 'handleClick()',
  },
  imports: [MatButtonModule, MatIconModule, TranslocoDirective],
})
export class FeatureCardComponent {
  // Signal inputs
  item = input.required<Item>();
  disabled = input(false);

  // Signal outputs
  itemClick = output<Item>();

  // Computed
  readonly isActive = computed(() => this.item().status === 'Active');

  handleClick(): void {
    if (!this.disabled()) {
      this.itemClick.emit(this.item());
    }
  }
}
```

## Template Pattern (Control Flow)

```html
<div *transloco="let t">
  @if (loading()) {
    <mat-spinner diameter="40" />
  } @else {
    @for (item of items(); track item.id) {
      <app-item-card
        [item]="item"
        (itemClick)="onSelect($event)" />
    } @empty {
      <p class="text-slate-500">{{ t('common.noResults') }}</p>
    }
  }

  @switch (status()) {
    @case ('Active') { <span class="text-blue-600">{{ t('status.active') }}</span> }
    @case ('Completed') { <span class="text-green-600">{{ t('status.completed') }}</span> }
    @default { <span class="text-gray-600">{{ t('status.unknown') }}</span> }
  }
</div>
```

**Critical**: Always use `track` with `@for`. Prefer `track item.id`.

## NgRx Signal Store Pattern

```typescript
import { Injectable, inject, computed } from '@angular/core';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, exhaustMap } from 'rxjs';

type FeatureState = {
  items: ItemDto[];
  selectedItem: ItemDetailsDto | null;
  loading: boolean;
  saving: boolean;
  error: string | null;
};

const initialState: FeatureState = {
  items: [],
  selectedItem: null,
  loading: false,
  saving: false,
  error: null,
};

@Injectable()
export class FeatureStore extends signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    activeItems: computed(() => store.items().filter(i => i.status === 'Active')),
    hasItems: computed(() => store.items().length > 0),
  })),
  withMethods((
    store,
    service = inject(FeatureService),
    notification = inject(NotificationService),
  ) => ({
    // Queries use switchMap (cancels previous)
    loadItems: rxMethod<void>(
      pipe(
        switchMap(() => {
          patchState(store, { loading: true, error: null });
          return service.getItems().pipe(
            tap({
              next: (response) => patchState(store, { items: response.data, loading: false }),
              error: () => {
                patchState(store, { loading: false, error: 'Failed to load' });
                notification.notifyError('feature.errors.loadFailed');
              },
            })
          );
        })
      )
    ),

    // Mutations use exhaustMap (ignores while busy)
    createItem: rxMethod<CreateCommand>(
      pipe(
        exhaustMap((command) => {
          patchState(store, { saving: true, error: null });
          return service.create(command).pipe(
            tap({
              next: (response) => {
                patchState(store, { saving: false });
                notification.notifySuccess('feature.messages.created');
              },
              error: () => {
                patchState(store, { saving: false, error: 'Failed to create' });
                notification.notifyError('feature.errors.createFailed');
              },
            })
          );
        })
      )
    ),
  }))
) {}
```

## Service Pattern

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export interface CollectionResponse<T> {
  data: T[];
  total: number;
  page: number;
  size: number;
}

export interface ActionResponse {
  id: string;
  message: string;
  success: boolean;
}

@Injectable({ providedIn: 'root' })
export class FeatureService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/Features`;

  getItems(filters: Filters = {}): Observable<CollectionResponse<ItemDto>> {
    let params = new HttpParams();
    if (filters.page) params = params.set('page', filters.page.toString());
    if (filters.search) params = params.set('search', filters.search);
    return this.http.get<CollectionResponse<ItemDto>>(this.baseUrl, { params });
  }

  getItem(id: string): Observable<ItemDetailsDto> {
    return this.http.get<ItemDetailsDto>(`${this.baseUrl}/${id}`);
  }

  create(command: CreateCommand): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.baseUrl, command);
  }

  update(id: string, command: UpdateCommand): Observable<ActionResponse> {
    return this.http.put<ActionResponse>(`${this.baseUrl}/${id}`, { ...command, id });
  }

  delete(id: string): Observable<ActionResponse> {
    return this.http.delete<ActionResponse>(`${this.baseUrl}/${id}`);
  }
}
```

## RxJS + Signals Interop

### When to Use Each

| Scenario | Use |
|----------|-----|
| Local component/UI state | `signal()` |
| Derived synchronous value | `computed()` |
| Async streams (HTTP, WS, timer) | Observable → `toSignal()` for template |
| Complex async (cancellation, concurrency) | RxJS operators → signal |
| Cross-component broadcast | `Subject`/`BehaviorSubject` |

### Converting Observable → Signal

```typescript
// Do once at field level, never inside computed()
readonly userSig = toSignal(this.userService.user$, { initialValue: undefined });
readonly permissions = computed(() => derivePerms(this.userSig()));
```

### Flattening Operators

| Operator | Use For |
|----------|---------|
| `switchMap` | Queries, typeahead — cancels stale |
| `exhaustMap` | Mutations, form submit — ignores while busy |
| `concatMap` | Sequential writes — preserves order |
| `mergeMap` | Parallel independent calls |

### Signal-Friendly Service

```typescript
@Injectable({ providedIn: 'root' })
export class ItemsService {
  private readonly reload$ = new Subject<void>();
  private readonly items$ = this.reload$.pipe(
    startWith(void 0),
    switchMap(() => this.http.get<Item[]>('/api/items')),
    shareReplay({ bufferSize: 1, refCount: true })
  );
  readonly items = toSignal(this.items$, { initialValue: [] as Item[] });
  refresh() { this.reload$.next(); }
}
```

### Anti-Patterns
- Never call `toSignal()` inside `computed()` or `effect()` (NG0602)
- Never call `toSignal()` multiple times on same cold Observable (duplicates requests)
- Never mix `BehaviorSubject` + `signal()` for same state — pick one source
- Never use `mergeMap` where cancellation matters — use `switchMap`
- Never convert to Promise for template binding — use `toSignal()`

## Transloco i18n

Languages: `en.json`, `es.json` in `src/assets/i18n/`

**Always use the structural directive** (`*transloco="let t"`), never the pipe (`| transloco`).
Import `TranslocoDirective` (not `TranslocoModule` or `TranslocoPipe`).

```html
<div *transloco="let t">
  <h1>{{ t('projects.title') }}</h1>
  <button mat-flat-button>{{ t('common.save') }}</button>
</div>
```

When adding features, always add keys to BOTH translation files.

## Quality Checklist

Before submitting any component work, verify:

- [ ] `changeDetection: ChangeDetectionStrategy.OnPush`
- [ ] Template in separate `.html` file
- [ ] Signal inputs (`input()`, `input.required()`) — no `@Input()`
- [ ] Signal outputs (`output()`) — no `@Output()` / `EventEmitter`
- [ ] Semicolons in all TypeScript
- [ ] MD3 button syntax (`matButton="filled"`, etc.)
- [ ] Tailwind v4 classes (`shadow-xs`, not `shadow-sm`)
- [ ] `track` expression in every `@for`
- [ ] Transloco keys in both `en.json` and `es.json`
- [ ] ARIA labels on interactive elements
- [ ] `flex` + `gap-*` for layout (never `space-y`/`space-x`)
- [ ] Path aliases (`@core`, `@shared`) — no relative `../../`
- [ ] No `any` type
