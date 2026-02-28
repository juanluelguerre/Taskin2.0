# Error Handling Patterns -- Taskin 2.0

## Overview

Taskin 2.0 implements a full-stack error handling strategy that provides structured error
responses from the .NET backend and user-friendly feedback on the Angular frontend.
Errors flow through well-defined layers:

1. **Backend**: Custom exception hierarchy caught by `ErrorHandlingMiddleware`, serialized as
   JSON `Error` records with machine-readable codes.
2. **Frontend**: HTTP interceptor for global error routing, NgRx Signal Store per-method
   error state, and template-level error display with retry support.

---

## Frontend Error Handling

### HTTP Interceptor Pattern

The `ErrorInterceptor` (class-based `HttpInterceptor`) catches every `HttpErrorResponse`
globally. It routes to error pages for known status codes and shows toast notifications
for everything else.

**File**: `ui/src/src/app/core/interceptors/error-interceptor.ts`

```typescript
import { HttpErrorResponse, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export enum STATUS {
  UNAUTHORIZED = 401,
  FORBIDDEN = 403,
  NOT_FOUND = 404,
  INTERNAL_SERVER_ERROR = 500,
}

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastrService);

  private readonly errorPages = [STATUS.FORBIDDEN, STATUS.NOT_FOUND, STATUS.INTERNAL_SERVER_ERROR];

  private getMessage = (error: HttpErrorResponse) => {
    if (error.error?.message) return error.error.message;
    if (error.error?.msg) return error.error.msg;
    return `${error.status} ${error.statusText}`;
  };

  intercept(req: HttpRequest<unknown>, next: HttpHandler) {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => this.handleError(error))
    );
  }

  private handleError(error: HttpErrorResponse) {
    if (this.errorPages.includes(error.status)) {
      this.router.navigateByUrl(`/${error.status}`, { skipLocationChange: true });
    } else {
      console.error('ERROR', error);
      this.toast.error(this.getMessage(error));
      if (error.status === STATUS.UNAUTHORIZED) {
        this.router.navigateByUrl('/auth/login');
      }
    }
    return throwError(() => error);
  }
}
```

**Key behavior**:
- 403, 404, 500 navigate to dedicated error pages (without changing the URL).
- 401 redirects to the login page.
- All other errors show a toast with the server message.
- The error is always re-thrown so individual store/service callers can react.

### NotificationService

The `NotificationService` wraps `MatSnackBar` with Transloco i18n support. Stores call
it to show translated success, error, and warning messages.

**File**: `ui/src/src/app/core/services/notification.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly _snackBar = inject(MatSnackBar);
  private readonly _translateService = inject(TranslocoService);

  notifyError(message?: string, params?: Record<string, unknown>): void {
    const text = message
      ? this._translateService.translate(message, params)
      : this._translateService.translate('messages.error');
    this._open(text, 'snackbar-warn');
  }

  notifySuccess(message: string, params?: Record<string, unknown>): void {
    const text = this._translateService.translate(message, params);
    this._open(text, 'snackbar-success');
  }

  notifyWarning(message: string, params?: Record<string, unknown>): void { /* ... */ }
  notifyInfo(message: string, params?: Record<string, unknown>): void { /* ... */ }
  dismiss(): void { this._snackBar.dismiss(); }
}
```

### NgRx Signal Store Error Handling

Every store state type includes an `error: string | null` field. Error handling follows
a consistent pattern inside `rxMethod` operators:

1. **Reset error** at the start of each request: `patchState(store, { loading: true, error: null })`
2. **Catch in `tap({ error })`** and set the error message plus stop loading.
3. **Notify the user** via `NotificationService` with a Transloco key.

```typescript
type ProjectState = {
  projects: ProjectListDto[];
  loading: boolean;
  saving: boolean;
  error: string | null;       // <-- always present
};

// Inside withMethods:
loadProjects: rxMethod<void>(
  pipe(
    switchMap(() => {
      patchState(store, { loading: true, error: null });  // reset error

      return projectService.getProjects(filters).pipe(
        tap({
          next: (response) => {
            patchState(store, { projects: response.data, loading: false });
          },
          error: (error) => {
            patchState(store, { loading: false, error: 'Failed to load projects' });
            notificationService.notifyError('projects.errors.loadFailed');
            console.error('Load projects error:', error);
          },
        })
      );
    })
  )
),

// Mutations use exhaustMap (ignores while busy):
createProject: rxMethod<CreateProjectCommand>(
  pipe(
    exhaustMap((command) => {
      patchState(store, { saving: true, error: null });
      return projectService.createProject(command).pipe(
        tap({
          next: (response) => { /* ... */ },
          error: (error) => {
            patchState(store, { saving: false, error: 'Failed to create project' });
            notificationService.notifyError('projects.errors.createFailed');
          },
        })
      );
    })
  )
),

// Utility method to clear error from templates:
clearError: () => patchState(store, { error: null }),
```

