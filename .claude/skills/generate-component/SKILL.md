---
name: generate-component
description: Generate an Angular 21 component with MD3, Tailwind v4, signals, and Transloco
user_invocable: true
---

# Generate Component Skill

Generate a complete Angular 21 standalone component following project conventions.

## Arguments

The user provides: `<feature-name>/<component-name>` (e.g., `projects/project-card`)

## Steps

1. **Parse arguments** to determine:
   - Feature area (e.g., `projects`)
   - Component name (e.g., `project-card`)
   - Full path: `ui/src/src/app/features/{feature}/{pages|components}/{component}/`

2. **Run Angular CLI** from `ui/src/`:
   ```bash
   npx ng g c features/{feature}/pages/{component} --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
   ```
   Do NOT add `--standalone` (default in Angular 21).

3. **Update the .ts file** with:
   - Signal inputs: `input()`, `input.required()`
   - Signal outputs: `output()`
   - `ChangeDetectionStrategy.OnPush`
   - `host` object if needed
   - Imports: Material modules, `TranslocoDirective`
   - Semicolons on all statements

4. **Create the template** (`.html` file) with:
   - `*transloco="let t"` wrapper
   - `@if` / `@for` (with `track`) / `@switch` control flow
   - Material Design 3 syntax
   - Tailwind v4 utility classes

5. **Add translation keys** to both:
   - `ui/src/src/assets/i18n/en.json`
   - `ui/src/src/assets/i18n/es.json`

6. **Verify build**:
   ```bash
   cd ui/src && npx ng build --configuration development 2>&1 | tail -5
   ```

## Component Template

```typescript
import { Component, ChangeDetectionStrategy, input, output, computed } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-{component-name}',
  templateUrl: './{component-name}.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslocoDirective],
})
export class {ComponentName}Component {
  // Define inputs/outputs based on user requirements
}
```

## Quality Verification

After generation, verify:
- [ ] `OnPush` change detection
- [ ] Template in separate `.html` file
- [ ] Signal inputs/outputs (no decorators)
- [ ] Semicolons
- [ ] Transloco keys in both language files
- [ ] Build succeeds
