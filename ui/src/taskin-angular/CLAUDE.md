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
- ✅ **Initial Setup**: Angular 18 with standalone components
- ✅ **Layout Components**: Responsive layout system complete
- ✅ **Feature Modules**: All major features implemented
- ✅ **Routing**: Navigation and routing configured
- ✅ **Styling**: Material Design theme applied
- ✅ **Internationalization**: Multi-language support active
- ✅ **Logo Integration**: Proper logo display and favicon setup
- ✅ **Mobile Optimization**: Responsive design implemented

### Next Steps for Production
1. **Backend Integration**: Connect authentication and data services to real API
2. **Testing**: Implement comprehensive unit and e2e tests
3. **Performance**: Optimize bundle size and implement lazy loading
4. **PWA Features**: Add offline support and push notifications
5. **Deployment**: Configure CI/CD pipeline and production build