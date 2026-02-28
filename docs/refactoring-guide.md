# Refactoring Guide -- Taskin 2.0

## Philosophy

Refactoring in Taskin 2.0 follows an **incremental** approach. No big-bang rewrites.
Every change should be small, reviewable, and backwards-compatible during the migration
window. The codebase already migrated from Angular 19 to 21, .NET 9 to 10, and
Tailwind CSS 3 to 4 -- all done in measured steps.

**Guiding principles**:
- One refactoring concern per commit.
- Keep the build green at every step.
- Update all consumers before removing old code.
- Prefer automated migrations (Angular schematics, `ng update`) when available.

---

## File Naming Migration

Angular 21 supports dropping the `.component` / `.service` / `.store` type suffix from
file names. Taskin 2.0 is migrating to the shorter convention incrementally.

### Naming Convention Table

| File Type | Old Convention | New Convention |
|---|---|---|
| Component | `task-card.component.ts` | `task-card.ts` |
| Component class | `TaskCardComponent` | `TaskCard` |
| Template | `task-card.component.html` | `task-card.html` |
| Styles | `task-card.component.scss` | `task-card.scss` |
| Service | `task.service.ts` | `task.service.ts` (keep for clarity) |
| Store | `task.store.ts` | `task.store.ts` (keep for clarity) |
| Guard | `auth.guard.ts` | `auth.guard.ts` (keep) |
| Interceptor | `error-interceptor.ts` | `error-interceptor.ts` (keep) |
| Pipe | `time-ago.pipe.ts` | `time-ago.ts` |
| Directive | `tooltip.directive.ts` | `tooltip.ts` |
| Types/Models | `task.types.ts` | `task.types.ts` (keep) |
| Validator (backend) | `CreateProjectCommandValidator.cs` | No change |

> **Note**: Services, stores, guards, and interceptors retain their suffix because the
> suffix carries important architectural meaning. Components, pipes, and directives
> drop the suffix since the `@Component` / `@Pipe` / `@Directive` decorator makes
> the type obvious.

### Migration Steps

1. **Create the new file** with the shortened name in the same directory.
2. **Move the content** from the old file to the new file. Update the class name if
   dropping the `Component` suffix.
3. **Update the `templateUrl` / `styleUrl`** inside the decorator to match the new
   file names.
4. **Update all imports** across the codebase that reference the old file path.
   Use your IDE's rename/refactor or a global find-and-replace.
5. **Delete the old file**.
6. **Update tests** (if any) that import or reference the old name.
7. **Verify the build**: `npm run build` must pass with zero errors.

### Example

Before:
```
features/projects/components/project-card/
  project-card.component.ts       (class: ProjectCardComponent)
  project-card.component.html
  project-card.component.scss
```

After:
```
features/projects/components/project-card/
  project-card.ts                 (class: ProjectCard)
  project-card.html
  project-card.scss
```

Updated decorator:
```typescript
@Component({
  selector: 'app-project-card',
  templateUrl: './project-card.html',
  styleUrl: './project-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectCard { /* ... */ }
```

Updated consumer imports:
```typescript
// Before
import { ProjectCardComponent } from './components/project-card/project-card.component';

// After
import { ProjectCard } from './components/project-card/project-card';
```

---

## Angular 21 Migration Patterns

These are the API migrations applicable to the Taskin 2.0 codebase. Each row shows
the legacy API and its modern replacement.

### Inputs and Outputs

| Legacy | Modern | Notes |
|---|---|---|
| `@Input() name: string` | `name = input.required<string>()` | Signal-based, required |
| `@Input() disabled = false` | `disabled = input(false)` | Signal-based, with default |
| `@Output() click = new EventEmitter<Item>()` | `itemClick = output<Item>()` | No `EventEmitter` needed |

```typescript
// Before
@Input() project!: ProjectViewModel;
@Input() disabled = false;
@Output() projectClick = new EventEmitter<ProjectViewModel>();

// After
project = input.required<ProjectViewModel>();
disabled = input(false);
projectClick = output<ProjectViewModel>();
```

### Host Bindings and Listeners

| Legacy | Modern |
|---|---|
| `@HostBinding('class.active') get isActive()` | `host: { '[class.active]': 'isActive()' }` |
| `@HostListener('click', ['$event'])` | `host: { '(click)': 'onClick($event)' }` |

