# Code Style Conventions -- Taskin 2.0

Comprehensive code style, formatting, naming, and quality standards for the Taskin 2.0 project.
Automated tooling (Prettier, dotnet format) enforces most rules; developers should internalize them.

---

## 1. TypeScript Standards

### Strict Mode

All strict options enabled in `tsconfig.json`: `strict: true`, `noImplicitReturns`, `noFallthroughCasesInSwitch`,
`forceConsistentCasingInFileNames`. Angular compiler options: `strictTemplates`, `strictInjectionParameters`,
`strictInputAccessModifiers`. Never weaken with `// @ts-ignore` or `skipLibCheck`.

### Type Safety

- **No `any`**: Use `unknown` and narrow with type guards, `typeof`, or `instanceof`.
- **Explicit return types** on all public methods (services, stores, components).
- **Prefer type inference** for obvious cases: `const x = signal(false)` not `signal<boolean>(false)`.
- **Generic constraints**: `<T extends { id: string }>` over broad `unknown`.

### Nullable Types

Use `T | null` for optional state, not `T | undefined`. This keeps state explicit.

```typescript
type ProjectState = {
  selectedProject: ProjectDetailsDto | null;  // null = not loaded
  error: string | null;                       // null = no error
  loading: boolean;
};
```

### Interface vs Type

- **Interface**: Object shapes, DTOs, API contracts -- `interface ProjectListDto { ... }`.
- **Type alias**: Unions, intersections, state definitions -- `type ProjectStatus = 'Active' | 'Completed'`.
- Never prefix with `I`. Write `ProjectListDto`, not `IProjectListDto`.

### Imports

- **Path aliases** over relative paths: `@core`, `@shared`, `@theme`, `@env`, `@testing`.
- **Import order**: Angular framework, then third-party, then project (blank line between groups).

---

## 2. Formatting (Prettier)

Prettier is the single source of truth. Run it and move on.

| Option                       | Value      | Rationale                                     |
|------------------------------|------------|-----------------------------------------------|
| `semi`                       | `true`     | Semicolons required on all statements         |
| `singleQuote`                | `true`     | Single quotes for all strings                 |
| `printWidth`                 | `120`      | Line length limit                             |
| `tabWidth`                   | `2`        | 2-space indentation (TypeScript)              |
| `useTabs`                    | `false`    | Spaces only                                   |
| `trailingComma`              | `all`      | Trailing commas everywhere                    |
| `arrowParens`                | `always`   | Always wrap arrow params: `(x) => x`         |
| `bracketSpacing`             | `true`     | Spaces inside objects: `{ a: 1 }`            |
| `htmlWhitespaceSensitivity`  | `css`      | Respect CSS display for whitespace            |
| `endOfLine`                  | `auto`     | Auto-detect (LF on CI, CRLF on Windows)      |

Prettier runs automatically via PostToolUse hooks on `.ts`, `.html`, and `.scss` files.

```bash
npx prettier --write "src/**/*.ts"       # Format all TS
npx prettier --check "src/**/*.{ts,html,scss}"  # Check without writing
```

---

## 3. Naming Conventions

