# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Taskin 2.0 is an Angular 19 application built with Angular Material, focusing on task management with features like projects, tasks, and pomodoro tracking. The project uses standalone components and follows modern Angular patterns with signal-based inputs and outputs.

## Development Commands

### Core Commands

- `npm start` or `ng serve` - Start development server at http://localhost:4200/
- `npm run build` or `ng build` - Build the project (output to `dist/`)
- `npm run watch` or `ng build --watch --configuration development` - Build with watch mode
- `npm test` or `ng test` - Run unit tests via Karma

### Component Generation

When creating new components, use standalone mode:

```bash
ng g c modules/organizations/pages/component-name --standalone --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
```

## Architecture

### Project Structure

- **Core Module** (`src/app/core/`): Contains authentication, services, interceptors, and bootstrap logic
- **Features** (`src/app/features/`): Feature modules organized by domain (dashboard, pomodoros, projects, tasks)
- **Layout** (`src/app/layout/`): Main layout components (header, footer, sidenav, toolbar)
- **Theme** (`src/app/theme/`): UI theme components and styling
- **Shared** (`src/app/shared/`): Reusable components, directives, pipes, services

### Key Technologies

- Angular 19 with standalone components (standalone by default)
- Angular Material 19 for UI components
- NgRx Store for state management
- Transloco for i18n (supports en-US, es-ES, zh-CN, zh-TW)
- TailwindCSS for utility-first styling
- Karma/Jasmine for testing

### Path Aliases

The project uses TypeScript path aliases defined in `tsconfig.json`:

- `@core` → `src/app/core`
- `@shared` → `src/app/shared`
- `@theme` → `src/app/theme`
- `@env` → `src/environments`

### Component Patterns

- All new components should be standalone
- Use OnPush change detection strategy
- Components are organized by feature in dedicated `pages/` directories
- Shared components are placed in `src/app/shared/components/`

### Authentication & Services

- Authentication system with JWT tokens in `src/app/core/authentication/`
- HTTP interceptors for API calls, error handling, and token management
- Navigation service for dynamic menu management
- Settings and bootstrap services for app initialization

### Styling

- Material Design theme with Azure Blue preset
- Custom SCSS files organized in `src/styles/`
- Utility classes for spacing, alignment, colors, etc.
- TailwindCSS integration for rapid styling

### Internationalization

- Transloco for translations with JSON files in `src/assets/i18n/`
- Support for multiple languages with locale-specific formatting

## Application Status

### Completed Features

The application is fully implemented and functional with the following components:

#### Layout System

- **Responsive Layout** (`src/app/layout/layout/`): Main layout with sidebar, header, and content area
- **Sidebar Navigation** (`src/app/layout/sidenav/`): Collapsible sidebar with logo and navigation menu
- **Header Component** (`src/app/layout/header/`): Top header with user profile, language switcher, and mobile menu
- **Footer Component** (`src/app/layout/footer/`): Application footer with version info

#### Feature Modules

- **Dashboard** (`src/app/features/dashboard/`): Main dashboard with analytics cards and overview
- **Projects** (`src/app/features/projects/`): Project management with CRUD operations
- **Tasks** (`src/app/features/tasks/`): Task management system with status tracking
- **Pomodoros** (`src/app/features/pomodoros/`): Pomodoro timer and tracking functionality

#### Core Services

- **Navigation Service**: Dynamic menu management with role-based access
- **Bootstrap Service**: Application initialization and configuration
- **Authentication**: JWT-based authentication system (ready for backend integration)
- **HTTP Interceptors**: Error handling, loading states, and token management

#### UI/UX Implementation

- **Material Design**: Complete Angular Material integration with Azure Blue theme
- **Responsive Design**: Mobile-first design with TailwindCSS utilities
- **Logo Integration**: Custom logo display in sidebar (expanded/minimized states) and mobile header
- **Language Support**: Multi-language support with English and Spanish translations

### Assets and Static Files

- **Images**: Located in `src/assets/images/` (standard Angular convention)
  - `logo.png`: Main logo displayed in sidebar when expanded
  - `logo-icon.png`: Icon version for minimized sidebar and mobile header
  - `favicon-16x16.png`, `favicon-32x32.png`: Favicon files
- **Translations**: JSON files in `src/assets/i18n/` for en-US and es-ES
- **Icons**: Material Icons and custom SVG icon sets in `src/assets/icons/`

### Configuration Files

- **PWA Support**: `public/manifest.json` configured for installable web app
- **Favicon Configuration**: Proper favicon setup in `src/index.html`
- **Angular Assets**: Standard `src/assets/` configuration in `angular.json`

### Development Status

- ✅ **Initial Setup**: Angular 19 with standalone components
- ✅ **Layout Components**: Responsive layout system complete
- ✅ **Feature Modules**: All major features implemented
- ✅ **Routing**: Navigation and routing configured
- ✅ **Styling**: Material Design theme applied
- ✅ **Internationalization**: Multi-language support active
- ✅ **Logo Integration**: Proper logo display and favicon setup
- ✅ **Mobile Optimization**: Responsive design implemented
- ✅ **Angular 19 Migration**: Successfully upgraded from Angular 18 to Angular 19