```typescript
// Before
@HostBinding('class.active') get isActive() { return this.active; }
@HostListener('click') onClick() { this.toggle(); }

// After
@Component({
  host: {
    '[class.active]': 'isActive()',
    '(click)': 'onClick()',
  },
})
```

### Control Flow Syntax

| Legacy (directive) | Modern (built-in) |
|---|---|
| `*ngIf="condition"` | `@if (condition) { }` |
| `*ngIf="condition; else tmpl"` | `@if (condition) { } @else { }` |
| `*ngFor="let item of items"` | `@for (item of items; track item.id) { }` |
| `*ngFor` empty state | `@for (...) { } @empty { }` |
| `[ngSwitch]="value"` | `@switch (value) { @case ('A') { } }` |

```html
<!-- Before -->
<div *ngIf="loading; else content">
  <mat-spinner></mat-spinner>
</div>
<ng-template #content>
  <div *ngFor="let item of items; trackBy: trackById">
    {{ item.name }}
  </div>
</ng-template>

<!-- After -->
@if (loading()) {
  <mat-spinner diameter="40" />
} @else {
  @for (item of items(); track item.id) {
    <div>{{ item.name }}</div>
  } @empty {
    <p class="text-slate-500">No items found</p>
  }
}
```

**Critical**: Always use `track` with `@for`. Prefer `track item.id`.

### Dependency Injection

| Legacy | Modern |
|---|---|
| `constructor(private service: MyService)` | `private readonly service = inject(MyService)` |

```typescript
// Before
constructor(
  private projectService: ProjectService,
  private router: Router,
  private fb: FormBuilder
) {}

// After
private readonly projectService = inject(ProjectService);
private readonly router = inject(Router);
private readonly fb = inject(FormBuilder);
```

### Standalone Components

| Legacy | Modern |
|---|---|
| `NgModule` with `declarations` | Standalone component with `imports` array |
| `CommonModule` in imports | Not needed (control flow is built-in) |

```typescript
// Before
@NgModule({
  declarations: [ProjectListComponent],
  imports: [CommonModule, MatButtonModule, MatIconModule],
})
export class ProjectsModule {}

// After
@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButtonModule, MatIconModule, TranslocoDirective],
})
export class ProjectList {}
```

---

## Dead Code Detection

### Categories

| Category | Description | Severity |
|---|---|---|
| Unused imports | Import statements that are never referenced | Low |
| Unreachable code | Code after `return`, `throw`, or `break` | Medium |
| Commented-out blocks | Large blocks of `//` or `/* */` code | Low |
| Unused variables | Declared but never read | Medium |
| Unused methods | Private methods with zero callers | Medium |
| Deprecated API usage | `@Input()`, `*ngIf`, `NgModule`, etc. | Low |
| Dead routes | Routes pointing to removed/renamed components | High |
| Orphan translation keys | i18n keys in `en.json`/`es.json` with no template reference | Low |

### How to Find Dead Code

**TypeScript compiler** -- enable strict mode in `tsconfig.json`:
```json
{
  "compilerOptions": {
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true
  }
}
```

**ESLint rules** (`@typescript-eslint`):
```
@typescript-eslint/no-unused-vars
@typescript-eslint/no-unused-expressions
```

**Angular compiler warnings** -- the `ng build` output warns about:
- Unused standalone imports
- Missing `track` in `@for`
- Deprecated decorator usage

**IDE features** -- VS Code dims unreferenced imports and variables.
Use "Organize Imports" (`Shift+Alt+O`) to auto-remove unused imports.

**Manual search** -- for orphan files, search for files that are never imported:
```bash
# Find component files and check if they are imported anywhere
rg -l "ProjectCardComponent" ui/src/src/app/
```

---

## Tailwind CSS 3 to 4 Migration Notes

These patterns were already applied during the Taskin 2.0 migration and serve as
reference for any remaining or future cleanup.

| Tailwind v3 | Tailwind v4 | Notes |
|---|---|---|
| `tailwind.config.js` | `@theme { }` in CSS | Config moved to CSS |
| `@tailwind base/components/utilities` | `@import "tailwindcss"` | Single import |
| `shadow-sm` | `shadow-xs` | Class renamed |
| `@screen md { }` | `@media (width >= 768px) { }` | Directive removed |
| Component SCSS with `@apply` | Add `@reference "tailwindcss";` at top | Required for v4 |
| `postcss.config.js` | `.postcssrc.json` | `@angular/build` reads JSON |

