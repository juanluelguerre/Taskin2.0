# Taskin 2.0 - GitHub Copilot Instructions

**ALWAYS follow these instructions first and only fallback to additional search and context gathering if the information here is incomplete or found to be in error.**

## Project Overview

Taskin 2.0 is a full-stack productivity application implementing the Pomodoro Technique for task management:

- **Backend**: ASP.NET Core Web API (.NET 9) following Clean Architecture
- **Frontend**: Angular 19 application with Angular Material and standalone components
- **Architecture**: Clean separation with CQRS, Entity Framework Core, and signal-based state management

## Working Effectively

### Bootstrap, Build, and Test the Repository

**CRITICAL TIMING NOTE: NEVER CANCEL BUILD OR TEST COMMANDS. Set appropriate timeouts and wait for completion.**

#### Prerequisites Installation

Install .NET 9 SDK (required - system has .NET 8 by default):
```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.103 --install-dir /tmp/dotnet
export PATH="/tmp/dotnet:$PATH"
export DOTNET_ROOT="/tmp/dotnet"
```

#### Frontend (Angular 19) - Run from `/ui/src/taskin-angular/`

```bash
cd /ui/src/taskin-angular/

# Install dependencies - takes ~2 minutes. NEVER CANCEL. Set timeout to 5+ minutes.
npm install

# Development build - takes ~11 seconds. NEVER CANCEL. Set timeout to 2+ minutes.
npm run watch
# OR for single build:
# ng build --configuration development

# Start development server - takes ~11 seconds. NEVER CANCEL. Set timeout to 2+ minutes.
npm start
# Runs on http://localhost:4200

# Unit tests - CURRENTLY FAIL due to migration from ngx-translate to Transloco
# npm test -- --watch=false --browsers=ChromeHeadless
# Known issue: Missing dependencies cause test failures. Do not attempt to fix tests.
```

#### Backend (.NET 9) - Run from `/back/src/Taskin.Api/`

```bash
cd /back/src/Taskin.Api/

# Restore packages - takes ~15 seconds. NEVER CANCEL. Set timeout to 2+ minutes.
export PATH="/tmp/dotnet:$PATH" && export DOTNET_ROOT="/tmp/dotnet"
dotnet restore

# Build solution - takes ~9 seconds. NEVER CANCEL. Set timeout to 2+ minutes.
dotnet build

# Run API server - starts in ~2 seconds, includes database seeding
dotnet run --project ElGuerre.Taskin.Api
# Runs on http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
# Health check: http://localhost:5000/health

# No unit tests available - test projects not implemented
```

## Validation Scenarios

**ALWAYS run through these complete scenarios after making changes:**

### Frontend Validation
1. Start Angular dev server: `npm start`
2. Navigate to http://localhost:4200
3. Verify dashboard loads with Material Design UI
4. Test navigation: Dashboard → Projects → Tasks → Pomodoros
5. Verify sidebar navigation works and user profile displays

### Backend Validation  
1. Start API server: `dotnet run --project ElGuerre.Taskin.Api`
2. Test health endpoint: `curl http://localhost:5000/health` (should return "Healthy")
3. Test API data: `curl http://localhost:5000/api/Projects` (should return JSON with seeded projects)
4. Verify Swagger UI: http://localhost:5000/swagger (should load API documentation)

### Full Stack Integration
1. Start both frontend and backend servers
2. Angular app should connect to API (check browser network tab)
3. Navigate to Projects page - should attempt API calls to localhost:5001
4. Verify error handling when backend is unavailable

## Known Issues and Workarounds

### Google Fonts Network Blocking
- **Issue**: Production build (`npm run build`) fails with "ENOTFOUND fonts.googleapis.com"
- **Workaround**: Use development build (`npm run watch`) which works correctly
- **Do not attempt to fix**: This is expected in sandboxed environments

### Missing .NET 9 SDK
- **Issue**: "does not support targeting .NET 9.0" error
- **Solution**: Install .NET 9 SDK using commands above
- **Always export PATH**: Required for each session

### Angular Test Dependencies
- **Issue**: Tests fail with missing ngx-permissions, @ngx-translate/core, ngx-toastr
- **Root cause**: Migration from ngx-translate to Transloco incomplete
- **Do not fix**: Focus on application functionality, not test fixes

### API Connection Configuration
- Angular configured for: https://localhost:5001/api (backend default)
- Backend actually runs on: http://localhost:5000
- This mismatch is intentional for production vs development environments

## Common Commands Reference

### Directory Navigation
```bash
# Frontend
cd /ui/src/taskin-angular/

# Backend  
cd /back/src/Taskin.Api/
```

### Quick Status Checks
```bash
# Check .NET version
dotnet --version

# Check Node.js version
node --version

# Verify Angular CLI
npx ng version
```

### Build Outputs
- **Frontend**: `dist/` directory (development build)
- **Backend**: `bin/Debug/net9.0/` directories per project

## Project Structure

```
Taskin2.0/
├── back/src/Taskin.Api/           # .NET 9 Clean Architecture
│   ├── ElGuerre.Taskin.Api/       # Web API layer
│   ├── ElGuerre.Taskin.Application/ # CQRS with MediatR
│   ├── ElGuerre.Taskin.Domain/    # Domain entities
│   └── ElGuerre.Taskin.Infrastructure/ # EF Core data access
└── ui/src/taskin-angular/         # Angular 19 application
    ├── src/app/features/          # Feature modules
    ├── src/app/core/              # Core services & auth
    ├── src/app/shared/            # Reusable components
    └── src/app/layout/            # Layout components
```

## Key Features Implemented

### Frontend
- ✅ Angular 19 with standalone components
- ✅ Material Design with Azure Blue theme  
- ✅ Responsive layout with collapsible sidebar
- ✅ Multi-language support (English/Spanish)
- ✅ Signal-based state management
- ✅ Complete dashboard with analytics cards
- ✅ Project, Task, and Pomodoro management pages

### Backend
- ✅ Clean Architecture with CQRS
- ✅ Entity Framework Core with SQL Server LocalDB
- ✅ MediatR for command/query handling
- ✅ Swagger/OpenAPI documentation
- ✅ Health checks and logging
- ✅ Database seeding with sample data
- ✅ CORS configured for Angular frontend

## Always Remember

1. **Set long timeouts**: Build processes may take several minutes
2. **Install .NET 9 first**: Required for backend compilation
3. **Use development builds**: Production builds fail due to network restrictions
4. **Test both applications**: Ensure full-stack functionality
5. **Check API connectivity**: Verify endpoints return expected data
6. **Never fix unrelated tests**: Focus on core functionality validation

## Emergency Commands

If builds fail unexpectedly:
```bash
# Frontend cleanup
cd /ui/src/taskin-angular/
rm -rf node_modules package-lock.json
npm install

# Backend cleanup  
cd /back/src/Taskin.Api/
dotnet clean
dotnet restore
dotnet build
```

## Screenshots

Successfully running application:
![Angular Dashboard](https://github.com/user-attachments/assets/e701895f-442b-473f-bd2e-4e7f53678362)

---

**Last validated**: September 2025 with Angular 19 and .NET 9