### Next Steps for Production

1. **Backend Integration**: Connect authentication and data services to real API
2. **Testing**: Implement comprehensive unit and e2e tests
3. **Performance**: Optimize bundle size and implement lazy loading
4. **PWA Features**: Add offline support and push notifications
5. **Deployment**: Configure CI/CD pipeline and production build

### Angular 19 Signal-Based Patterns

#### Component Inputs and Outputs

**ALWAYS use `input()` and `output()` functions instead of decorators:**

```typescript
// ✅ CORRECT - Use input() and output() functions
import { input, output, signal } from '@angular/core';

@Component({...})
export class MyComponent {
  // Signal-based inputs (reactive by default)
  data = input<string>(''); // with default value
  config = input.required<Config>(); // required input
  disabled = input<boolean>(false);

  // Signal-based outputs
  valueChange = output<string>();
  save = output<void>();
  delete = output<string>();

  // Internal signals for component state
  readonly isLoading = signal(false);
  readonly selectedItems = signal<Item[]>([]);
}
```

```typescript
// ❌ INCORRECT - Don't use decorators
@Component({...})
export class MyComponent {
  @Input() data: string = '';
  @Input() disabled: boolean = false;
  @Output() valueChange = new EventEmitter<string>();
}
```

#### Benefits of Signal-Based Inputs

- **Reactive by Default** - Inputs are signals that can be used in computed() and effect()
- **Better Performance** - Automatic change detection optimization
- **Type Safety** - Better TypeScript inference and validation
- **Composition** - Easy to compose with other signals and computed values

#### Usage in Templates and Component Logic

```html
<!-- Signal inputs are called as functions in templates -->
<div [class.disabled]="disabled()">
  <span>{{ data() }}</span>
</div>
```

```typescript
// In component methods, always call signal inputs as functions
ngOnInit() {
  const configValue = this.config(); // ✅ CORRECT
  if (configValue) {
    this.processConfig(configValue);
  }

  // ❌ INCORRECT - Don't access signal inputs directly
  // if (this.config) { ... }
}
```

#### Reactive Patterns

```typescript
// Computed values based on inputs
readonly displayValue = computed(() => {
  return this.disabled() ? 'N/A' : this.data().toUpperCase();
});

// Effects for side effects
constructor() {
  effect(() => {
    if (this.disabled()) {
      console.log('Component is disabled');
    }
  });
}
```

### Environment Configuration

- **Environment Files** - Development and production configs
- **Build Configurations** - Multiple build targets (dev, prod)
- **Internationalization** - Translation files and font assets

### RxJS + Signals Interop Patterns

This section codifies how we use RxJS together with Angular 19 Signals. It is based on official Angular (signals, diagnostics, migrations) and RxJS documentation (creation, flattening, multicasting, promise interop) retrieved via context7.

#### When to Prefer Each Primitive

| Use                                                      | Prefer Signals                           | Prefer RxJS Observable / Subject         |
| -------------------------------------------------------- | ---------------------------------------- | ---------------------------------------- |
| Local component/UI state                                 | ✅                                       | ❌                                       |
| Derived synchronous value                                | ✅ computed()                            | ❌                                       |
| Async stream (HTTP, websocket, timer, user events)       | bridge to signal if consumed in template | ✅ native Observable                     |
| Cross-component broadcast / service push                 | sometimes (readonly)                     | ✅ Subject/BehaviorSubject/ReplaySubject |
| Complex async transformation (cancellation, concurrency) | feed result into signal                  | ✅ RxJS operators                        |

Guideline: Keep transformation & concurrency in RxJS, then expose a stable signal to templates (`toSignal`) to avoid manual subscribe/unsubscribe. Keep local mutation purely signal-based.

#### Converting Observables to Signals

Never call `toSignal()` inside a `computed` or another reactive context (Angular error NG0602). Create the signal once, then reference it:

```ts
// ✅ CORRECT
readonly userSig = toSignal(this.userService.user$ /* Observable<User> */, { initialValue: undefined });
readonly permissions = computed(() => derivePerms(this.userSig()));

// ❌ INCORRECT (inside computed())
// const permissions = computed(() => derivePerms(toSignal(this.userService.user$)()));
```

Prefer supplying `initialValue` so the signal is synchronous for first render.

#### Converting Signals to Observables

When a consumer still expects an Observable (e.g. a 3rd‑party library), convert with `toObservable(mySignal)` (Angular rxjs-interop). Limit this to boundary layers; do not wrap purely to reuse operators.

#### Flattening & Concurrency Patterns

Use the appropriate RxJS higher‑order mapping operator (official docs list: `mergeMap`, `switchMap`, `concatMap`, `exhaustMap`). Rule of thumb:

