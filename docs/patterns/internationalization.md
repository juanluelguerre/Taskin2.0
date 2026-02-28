# Internationalization (i18n) -- Taskin 2.0

## Overview

Taskin 2.0 uses [Transloco 8](https://jsverse.github.io/transloco) for internationalization
in the Angular 21 frontend. Transloco provides runtime translation loading, structural
directive-based template integration, and programmatic access via an injectable service.

The application is fully zoneless, so Transloco is configured with `reRenderOnLangChange: true`
to ensure the UI updates when the user switches languages.

---

## Supported Languages

| Code | Language | Status |
|------|----------|--------|
| `en` | English  | Default |
| `es` | Spanish  | Fully translated |

The default language is English. Users can switch languages at runtime via the header
language selector.

---

## Translation File Structure

Translation files are located at:

```
ui/src/src/assets/i18n/
  en.json    # English (default)
  es.json    # Spanish
```

### Format

Files use nested JSON with alphabetical key ordering within each level:

```json
{
  "common": {
    "cancel": "Cancel",
    "delete": "Delete",
    "edit": "Edit",
    "loading": "Loading...",
    "noResults": "No results found",
    "save": "Save",
    "search": "Search"
  },
  "projects": {
    "title": "Projects",
    "errors": {
      "loadFailed": "Failed to load projects."
    },
    "messages": {
      "created": "Project '{{name}}' created successfully."
    }
  }
}
```

### Key Organization

Top-level keys map to features or shared concerns:

| Key | Purpose |
|-----|---------|
| `common` | Shared labels (save, cancel, delete, edit, search, loading, etc.) |
| `menu` | Sidebar navigation labels |
| `header` | Header component strings |
| `dashboard` | Dashboard feature |
| `projects` | Projects feature |
| `tasks` | Tasks feature |
| `pomodoros` | Pomodoro timer feature |
| `validation` | Form validation messages |
| `forms` | Form-level messages (reset, validation summary) |
| `guards` | Route guard dialogs (unsaved changes) |
| `messages` | Global application messages |
| `paginator` | Material paginator labels |

---

## Transloco Configuration

Transloco is configured in `app.config.ts` via `provideTransloco`:

```typescript
import { provideTransloco } from '@jsverse/transloco';
import { TranslocoHttpLoader } from './transloco-loader';

provideTransloco({
  config: {
    availableLangs: ['en', 'es'],
    defaultLang: 'en',
    reRenderOnLangChange: true,
    prodMode: !isDevMode(),
  },
  loader: TranslocoHttpLoader,
});
```

The `TranslocoHttpLoader` fetches translation files from `/assets/i18n/{lang}.json`:

```typescript
@Injectable({ providedIn: 'root' })
export class TranslocoHttpLoader implements TranslocoLoader {
  private http = inject(HttpClient);

  getTranslation(lang: string) {
    return this.http.get<Translation>(`/assets/i18n/${lang}.json`);
  }
}
```

---

## Template Usage

**ALWAYS use the `TranslocoDirective`** (`*transloco="let t"`).
**NEVER use `TranslocoModule` or `TranslocoPipe`.**

### Basic Usage

```html
<div *transloco="let t">
  <h1>{{ t('projects.title') }}</h1>
  <p>{{ t('projects.subtitle') }}</p>
  <button mat-flat-button>{{ t('common.save') }}</button>
</div>
```

### With Interpolation Parameters

Use double curly braces inside the translation value and pass a params object:

```json
{
  "dashboard": {
    "welcome": "Welcome back, {{name}}! Here's your productivity overview."
  }
}
```

```html
<div *transloco="let t">
  <p>{{ t('dashboard.welcome', { name: userName() }) }}</p>
</div>
```

### With `ng-container` (No Extra DOM Element)

When you do not want to introduce an extra wrapper element:

```html
<ng-container *transloco="let t">
  @if (loading()) {
    <mat-spinner diameter="40" />
  } @else {
    <h2>{{ t('tasks.title') }}</h2>
  }
</ng-container>
```

### Component Import

Import `TranslocoDirective` in the component's `imports` array:

```typescript
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslocoDirective],
})
export class ProjectsComponent { }
```

---

## Component and Service Usage

For programmatic translations (snackbars, guards, dynamic strings), inject `TranslocoService`:

```typescript
import { TranslocoService } from '@jsverse/transloco';

export class NotificationService {
  private readonly translateService = inject(TranslocoService);

  notifySuccess(key: string, params?: Record<string, unknown>): void {
    const message = this.translateService.translate(key, params);
    this._snackBar.open(message, this.translateService.translate('common.dismiss'), {
      duration: 3000,
    });
  }
}
```

### Language Switching

```typescript
export class HeaderComponent {
  private readonly translocoService = inject(TranslocoService);

  switchLanguage(lang: string): void {
    this.translocoService.setActiveLang(lang);
  }

  get currentLang(): string {
    return this.translocoService.getActiveLang();
  }
}
```

---

## Scope Configuration

Taskin 2.0 uses a **shared (global) scope** for all translations. All keys live in the
root `en.json` and `es.json` files. There are no per-feature scoped translation files.

If a feature grows large enough to warrant its own scope, Transloco supports lazy-loaded
scoped translations. However, the current project keeps all translations in a single
file per language for simplicity.

---

## Translation Key Naming

### Convention

Keys follow the pattern: `feature.section.key`

| Pattern | Example | Usage |
|---------|---------|-------|
| `feature.key` | `projects.title` | Simple feature-level labels |
| `feature.section.key` | `tasks.form.notes` | Nested within a feature section |
| `feature.messages.key` | `projects.messages.created` | Success/info messages |
| `feature.errors.key` | `projects.errors.loadFailed` | Error messages |
| `common.key` | `common.save` | Shared across all features |
| `validation.key` | `validation.required` | Form validation messages |

### Rules

- Use **camelCase** for all key segments: `noResults`, not `no_results` or `no-results`.
- Keep keys **descriptive but concise**: `searchProjects`, not `searchFieldPlaceholderText`.
- Group related keys under a common parent: `projects.messages.*`, `projects.errors.*`.
- Sort keys **alphabetically** within each JSON object.

---

## Translation Workflow

When adding or modifying translations, follow these steps:

1. **Add the key to `en.json`** with the English text.
2. **Add the same key to `es.json`** with the Spanish translation.
3. **Maintain alphabetical order** within each JSON object level.
4. **Use the key in the template** via `t('feature.key')`.
5. **Test both languages** by switching in the UI to verify rendering.

### Adding a New Feature Section

```json
// en.json
{
  "reports": {
    "title": "Reports",
    "subtitle": "View your productivity analytics.",
    "messages": {
      "generated": "Report generated successfully."
    },
    "errors": {
      "loadFailed": "Failed to load report data."
    }
  }
}
```

```json
// es.json
{
  "reports": {
    "title": "Informes",
    "subtitle": "Consulta tus analíticas de productividad.",
    "messages": {
      "generated": "Informe generado exitosamente."
    },
    "errors": {
      "loadFailed": "Error al cargar los datos del informe."
    }
  }
}
```

---

## Common Translation Patterns

These keys are already defined and should be reused across features:

| Key | English | Spanish |
|-----|---------|---------|
| `common.save` | Save | Guardar |
| `common.cancel` | Cancel | Cancelar |
| `common.delete` | Delete | Eliminar |
| `common.edit` | Edit | Editar |
| `common.create` | Create | Crear |
| `common.search` | Search | Buscar |
| `common.loading` | Loading... | Cargando... |
| `common.noResults` | No results found | No se encontraron resultados |
| `common.refresh` | Refresh | Actualizar |
| `common.clear` | Clear | Limpiar |
| `common.viewDetails` | View Details | Ver Detalles |
| `common.duplicate` | Duplicate | Duplicar |
| `common.yes` | Yes | Si |
| `common.dismiss` | Dismiss | Descartar |
| `common.undo` | Undo | Deshacer |
| `common.active` | Active | Activo |
| `common.completed` | Completed | Completado |
| `common.overdue` | Overdue | Vencida |
| `common.progress` | Progress | Progreso |

---

## Validation Messages

Form validation translations follow the `validation.*` namespace with interpolation
parameters for dynamic values:

```json
{
  "validation": {
    "required": "This field is required",
    "min": "This value should be no less than {{number}}",
    "max": "This value should be no more than {{number}}",
    "min_length": "This value should be no less than {{number}} characters",
    "max_length": "This value should be no more than {{number}} characters",
    "invalid_email": "Invalid email"
  }
}
```

### Usage in Templates

```html
<div *transloco="let t">
  <mat-form-field>
    <mat-label>{{ t('projects.projectName') }}</mat-label>
    <input matInput formControlName="name" />
    @if (form.controls.name.hasError('required')) {
      <mat-error>{{ t('validation.required') }}</mat-error>
    }
    @if (form.controls.name.hasError('maxlength')) {
      <mat-error>{{ t('validation.max_length', { number: 100 }) }}</mat-error>
    }
  </mat-form-field>
</div>
```

---

## Date and Number Formatting

Angular's built-in pipes handle locale-aware formatting. The locale is determined by
the active Transloco language.

### Date Formatting

```html
<span>{{ dueDate() | date:'mediumDate' }}</span>
<span>{{ createdAt() | date:'short' }}</span>
```

### Number Formatting

```html
<span>{{ completionRate() | number:'1.0-1' }}%</span>
<span>{{ totalHours() | number:'1.1-2' }}</span>
```

Note: For full locale support, register the locale data in `app.config.ts` if needed:

```typescript
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';

registerLocaleData(localeEs, 'es');
```

---

## Troubleshooting

### Missing Translation Key

**Symptom**: The raw key string (e.g., `projects.title`) appears in the UI instead of
the translated text.

**Causes**:
- The key does not exist in the JSON file. Add it to both `en.json` and `es.json`.
- A typo in the key string. Check for exact case match (`camelCase`).
- The JSON file has a syntax error. Validate with a JSON linter.

### Translations Not Updating on Language Switch

**Symptom**: Changing the language does not update visible text.

**Causes**:
- `reRenderOnLangChange` is not set to `true` in the Transloco config.
- The component does not use the `*transloco` directive (static text will not update).
- The component uses `OnPush` but is not inside a `*transloco` directive scope.

### Wrong Directive or Module Used

**Symptom**: Compilation error or translations not rendering.

**Fix**: Import `TranslocoDirective` from `@jsverse/transloco`. Do not use:
- `TranslocoModule` (deprecated pattern)
- `TranslocoPipe` (not used in this project)

```typescript
// Correct
import { TranslocoDirective } from '@jsverse/transloco';

// Wrong
import { TranslocoModule } from '@jsverse/transloco';
import { TranslocoPipe } from '@jsverse/transloco';
```

### Scope Conflicts

**Symptom**: Keys resolve to `undefined` or the wrong value.

**Fix**: Taskin 2.0 uses global scope only. Do not pass a `scope` parameter to the
`*transloco` directive. Use the key directly:

```html
<!-- Correct -->
<div *transloco="let t">
  {{ t('projects.title') }}
</div>

<!-- Wrong - do not specify scope -->
<div *transloco="let t; scope: 'projects'">
  {{ t('title') }}
</div>
```

### Interpolation Parameters Not Replaced

**Symptom**: `{{name}}` appears literally in the rendered text.

**Fix**: Pass the params object as the second argument to the translation function:

```html
<!-- Correct -->
{{ t('projects.messages.created', { name: project.name }) }}

<!-- Wrong - missing params -->
{{ t('projects.messages.created') }}
```