**Operator choice**:
- `switchMap` for queries (cancels stale requests).
- `exhaustMap` for mutations (ignores while a request is in flight).

### Error State in Templates

Display error messages and retry buttons using the `@if` control flow:

```html
<div *transloco="let t">
  @if (store.error()) {
    <div class="flex items-center gap-3 rounded-xl border border-red-200 bg-red-50 p-4">
      <mat-icon class="text-red-600">error_outline</mat-icon>
      <span class="text-sm text-red-700">{{ store.error() }}</span>
      <button mat-flat-button
              class="ml-auto"
              (click)="store.clearError(); store.loadProjects()">
        {{ t('common.retry') }}
      </button>
    </div>
  }

  @if (store.loading()) {
    <mat-spinner diameter="40" />
  } @else {
    @for (project of store.projectViewModels(); track project.id) {
      <app-project-card [project]="project" />
    } @empty {
      <p class="text-slate-500">{{ t('projects.empty') }}</p>
    }
  }
</div>
```

### Form Validation Errors

Use `mat-error` inside `mat-form-field` with reactive form validators:

```html
<form [formGroup]="projectForm" (ngSubmit)="onSubmit()">
  <mat-form-field appearance="outline" class="w-full">
    <mat-label>{{ t('projects.form.name') }}</mat-label>
    <input matInput formControlName="name" required />
    @if (projectForm.get('name')?.hasError('required')) {
      <mat-error>{{ t('validation.required') }}</mat-error>
    }
    @if (projectForm.get('name')?.hasError('maxlength')) {
      <mat-error>{{ t('validation.maxLength', { max: 100 }) }}</mat-error>
    }
  </mat-form-field>

  <button matButton="filled" type="submit" [disabled]="projectForm.invalid || store.saving()">
    {{ t('common.save') }}
  </button>
</form>
```

```typescript
// In the component class:
private readonly fb = inject(FormBuilder);

readonly projectForm = this.fb.group({
  name: ['', [Validators.required, Validators.maxLength(100)]],
  description: ['', [Validators.maxLength(1000)]],
  status: ['Active'],
  dueDate: [null as Date | null],
});
```

---

## Backend Error Handling

### Exception Hierarchy

Taskin uses a custom exception hierarchy rooted in `TaskinExceptionBase`. All domain
exceptions implement `ITaskinException` which carries a machine-readable `Code`, a
human-readable `Message`, and optional `Values`.

```
ITaskinException (interface)
  TaskinExceptionBase (abstract, extends Exception)
    BusinessException
    EntityNotFoundException / EntityNotFoundException<T>
    EntityAlreadyExistsException / EntityAlreadyExistsException<T>
    KeyAlreadyExistsException
    ForbiddenEntityException
    UnauthorizedException
```

```csharp
// Interface
public interface ITaskinException
{
    string Code { get; }
    string Message { get; }
    IReadOnlyCollection<object> Values { get; }
}

// Base class (primary constructor)
public abstract class TaskinExceptionBase(
    string exceptionCode, string message, params object[] values)
    : Exception(message), ITaskinException
{
    public string Code { get; protected set; } = exceptionCode;
    public IReadOnlyCollection<object> Values { get; protected set; } = values;
}
```

### ErrorHandlingMiddleware

**File**: `back/src/ElGuerre.Taskin.Infrastructure/Middleware/ErrorHandlingMiddleware.cs`

The middleware wraps the entire HTTP pipeline. Known `TaskinExceptionBase` exceptions are
mapped to specific HTTP status codes. Unexpected exceptions return 500.

```csharp
public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await (ex is TaskinExceptionBase exception
                ? HandleTaskinException(context, exception)
                : HandleUnexpectedException(context, ex));
        }
    }

    private static int? GetStatusCode(Exception exception) => exception switch
    {
        UnauthorizedException          => StatusCodes.Status401Unauthorized,
        ForbiddenEntityException       => StatusCodes.Status403Forbidden,
        EntityNotFoundException        => StatusCodes.Status404NotFound,
        EntityAlreadyExistsException   => StatusCodes.Status409Conflict,
        KeyAlreadyExistsException      => StatusCodes.Status409Conflict,
        _                              => StatusCodes.Status400BadRequest,
    };
}
```

