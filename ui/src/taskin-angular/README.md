@@ -1,27 +1,27 @@

# Taskin 2.0

Standalone Angular application using Angular Material, CDK, TailwindCSS and Transloco.

## Stack

| Technology             | Version (package.json) | Purpose                             |
| ---------------------- | ---------------------- | ----------------------------------- |
| Angular Core           | ^19.2.x                | SPA framework (standalone, signals) |
| Angular Material / CDK | ^19.2.x                | UI components & a11y utilities      |
| TailwindCSS            | ^3.4.x                 | Utility‑first styling               |
| Transloco              | ^7.6.x                 | i18n (translations & lazy loading)  |
| NgRx Signals Store     | ^19.2.x                | Reactive state (signals)            |

> Short reference below summarizing docs gathered via context7 (Angular, Material, Tailwind, Transloco).

## Quick Start

1. Install deps: `pnpm i` or `npm i`.
2. Dev server: `npm start` (opens `http://localhost:4200/`).
3. Production build: `npm run build` (outputs to `dist/`).
4. Unit tests: `npm test`.

## Generate standalone components

Example (no spec, inline style, OnPush, no encapsulation):

```
ng g c features/sample-feature/components/sample-widget \
	--standalone --skip-tests --inline-style \
	--change-detection OnPush --view-encapsulation None
```

Other artifacts: `ng g directive|pipe|service|interface|enum` (add `--standalone` where relevant).

## Angular Material

Initial install (already applied):

```
ng add @angular/material
```

Minimal theming (SCSS):

```scss
@use '@angular/material' as mat;

html {
  color-scheme: light dark; // auto light/dark support
  @include mat.theme(
    (
      color: mat.$violet-palette,
      typography: Roboto,
      density: 0,
    )
  );
}
```

You can include per‑component mixins (e.g. `@include mat.button-theme($theme);`) if customizing M2/M3 themes. Keep Tailwind + Material tokens consistent (`tailwind.config.js`, `styles.scss`).

## TailwindCSS

Configured through PostCSS (see `devDependencies`). In `src/styles.scss` ensure Tailwind import:

```css
@import 'tailwindcss';
```

Usage in Angular templates:

```html
<h1 class="text-3xl font-bold underline">Hello Tailwind</h1>
```

Good practices:

- Prefer utilities + Material system CSS vars (`--mat-sys-*`) for visual consistency.
- Add custom utilities with layers (`@layer utilities { ... }`).

## Transloco (i18n)

Current translation file locations:

- `public/i18n/*.json` (e.g. `en-US.json`, `es-ES.json`)
- `src/assets/i18n/*.json` (e.g. `en.json`, `es.json`)

Pipe usage:

```html
{{ 'app.title' | transloco }} {{ 'greeting' | transloco:{ name: 'John' } }}
```

Structural directive (v2+ syntax):

```html
<ng-container *transloco="let t">
  <p>{{ t('title') }}</p>
  <p>{{ t('hello', { name: 'world' }) }}</p>
</ng-container>
```

Service usage:

```ts
constructor(private t: TranslocoService) {}
const msg = this.t.translate('errors.required');
```

## State (NgRx Signals)

Using `@ngrx/signals` for lightweight signal‑based state (see upstream docs for selector/effect patterns).

## Icons & Flags

- Flags: https://github.com/Yummygum/flagpack-core
- Material Symbols: https://fonts.google.com/icons?icon.set=Material+Symbols

## Scripts

| Script          | Action                     |
| --------------- | -------------------------- |
| `npm start`     | Dev server (HMR)           |
| `npm run build` | Production build           |
| `npm test`      | Unit tests (Karma/Jasmine) |

## Conventions

- Components always standalone.
- Default change detection: `OnPush`.
- View encapsulation: `None` (global theming + Tailwind utilities).
- Feature‑based structure under `src/app/features/*`.

## Suggested Next Steps

- Add architecture doc (features/core/shared diagram).
- Consolidate translation file location (choose `src/assets/i18n` or `public/i18n`).
- Add tests for critical services (auth, permissions).

## Quick References (context7)

- Angular CLI (new project): `ng new <name>`
- Material add: `ng add @angular/material`
- Multiple themes: use `@include mat.theme(...)` in different scopes (`html`, `.dark-mode`).
- Tailwind import: `@import "tailwindcss";`
- Transloco helper: `t('key', { param: 'value' })`

## Help

`ng help` or official guide: https://angular.dev/tools/cli