---

## Pre-Refactoring Checklist

Run through this checklist **before** starting any refactoring work:

- [ ] Read the current code thoroughly -- understand what it does and why.
- [ ] Identify all usages and consumers of the code being changed.
  - Use IDE "Find All References" or `rg` to search the entire codebase.
- [ ] Check for tests that reference the old names/APIs.
- [ ] Plan the migration path -- can it be done in one commit or does it need
      a multi-step approach with backwards compatibility?
- [ ] Verify no breaking changes to the public API (inputs, outputs, route paths,
      HTTP endpoints).
- [ ] Confirm the Transloco keys in `en.json` and `es.json` are still valid after
      the rename.
- [ ] Check that `angular.json` does not reference the old file paths (e.g.,
      in `styles`, `assets`, or `fileReplacements`).

---

## Post-Refactoring Checklist

Run through this checklist **after** completing the refactoring:

- [ ] All imports updated -- no broken `import` statements.
- [ ] No broken references -- `templateUrl`, `styleUrl`, route `loadComponent`,
      and lazy-loaded paths all resolve.
- [ ] Frontend build passes: `npm run build` (from `ui/src/`).
- [ ] Backend build passes: `dotnet build` (from `back/src/`).
- [ ] Tests pass (if any): `npm test` / `dotnet test`.
- [ ] Translations still work -- all Transloco keys resolve in both `en.json`
      and `es.json`.
- [ ] No regressions in the UI -- visually inspect affected pages.
- [ ] No console errors in the browser dev tools.
- [ ] Lint passes: ESLint for frontend, `dotnet format` for backend.
- [ ] Git diff is clean -- no unintended changes outside the refactoring scope.

---

## Common Refactoring Scenarios

### Extracting a Shared Component

1. Identify the duplicated template/logic across features.
2. Create the component under `ui/src/src/app/shared/components/`.
3. Define signal inputs for all data the component needs.
4. Define signal outputs for all events the component emits.
5. Move the template markup and styles.
6. Replace the duplicated code in each feature with the new shared component.
7. Add the component to both feature imports.

### Splitting a Large Store

1. Identify orthogonal state slices (e.g., filters vs. CRUD vs. stats).
2. Create separate stores or use `withFeature` extensions.
3. Move relevant state, computed, and methods to the new store.
4. Update components to inject the correct store.
5. Verify that cross-store dependencies (if any) use `inject()` properly.

### Moving a Backend Handler to a New Feature Folder

1. Create the new folder structure under `Application/NewFeature/`.
2. Move the Command, Handler, Validator, and DTO files.
3. Update all `namespace` declarations.
4. Verify `AddValidatorsFromAssembly` still discovers the validators (it scans
   the assembly, not specific folders, so this usually just works).
5. Update any `typeof()` references used for assembly scanning.
6. Run `dotnet build` to confirm.

### Replacing a Class-Based Interceptor with a Functional One

```typescript
// Before (class-based)
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<unknown>, next: HttpHandler) { /* ... */ }
}

// After (functional)
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 404) {
        router.navigateByUrl('/404', { skipLocationChange: true });
      } else {
        notification.notifyError();
      }
      return throwError(() => error);
    })
  );
};

// Registration in app.config.ts
provideHttpClient(withInterceptors([errorInterceptor]))
```

---

## Tools and Commands Reference

| Task | Command |
|---|---|
| Frontend build check | `cd ui/src && npm run build` |
| Backend build check | `cd back/src && dotnet build` |
| Run frontend tests | `cd ui/src && npm test` |
| Run backend tests | `cd back/src && dotnet test` |
| Lint TypeScript | `cd ui/src && npx eslint src/` |
| Format C# files | `cd back/src && dotnet format` |
| Find unused exports | `npx ts-prune` (install separately) |
| Check circular deps | `npx madge --circular --extensions ts src/` |
| Angular schematic migration | `npx ng update @angular/core @angular/cli` |
| EF Core migration after entity changes | See `back/src/CLAUDE.md` for full command |
