# Code Style Conventions — Taskin 2.0

## TypeScript Conventions

### Formatting
- **Semicolons**: Required on all statements
- **Quotes**: Single quotes for strings
- **Line length**: 120 characters max
- **Indentation**: 2 spaces
- **Trailing commas**: Yes in multiline arrays/objects

### Types
- **No `any`**: Use `unknown` when type is uncertain
- **Prefer type inference** when type is obvious: `const x = signal(false)` not `const x = signal<boolean>(false)`
- **Explicit return types** on public methods
- **Interface over type** for object shapes (DTOs, state)
- **Nullable**: Use `T | null` not `T | undefined` for optional state

### Naming
- **Components**: PascalCase with `Component` suffix — `ProjectCardComponent`
- **Services**: PascalCase with `Service` suffix — `ProjectService`
- **Stores**: PascalCase with `Store` suffix — `ProjectStore`
- **Interfaces**: PascalCase, no `I` prefix — `ProjectListDto` not `IProjectListDto`
- **Signals**: camelCase — `loading`, `selectedProject`
- **Observables**: camelCase with `$` suffix — `projects$`, `reload$`
- **Constants**: UPPER_SNAKE_CASE — `MAX_PAGE_SIZE`

### Imports
- **Path aliases** over relative paths: `@core/services/notification.service` not `../../../core/services/notification.service`
- **Aliases**: `@core`, `@shared`, `@theme`, `@env`
- **Order**: Angular → third-party → project imports

## Angular Conventions

### Components
- **Standalone** — no NgModules
- **OnPush** — `changeDetection: ChangeDetectionStrategy.OnPush`
- **Zoneless** — no zone.js
- **Signal inputs** — `input()`, `input.required()` (never `@Input()`)
- **Signal outputs** — `output()` (never `@Output()` / `EventEmitter`)
- **Host object** — never `@HostBinding` / `@HostListener`
- **Separate templates** — `.html` files, never inline

### Templates
- **Native control flow** — `@if`, `@for`, `@switch`, `@defer`
- **Track expressions** — every `@for` must have `track item.id`
- **Class bindings** — `[class.active]="isActive()"` not `[ngClass]`
- **Style bindings** — `[style.opacity]` not `[ngStyle]`

### State Management
- **NgRx Signal Store** — `signalStore`, `withState`, `withComputed`, `withMethods`
- **rxMethod** — for async operations with RxJS
- **switchMap** — for queries (cancels previous)
- **exhaustMap** — for mutations (ignores while busy)

### i18n
- **Transloco** — `*transloco="let t"` wrapper
- **Both languages** — always add keys to `en.json` AND `es.json`

## C# Conventions

### Formatting
- **File-scoped namespaces** — `namespace X;` not `namespace X { }`
- **Primary constructors** — for handlers and controllers
- **Indentation**: 4 spaces
- **Braces**: K&R style (opening brace on same line for methods)

### Types
- **`required`** — on mandatory properties
- **`init`** — on immutable properties (Id, CreatedAt, collections)
- **Nullable reference types** — `string?` for optional
- **`sealed`** — on concrete entity classes
- **Records** — for immutable DTOs: `public record ActionResponse(Guid Id, string Message)`

### Naming
- **Entities**: PascalCase, singular — `Project`, `Task`
- **Controllers**: PascalCase with `Controller` suffix — `ProjectsController`
- **Commands**: `{Verb}{Entity}Command` — `CreateProjectCommand`
- **Handlers**: `{Verb}{Entity}CommandHandler`
- **Validators**: `{Verb}{Entity}CommandValidator`
- **Queries**: `Get{Entity}Query`, `Get{Entity}ByIdQuery`
- **DTOs**: `{Entity}ListDto`, `{Entity}DetailsDto`
- **EF Configs**: `{Entity}EntityTypeConfiguration`

### Architecture
- **Clean Architecture** — Domain has zero dependencies
- **CQRS** — every command has Command + Handler + Validator
- **No logic in controllers** — delegate everything to MediatR handlers
- **Primary constructor DI** — on handlers and controllers

## HTML Template Conventions

### Accessibility
- **ARIA labels** — on interactive elements without visible text
- **Semantic HTML** — `<nav>`, `<main>`, `<header>`, `<footer>`, `<section>`
- **Keyboard navigation** — all interactive elements focusable
- **Alt text** — on images

### Material Components
- **MD3 syntax** — `matButton="filled"`, `matButton="elevated"`, etc.
- **Mat form fields** — `<mat-form-field>` with `<mat-label>`
- **Icons** — `<mat-icon>name</mat-icon>`

### Tailwind CSS v4
- **Layout** — `flex` + `gap-*` (never `space-y` / `space-x`)
- **Shadows** — `shadow-xs` (not `shadow-sm`)
- **Responsive** — `md:`, `lg:` prefixes
- **Colors** — design system palette (slate, blue, green, etc.)