| Operator   | Use For                                        | Notes                                             |
| ---------- | ---------------------------------------------- | ------------------------------------------------- |
| switchMap  | HTTP queries, typeahead, cancel stale requests | Cancels previous inner observable on new emission |
| concatMap  | Ordered writes / sequential saves              | Buffers; preserves order                          |
| mergeMap   | Parallel independent calls                     | Configure concurrency if needed                   |
| exhaustMap | Ignore while busy (login button, form submit)  | Drops subsequent triggers until completion        |

Expose the final transformed stream as a signal:

```ts
private readonly searchTrigger = new Subject<string>();
readonly resultsSig = toSignal(
  this.searchTrigger.pipe(
    debounceTime(250),
    distinctUntilChanged(),
    switchMap(q => this.api.search(q)), // cancellation-safe
    shareReplay({ bufferSize: 1, refCount: true })
  ),
  { initialValue: [] as Result[] }
);

effect(() => {
  console.log('Results updated', this.resultsSig().length);
});
```

#### Multicasting & Caching

Prefer modern `share` / `shareReplay` (or `share({ connector: () => new ReplaySubject(1) })`) over deprecated `publish*` + `refCount`. Use when:

- The Observable performs expensive work (HTTP, CPU) you want to share
- Multiple subscribers (effects + `toSignal`) would otherwise duplicate side effects

```ts
readonly config$ = this.http.get<Config>('/api/config').pipe(
  shareReplay({ bufferSize: 1, refCount: true })
);
readonly configSig = toSignal(this.config$, { initialValue: undefined });
```

Avoid `BehaviorSubject` solely for template consumption; prefer `signal()`. Use `BehaviorSubject` only if:

- Legacy code already exposes it broadly
- External imperative producers push values from outside Angular zone
- You need synchronous current value for non-Angular consumers

#### Subject Selection

| Need                                           | Pick                     |
| ---------------------------------------------- | ------------------------ | --- |
| Broadcast events w/o stored last value         | Subject<void             | T>  |
| Store & emit current value to late subscribers | BehaviorSubject<T>       |
| Cache & replay a fixed number of past values   | ReplaySubject<T>(buffer) |
| Emit only final value on completion            | AsyncSubject<T>          |

Prefer `Subject<void>` for pure event signals (see RxJS doc example) and pair with effects:

```ts
readonly refresh$ = new Subject<void>();
readonly itemsSig = toSignal(
  this.refresh$.pipe(
    startWith(void 0),
    switchMap(() => this.api.loadItems()),
    shareReplay({ bufferSize: 1, refCount: true })
  ),
  { initialValue: [] as Item[] }
);
```

#### Bridging to Imperative Code (Promises)

Use `firstValueFrom()` / `lastValueFrom()` only at strict boundaries (e.g. in an `async` route resolver). Provide a `{ defaultValue }` for possibly empty streams to avoid rejection.

```ts
const initialData = await firstValueFrom(this.data$, { defaultValue: [] });
```

Do NOT convert to a Promise just for template binding—use a signal instead.

#### Memory & Cleanup

`toSignal(ob$, { initialValue })` handles subscription & cleanup automatically. If you manually subscribe (rare), always unsubscribe (e.g. `takeUntil(destroy$)` pattern). Prefer deriving everything via signals + effects.

Avoid nesting `toSignal` inside `effect` / `computed`; create once at field level.

#### Anti-Patterns

- ❌ Calling `toSignal` multiple times on the same cold HTTP Observable (duplicates requests). Create once, reuse.
- ❌ Mixing `BehaviorSubject` + signal for the same piece of state; pick one canonical source (usually the signal, unless external imperative writes dominate).
- ❌ Using `mergeMap` for request-response where cancellation matters (prefer `switchMap`).
- ❌ Using `shareReplay` without understanding cache invalidation (may serve stale data). Provide explicit refresh triggers.

#### Example: Service Providing Signal-Friendly API

```ts
@Injectable({ providedIn: 'root' })
export class ProjectsService {
  private readonly reload$ = new Subject<void>();

  private readonly projects$ = this.reload$.pipe(
    startWith(void 0),
    switchMap(() => this.http.get<Project[]>('/api/projects')),
    shareReplay({ bufferSize: 1, refCount: true })
  );

  // Public signal for components
  readonly projects = toSignal(this.projects$, { initialValue: [] as Project[] });

  refresh() {
    this.reload$.next();
  }
}
```

Component usage:

```ts
readonly projects = inject(ProjectsService).projects; // already a signal

trackById = (_: number, p: Project) => p.id;
```

Template:

```html
@for (p of projects(); track p.id) { <app-project-row [project]="p" /> }
```

#### Decision Flow

1. Is it async / event-based? Start with Observable operators.
2. Do templates need it? Convert once with `toSignal`.
3. Do other services need to transform further? Keep original Observable exported (suffix `$`) AND a signal for UI.
4. Need manual trigger? Model it as `Subject<void>` (event) + pipeline -> signal.

This ensures minimal subscriptions, clear ownership, and consistent reactive style across the codebase.