| Element                     | Convention                        | Example                              |
|-----------------------------|-----------------------------------|--------------------------------------|
| Component file (NEW)        | kebab-case, no suffix             | `task-card.ts`                       |
| Component class (NEW)       | PascalCase, no suffix             | `TaskCard`                           |
| Component file (existing)   | kebab-case`.component.ts`         | `task-card.component.ts`             |
| Component class (existing)  | PascalCase + `Component`          | `TaskCardComponent`                  |
| Component template          | kebab-case`.component.html`       | `task-card.component.html`           |
| Component styles            | kebab-case`.component.scss`       | `task-card.component.scss`           |
| Service                     | kebab-case`.service.ts`           | `project.service.ts`                 |
| Service class               | PascalCase + `Service`            | `ProjectService`                     |
| Store                       | kebab-case`.store.ts`             | `project.store.ts`                   |
| Store class                 | PascalCase + `Store`              | `ProjectStore`                       |
| Guard (functional)          | kebab-case`.guard.ts`             | `auth.guard.ts`                      |
| Interceptor (functional)    | kebab-case`.interceptor.ts`       | `api.interceptor.ts`                 |
| Pipe                        | kebab-case`.pipe.ts`              | `time-ago.pipe.ts`                   |
| Interface / DTO             | PascalCase, no `I` prefix         | `ProjectListDto`                     |
| Enum                        | PascalCase                        | `ProjectStatus`                      |
| Signal                      | camelCase                         | `loading`, `selectedProject`         |
| Observable                  | camelCase + `$`                   | `projects$`, `reload$`               |
| Constant                    | UPPER_SNAKE_CASE                  | `MAX_PAGE_SIZE`                      |
| Route path                  | kebab-case                        | `project-details`                    |
| Translation key             | dot-separated camelCase           | `projects.createDialog.title`        |
| Entity (C#)                 | PascalCase, singular              | `Project`, `Task`                    |
| Controller (C#)             | PascalCase + `Controller`         | `ProjectsController`                 |
| Command (C#)                | Verb + Entity + `Command`         | `CreateProjectCommand`               |
| Handler (C#)                | Verb + Entity + `CommandHandler`  | `CreateProjectCommandHandler`        |
| Validator (C#)              | Verb + Entity + `CommandValidator` | `CreateProjectCommandValidator`     |
| Query (C#)                  | `Get` + Entity + `Query`          | `GetProjectsQuery`                   |
| Query Handler (C#)          | `Get` + Entity + `QueryHandler`   | `GetProjectsQueryHandler`            |
| DTO (C#)                    | Entity + `List`/`Details` + `Dto` | `ProjectListDto`                     |
| EF Config (C#)              | Entity + `EntityTypeConfiguration`| `ProjectEntityTypeConfiguration`     |
| Migration (C#)              | Descriptive PascalCase            | `AddNotesFieldToTask`                |

**Booleans**: Name as questions/assertions -- `isLoading`, `hasItems`, `canEdit`.

---

## 4. Angular Component Organization

Member ordering within every component class:

```typescript
@Component({
  selector: 'app-project-card',
  templateUrl: './project-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButtonModule, MatIconModule, TranslocoDirective],
  host: { '[class.completed]': 'isCompleted()' },
})
export class ProjectCardComponent implements OnInit {
  // 1. Injected services
  private readonly store = inject(ProjectStore);
  private readonly router = inject(Router);

  // 2. Signal inputs
  project = input.required<ProjectListDto>();
  showActions = input(true);

  // 3. Signal outputs
  projectSelected = output<ProjectListDto>();

  // 4. Writable signals (local state)
  isExpanded = signal(false);

  // 5. Computed signals (derived state)
  readonly isCompleted = computed(() => this.project().status === 'Completed');

  // 6. Lifecycle hooks
  ngOnInit(): void { /* ... */ }

  // 7. Public methods (template-callable)
  onSelect(): void { this.projectSelected.emit(this.project()); }

  // 8. Private methods
  private formatDate(date: string): string { return new Date(date).toLocaleDateString(); }
}
```

**Metadata rules**: Always `OnPush`. Always separate `.html` template. Use `host: {}` for host bindings
(never `@HostBinding`/`@HostListener`). Selector prefixed with `app-`. Signal-based `input()`/`output()`
only (never `@Input()`/`@Output()`).

---

## 5. Template Conventions

### Native Control Flow

Use `@if`, `@for`, `@switch`, `@defer`, `@let`. Never use `*ngIf`, `*ngFor`, `[ngSwitch]`.

```html
@if (loading()) {
  <mat-spinner diameter="40" />
} @else if (error()) {
  <p class="text-red-600">{{ error() }}</p>
} @else {
  @for (project of projects(); track project.id) {
    <app-project-card [project]="project" />
  } @empty {
    <p class="text-slate-500">{{ t('projects.noResults') }}</p>
  }
}

@defer (on viewport) {
  <app-project-analytics [projectId]="projectId()" />
} @placeholder {
  <div class="h-64 bg-gray-50 rounded-xl animate-pulse"></div>
}

@let total = projects().length;
<p>Showing {{ total }} projects</p>
```

### Track Expressions

Every `@for` must have `track`. Prefer `track item.id`. Use `track $index` only when items lack IDs.

### Transloco

Wrap with `*transloco="let t"` directive. Import `TranslocoDirective` (never `TranslocoModule`/`TranslocoPipe`).
Always add keys to both `en.json` and `es.json`.

### Bindings

- Class: `[class.active]="isActive()"` (not `[ngClass]`)
- Style: `[style.width.%]="progress()"` (not `[ngStyle]`)
- Self-closing tags for void elements: `<mat-spinner diameter="40" />`, `<mat-divider />`

---

## 6. Observable / RxJS Patterns

### Flattening Operators

| Operator       | Use Case                   | Behavior                          |
|----------------|----------------------------|-----------------------------------|
| `switchMap`    | Queries, search, typeahead | Cancels previous inner stream     |
| `exhaustMap`   | Mutations, form submit     | Ignores while busy                |
| `concatMap`    | Sequential ordered writes  | Queues in order                   |
| `mergeMap`     | Parallel independent ops   | Runs all concurrently             |

### Search Pattern

Always combine `debounceTime(300)` + `distinctUntilChanged()` + `switchMap` for search inputs.

### Signal Interop Rules

- **`toSignal()`**: Call at field level only. Never inside `computed()` or `effect()` (causes `NG0602`).
- **`toObservable()`**: Use only at boundary layers bridging signals back to RxJS.
- **`shareReplay`**: Always use `{ bufferSize: 1, refCount: true }` for cached streams.

### Anti-Patterns

- Never call `toSignal()` inside `computed()` or `effect()`.
- Never call `toSignal()` multiple times on the same cold Observable.
- Never mix `BehaviorSubject` and `signal()` for the same state.
- Never use `mergeMap` where cancellation matters.
- Never convert Observable to Promise for template binding.
- Never subscribe manually when `rxMethod`, `toSignal`, or `async` pipe suffices.

---

## 7. C# Conventions

### File-Scoped Namespaces

Always `namespace X;` (not `namespace X { }`). Saves one indentation level.

### Primary Constructors

Use for DI in handlers and controllers:

```csharp
public class CreateProjectCommandHandler(
    ITaskinDbContext context,
    IUnitOfWork unitOfWork,
    TaskinMetrics metrics) : IRequestHandler<CreateProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Use context, unitOfWork, metrics directly
    }
}
```

### Property Modifiers

| Modifier   | When                                             | Example                                   |
|------------|--------------------------------------------------|-------------------------------------------|
| `required` | Mandatory properties                             | `public required string Name { get; set; }` |
| `init`     | Immutable after construction (IDs, timestamps)   | `public Guid Id { get; init; }`           |
| `set`      | Mutable properties                               | `public string? Description { get; set; }` |
| `string?`  | Optional/nullable reference types                | `public string? ImageUrl { get; set; }`    |

### Additional Rules

- **`sealed`** on all concrete entity classes (`sealed class Project : TrackedEntity`).
- **Records** for immutable DTOs: `public record ActionResponse(Guid Id, string Message)`.
- **Nullable reference types** enabled project-wide (`<Nullable>enable</Nullable>`).
- **Expression-bodied members** for single-expression methods: `public void Record() => _counter.Add(1);`
- **Pattern matching**: Prefer `is`, switch expressions over traditional casts.
- **Formatting**: 4-space indentation, 120-char line length, one type per file.

---

## 8. Performance Best Practices

### Frontend

- **OnPush** on ALL components -- Angular only checks when inputs change or events fire.
- **Lazy loading** for feature routes via `loadChildren`.
- **`track`** in every `@for` -- reuses existing DOM nodes instead of recreating.
- **`computed()`** for derived values -- memoized, recalculates only when dependencies change.
- **`@defer`** for heavy below-the-fold components (analytics, charts).
- **Virtual scrolling** for lists >100 items: `<cdk-virtual-scroll-viewport itemSize="48">`.
- **`NgOptimizedImage`** with explicit `width`/`height` to prevent layout shift.

### Backend

- **`AsNoTracking()`** for read-only queries -- disables change tracking overhead.
- **`Select()` projection** -- never load full entities when only a few fields are needed.
- **Pagination** on all list endpoints -- never return unbounded result sets.
- **`CancellationToken`** forwarded through every async call chain.
- **Compiled queries** for hot paths: `EF.CompileAsyncQuery(...)`.
- **Avoid N+1**: Use `.Include()` or `Select()` projection for related data.

---

## 9. Accessibility Standards

### ARIA Labels

Add `aria-label` to interactive elements without visible text:

```html
<button mat-icon-button aria-label="Delete project">
  <mat-icon>delete</mat-icon>
</button>
```

### Semantic HTML

Use `<nav>`, `<main>`, `<header>`, `<footer>`, `<section>`, `<article>` for document structure.
Screen readers rely on these landmarks for navigation.

### Keyboard Navigation

- All interactive elements focusable via Tab.
- Custom interactive elements: `tabindex="0"` + `(keydown.enter)` + `(keydown.space)`.
- Logical tab order: left-to-right, top-to-bottom. Never use `tabindex > 0`.
- Dialogs must trap focus.

### Color and Contrast

- WCAG 2.1 AA minimum: 4.5:1 for normal text, 3:1 for large text.
- Never convey information through color alone -- pair with icons or text.
- Focus indicators must remain visible. Never remove `outline` without an alternative.

### Images

- Descriptive `alt` text on informational images.
- `alt=""` + `role="presentation"` on decorative images.

---

## 10. Material CSS Token Rules

### Token Naming

Material tokens follow `--mat-{component}-{property}` or `--mdc-{component}-{property}`:

```
--mat-icon-color, --mat-card-container-color, --mat-menu-container-color
--mdc-filled-button-container-color, --mdc-snackbar-container-color
```

### Never Use `!important`

Override via design tokens, not brute force:

```css
/* Good */
html .mat-mdc-card { --mat-card-container-color: #ffffff; }

/* Bad */
.mat-mdc-card { background-color: #ffffff !important; }
```

### Tailwind v4 CSS Cascade

Tailwind v4 wraps utilities in `@layer utilities`. Unlayered CSS (Material) always wins over layered CSS.
Fix by adding unlayered overrides in `styles.css` with type+class specificity:

```css
mat-icon.text-blue-600 { color: var(--color-blue-600); }
app-sidebar nav a { color: var(--color-slate-300); }
```

### Theme Mixins and SCSS

Component SCSS using `@apply` needs `@reference "tailwindcss";` at top (Tailwind v4 requirement).
Custom theme tokens defined in `styles.css` under `@theme { --color-primary: #2563EB; }`.

### Snackbar and Dialog Variants

Set MDC tokens on variant CSS classes (unlayered) for colored snackbars and dialog buttons:

```css
.snackbar-success .mdc-snackbar__surface {
  --mdc-snackbar-container-color: var(--color-green-600);
  --mdc-snackbar-supporting-text-color: #fff;
}
button.confirm-btn-error {
  --mdc-filled-button-container-color: var(--color-red-600);
}
```

---

## Summary Checklist

### TypeScript / Angular

- [ ] `OnPush` on every component
- [ ] Template in separate `.html` file
- [ ] Signal `input()` / `output()` (no decorators)
- [ ] Semicolons, single quotes, no `any`
- [ ] Explicit return types on public methods
- [ ] Path aliases (`@core`, `@shared`)
- [ ] `track` in every `@for`
- [ ] Transloco keys in both `en.json` and `es.json`
- [ ] MD3 button syntax (`matButton="filled"`)
- [ ] Tailwind v4 (`shadow-xs`, `flex` + `gap-*`)
- [ ] ARIA labels, semantic HTML, keyboard nav
- [ ] `@defer` for heavy below-the-fold content

### C# / .NET

- [ ] File-scoped namespaces
- [ ] Primary constructor DI
- [ ] `required` / `init` / `sealed` as appropriate
- [ ] `AsNoTracking()` on reads, `Select()` projection
- [ ] `CancellationToken` in all async chains
- [ ] Every command has a FluentValidation validator
- [ ] POST returns `CreatedAtAction` (201)
- [ ] `{id:guid}` route constraints
- [ ] No logic in controllers
- [ ] Metrics on important operations