### Exception to Status Code Mapping

| Exception Type | HTTP Status | Code |
|---|---|---|
| `UnauthorizedException` | 401 Unauthorized | `UNAUTHORIZED` |
| `ForbiddenEntityException` | 403 Forbidden | `FORBIDDEN` |
| `EntityNotFoundException` / `EntityNotFoundException<T>` | 404 Not Found | `ENTITY_NOT_FOUND` |
| `EntityAlreadyExistsException` / `EntityAlreadyExistsException<T>` | 409 Conflict | `ENTITY_ALREADY_EXISTS` |
| `KeyAlreadyExistsException` | 409 Conflict | `KEY_ALREADY_EXISTS` |
| `BusinessException` | 400 Bad Request | Custom per domain rule |
| `ValidationException` (FluentValidation) | 400 Bad Request | Validation failures |
| Unhandled `Exception` | 500 Internal Server Error | `An error occurred during action handling` |

### Error Response Format

All errors are serialized as the `Error` record:

```csharp
public sealed record Error(
    string Code,
    string Description,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    IReadOnlyCollection<object>? Values = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("NULL_VALUE", "Null value was provided");
}
```

JSON example:
```json
{
  "code": "ENTITY_NOT_FOUND",
  "description": "Entity of type Project with id 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
}
```

### FluentValidation Pipeline

The `ValidationBehavior<TRequest, TResponse>` is registered as a MediatR pipeline behavior.
It runs **before** every handler, auto-validating commands that have registered validators.

```csharp
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }
        return await next();
    }
}
```

Validators are auto-discovered and registered via `InfrastructureExtensions`:

```csharp
services.AddValidatorsFromAssembly(typeof(CreateProjectCommand).Assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

Example validator:

```csharp
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Project name required.")
            .MaximumLength(100).WithMessage("The name cannot exceed 100 characters.");
    }
}
```

### Handler-Level Error Handling

Handlers throw typed exceptions rather than returning error objects. The middleware
translates these into the correct HTTP response.

```csharp
// Query handler: throw if entity not found
var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
if (project is null)
    throw new EntityNotFoundException<Project>(request.Id);

// Or use the static helper:
var project = EntityNotFoundException<Project>.ThrowIfNull(
    await context.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, ct),
    request.Id);

// Business rule violation:
if (project.Status == ProjectStatus.Completed)
    throw new BusinessException("PROJECT_CLOSED", "Cannot modify a completed project");

// Duplicate check:
EntityAlreadyExistsException<Project>.ThrowIfNotNull(
    await context.Projects.FirstOrDefaultAsync(p => p.Name == request.Name, ct),
    request.Name);
```

**When to throw vs. return**:
- **Throw** for precondition failures (not found, unauthorized, business rule violations).
  The middleware handles mapping to HTTP status codes.
- **Return** error information only in specialized scenarios where the caller needs
  partial success data (e.g., bulk operations returning per-item results).

---

## Best Practices

1. **Always log errors server-side with structured data (Serilog)**. The middleware logs
   `BusinessException` at Information level and unexpected exceptions at Error level.

2. **Never expose stack traces to the client**. The `Error` record contains only `Code`,
   `Description`, and optional `Values` -- no internal details.

3. **Use meaningful error codes**. Codes like `ENTITY_NOT_FOUND`, `ENTITY_ALREADY_EXISTS`
   allow the frontend to react programmatically.

4. **Translate error messages using Transloco keys**. Stores pass i18n keys like
   `'projects.errors.loadFailed'` to `NotificationService`, not raw English strings.

5. **Handle offline/network errors gracefully**. The interceptor catches `HttpErrorResponse`
   with `status === 0` (no network) and can show a connection-lost message.

6. **Provide retry mechanisms for transient failures**. Templates include a retry button
   that calls `store.clearError()` followed by the load method.

7. **Reset error state before new requests**. Every `rxMethod` starts with
   `patchState(store, { loading: true, error: null })` to clear stale errors.

8. **Use typed exceptions on the backend**. Never throw raw `Exception` -- always use
   a `TaskinExceptionBase` subclass so the middleware can map it correctly.

9. **Validate early with FluentValidation**. The `ValidationBehavior` pipeline rejects
   invalid commands before they reach the handler, keeping handler code clean.

10. **Console.error for debugging, NotificationService for users**. Stores log the raw
    error object to the console for developers and show translated messages to users.
