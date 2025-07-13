# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Taskin 2.0 is an Angular 18 application built with Angular Material, focusing on task management with features like projects, tasks, and pomodoro tracking. The project uses standalone components and follows modern Angular patterns.

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
- Angular 18 with standalone components
- Angular Material 18 for UI components
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