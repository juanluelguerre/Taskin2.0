# Task[in] 2.0

Task[in] 2.0 is a full-stack productivity application for managing projects, tasks, and Pomodoros using the Pomodoro Technique. Built with Angular 21 and .NET 10, following Clean Architecture principles, with a complete observability stack.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Development](#development)
- [API Endpoints](#api-endpoints)
- [Observability](#observability)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Project Management**: Create, read, update, and delete projects with status tracking and statistics.
- **Task Management**: CRUD operations for tasks within projects, with priority, deadline, and status tracking.
- **Pomodoro Timer**: Manage Pomodoros associated with tasks, tracking focus sessions and productivity.
- **Dashboard**: Overview of projects, tasks, and Pomodoro statistics.
- **CQRS with MediatR**: Command/Query separation for clean, testable business logic.
- **Full Observability**: OpenTelemetry, Prometheus, Grafana, Loki, Tempo, and Seq integration.
- **Internationalization**: English and Spanish language support via Transloco.

## Architecture

```
┌──────────────────────────────────────────────────┐
│                   Angular 21 SPA                 │
│   Material 21 + Tailwind CSS 4 + NgRx Signals   │
├──────────────────────────────────────────────────┤
│                REST API (HTTPS)                  │
├──────────────────────────────────────────────────┤
│              .NET 10 Web API                     │
│     Clean Architecture + CQRS/MediatR            │
├──────────────────────────────────────────────────┤
│  SQL Server  │  Redis  │  Seq  │  OTel Collector │
└──────────────────────────────────────────────────┘
```

**Backend layers:**

1. **ElGuerre.Taskin.Api** — Controllers, middleware, Program.cs
2. **ElGuerre.Taskin.Application** — CQRS commands, queries, handlers, validators, DTOs
3. **ElGuerre.Taskin.Domain** — Entities, enums, seed work (zero dependencies)
4. **ElGuerre.Taskin.Infrastructure** — EF Core DbContext, configurations, migrations

**Domain model:** `Project` → has many `Task` → has many `Pomodoro`

## Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend | Angular | 21.1.3 |
| UI Components | Angular Material (MD3) | 21.1.3 |
| CSS Framework | Tailwind CSS | 4.1.18 |
| State Management | NgRx Signals | 21.0.1 |
| i18n | Transloco | 8.x |
| TypeScript | TypeScript | 5.9.3 |
| Backend | .NET / ASP.NET Core | 10.0 |
| ORM | Entity Framework Core | 10.0.2 |
| CQRS | MediatR | 14.0.0 |
| Validation | FluentValidation | 11.x |
| Logging | Serilog | 10.0.0 |
| API Docs | Scalar | 2.12.36 |
| Orchestration | .NET Aspire | Latest |
| Database | SQL Server | 2022 |
| Cache | Redis | Latest |
| Telemetry | OpenTelemetry | 1.15.0 |
| Metrics | Prometheus | v2.45.0 |
| Dashboards | Grafana | 10.3.3 |
| Tracing | Tempo | 2.3.1 |
| Log Aggregation | Loki | 3.0.0 |
| Structured Logs | Seq | Latest |

## Project Structure

```
taskin2.0/
├── back/src/                              # Backend (.NET 10 Clean Architecture)
│   ├── ElGuerre.Taskin.Api/               # Controllers, middleware, Program.cs
│   ├── ElGuerre.Taskin.Application/       # CQRS commands/queries, handlers, validators
│   ├── ElGuerre.Taskin.Domain/            # Entities, enums, seed work
│   ├── ElGuerre.Taskin.Infrastructure/    # EF Core DbContext, configurations, migrations
│   ├── Taskin2.0.AppHost/                 # .NET Aspire orchestrator
│   └── Taskin2.0.ServiceDefaults/         # Shared observability config
├── ui/src/                                # Frontend (Angular 21)
│   └── src/app/
│       ├── core/                          # Auth, services, interceptors, guards
│       ├── features/                      # dashboard, projects, tasks, pomodoros
│       ├── layout/                        # header, sidenav, footer
│       └── shared/                        # Reusable components, pipes, directives
├── deploy/                                # Observability configs (Grafana, Prometheus, etc.)
└── docs/                                  # Architecture and pattern documentation
```

## Getting Started

### Prerequisites

- **.NET 10 SDK**: Download from [Microsoft .NET](https://dotnet.microsoft.com/download)
- **Node.js 22+**: Download from [Node.js](https://nodejs.org/)
- **SQL Server**: Install SQL Server or use the Aspire-managed container
- **Docker Desktop**: Required for .NET Aspire container orchestration

### Setup Instructions

1. **Clone the Repository**

   ```bash
   git clone https://github.com/elguerre/Taskin2.0.git
   cd Taskin2.0
   ```

2. **Backend Setup**

   ```bash
   cd back/src
   dotnet restore
   dotnet build
   ```

3. **Frontend Setup**

   ```bash
   cd ui/src
   npm install
   ```

4. **Database Setup**

   If using .NET Aspire (recommended), the database is provisioned automatically. Otherwise, update `appsettings.json` in `ElGuerre.Taskin.Api` and run migrations:

   ```bash
   cd back/src
   dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
   ```

## Development

### Running with .NET Aspire (Recommended)

Start the full stack (API + SQL Server + Redis + Seq) via Aspire:

```bash
cd back/src
dotnet run --project Taskin2.0.AppHost
```

The Aspire Dashboard will open automatically, showing all services and their health.

### Running Individually

**Backend API:**
```bash
cd back/src
dotnet run --project ElGuerre.Taskin.Api
# API: https://localhost:6001
# API Docs: https://localhost:6001/scalar/v1
# OpenAPI: https://localhost:6001/openapi/v1.json
```

**Frontend:**
```bash
cd ui/src
npm start
# App: http://localhost:4200
```

### Database Migrations

```bash
cd back/src

# Add a migration
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations

# Update the database
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

## API Endpoints

Base URL: `https://localhost:6001/api`

### Projects

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Projects` | List projects (page, size, search, status, sort, order) |
| GET | `/Projects/{id}` | Get project details |
| POST | `/Projects` | Create project |
| PUT | `/Projects/{id}` | Update project |
| DELETE | `/Projects/{id}` | Delete project |
| GET | `/Projects/stats` | Project statistics |

### Tasks

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Tasks?projectId={id}` | List tasks by project |
| GET | `/Tasks/{id}` | Get task details |
| POST | `/Tasks` | Create task |
| PUT | `/Tasks/{id}` | Update task |
| DELETE | `/Tasks/{id}` | Delete task |

### Pomodoros

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Pomodoros?taskId={id}` | List pomodoros by task |
| GET | `/Pomodoros/{id}` | Get pomodoro details |
| POST | `/Pomodoros` | Create pomodoro |
| PUT | `/Pomodoros/{id}` | Update pomodoro |
| DELETE | `/Pomodoros/{id}` | Delete pomodoro |

## Observability

### Core Services (Always Available)

| Service | URL | Purpose |
|---------|-----|---------|
| Seq | `http://localhost:5341` | Structured log search |
| Aspire Dashboard | Auto-opens | Service health and traces |

### Production Stack (Enable via `Observability:EnableProductionStack=true`)

| Service | URL | Purpose |
|---------|-----|---------|
| Grafana | `http://localhost:3000` | Dashboards (admin/admin) |
| Prometheus | `http://localhost:9090` | Metrics collection |
| Tempo | `http://localhost:3200` | Distributed tracing |
| Loki | `http://localhost:3100` | Log aggregation |

## Documentation

| Document | Description |
|----------|-------------|
| [Architecture](docs/architecture/project-structure.md) | Full project structure and dependency rules |
| [Code Style](docs/patterns/code-style.md) | TypeScript and C# conventions |
| [State Management](docs/patterns/state-management.md) | NgRx Signal Store patterns |
| [Internationalization](docs/patterns/internationalization.md) | Transloco i18n setup |
| [Error Handling](docs/patterns/error-handling.md) | Frontend and backend error patterns |
| [Refactoring Guide](docs/refactoring-guide.md) | Migration patterns and checklists |
| [Frontend Guide](ui/src/CLAUDE.md) | Angular 21 patterns and conventions |
| [Backend Guide](back/src/CLAUDE.md) | .NET 10 Clean Architecture patterns |

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add your feature"`
4. Push to your fork: `git push origin feature/your-feature`
5. Create a Pull Request to the `main` branch

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Angular](https://angular.dev) | [Angular Material](https://material.angular.dev)
- [.NET](https://dotnet.microsoft.com) | [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [MediatR](https://github.com/jbogard/MediatR) | [FluentValidation](https://docs.fluentvalidation.net)
- [NgRx Signals](https://ngrx.io/guide/signals) | [Transloco](https://jsverse.github.io/transloco)
- [Tailwind CSS](https://tailwindcss.com) | [Scalar](https://scalar.com)
- [Grafana](https://grafana.com) | [Prometheus](https://prometheus.io) | [OpenTelemetry](https://opentelemetry.io)
