# Project Structure — Taskin 2.0

## Repository Layout

```
taskin2.0/
├── back/src/                                    # Backend (.NET 9)
│   ├── ElGuerre.Taskin.Api/                     # Web API Layer
│   │   ├── Controllers/                         # REST controllers
│   │   ├── Middleware/                           # Exception handling, logging
│   │   ├── Program.cs                           # App startup
│   │   └── appsettings.json                     # Configuration
│   ├── ElGuerre.Taskin.Application/             # Application Layer (CQRS)
│   │   ├── Projects/                            # Feature: Projects
│   │   │   ├── Commands/                        # Create, Update, Delete
│   │   │   ├── Queries/                         # GetAll, GetById, GetStats
│   │   │   └── DTOs/                            # List, Details DTOs
│   │   ├── Tasks/                               # Feature: Tasks
│   │   ├── Pomodoros/                           # Feature: Pomodoros
│   │   ├── Data/                                # ITaskinDbContext
│   │   └── Observability/                       # TaskinMetrics, TelemetryConstants
│   ├── ElGuerre.Taskin.Domain/                  # Domain Layer
│   │   ├── Entities/                            # Project, Task, Pomodoro
│   │   └── SeedWork/                            # Entity, TrackedEntity, IUnitOfWork
│   ├── ElGuerre.Taskin.Infrastructure/          # Infrastructure Layer
│   │   └── EntityFramework/
│   │       ├── EntityConfigurations/            # IEntityTypeConfiguration<T>
│   │       ├── Migrations/                      # EF Core migrations
│   │       └── TaskinDbContext.cs               # DbContext implementation
│   ├── Taskin2.0.AppHost/                       # .NET Aspire Orchestrator
│   │   └── AppHost.cs                           # Container orchestration
│   └── Taskin2.0.ServiceDefaults/               # Shared Observability
│       └── Extensions.cs                        # OpenTelemetry, health checks
│
├── ui/src/                                      # Frontend (Angular 21)
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                            # Singleton services
│   │   │   │   ├── authentication/              # Auth service, guards
│   │   │   │   ├── services/                    # Navigation, notification, bootstrap
│   │   │   │   └── interceptors/                # HTTP interceptors
│   │   │   ├── features/                        # Feature modules
│   │   │   │   ├── dashboard/                   # Dashboard + analytics
│   │   │   │   ├── projects/                    # Project CRUD
│   │   │   │   │   ├── pages/                   # Route components
│   │   │   │   │   │   ├── projects/            # List view
│   │   │   │   │   │   ├── project-details/     # Detail view
│   │   │   │   │   │   └── project-new/         # Create/edit form
│   │   │   │   │   ├── stores/                  # NgRx Signal Store
│   │   │   │   │   └── services/                # HTTP service + DTOs
│   │   │   │   ├── tasks/                       # Task management
│   │   │   │   └── pomodoros/                   # Pomodoro tracking
│   │   │   ├── layout/                          # App shell
│   │   │   │   ├── layout/                      # Main layout container
│   │   │   │   ├── header/                      # Top header bar
│   │   │   │   ├── sidenav/                     # Side navigation
│   │   │   │   └── footer/                      # Footer
│   │   │   └── shared/                          # Reusable
│   │   │       ├── components/                  # Shared UI components
│   │   │       ├── pipes/                       # Custom pipes
│   │   │       └── services/                    # Shared services
│   │   ├── assets/
│   │   │   ├── i18n/                            # en.json, es.json
│   │   │   ├── images/                          # Logo, icons
│   │   │   └── icons/                           # SVG icon sets
│   │   ├── environments/                        # Environment configs
│   │   └── styles/                              # SCSS modules
│   ├── styles.scss                              # Material theme entry
│   └── styles.css                               # Tailwind CSS v4 entry
│
├── deploy/                                      # Observability Configs
│   ├── grafana/                                 # Dashboards + provisioning
│   ├── prometheus/                              # Scrape config + alerts
│   ├── otel-collector/                          # OTLP pipeline config
│   ├── tempo/                                   # Trace storage config
│   ├── loki/                                    # Log aggregation config
│   └── alloy/                                   # Faro frontend telemetry
│
├── docs/                                        # Documentation
│   ├── architecture/                            # Architecture docs
│   └── patterns/                                # Code patterns
│
└── .claude/                                     # Claude Code Config
    ├── agents/                                  # 5 specialized agents
    ├── skills/                                  # 6 custom skills
    ├── settings.json                            # Shared permissions + hooks
    └── settings.local.json                      # Local overrides
```

## Architecture Diagrams

### Backend — Clean Architecture

```
┌─────────────────────────────────────────────┐
│                  API Layer                   │
│  Controllers → MediatR → Command/Query      │
├─────────────────────────────────────────────┤
│             Application Layer                │
│  Commands + Handlers + Validators            │
│  Queries + Handlers                          │
│  DTOs, Interfaces                            │
├─────────────────────────────────────────────┤
│               Domain Layer                   │
│  Entities, Enums, SeedWork                   │
│  (Zero external dependencies)                │
├─────────────────────────────────────────────┤
│           Infrastructure Layer               │
│  EF Core DbContext, Configurations           │
│  Repository implementations                  │
└─────────────────────────────────────────────┘
```

### Frontend — Feature Architecture

```
┌──────────────────────────────────────────┐
│            App Shell (Layout)             │
│  Header | Sidenav | Footer | Router      │
├──────────────────────────────────────────┤
│          Feature Modules                  │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ │
│  │Dashboard │ │Projects  │ │Tasks     │ │
│  │          │ │ Pages    │ │ Pages    │ │
│  │          │ │ Store    │ │ Store    │ │
│  │          │ │ Service  │ │ Service  │ │
│  └──────────┘ └──────────┘ └──────────┘ │
├──────────────────────────────────────────┤
│           Core Services                   │
│  Auth | Navigation | Notification        │
│  HTTP Interceptors | Bootstrap            │
├──────────────────────────────────────────┤
│          Shared Components                │
│  Pipes | Directives | Common UI           │
└──────────────────────────────────────────┘
```

### Infrastructure — Observability Stack

```
┌─────────────┐     ┌──────────────────┐
│  Angular UI  │────→│  .NET API        │
│  (Faro/Alloy)│     │  (OTLP exporter) │
└─────────────┘     └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │  OTel Collector   │
                    │  (4317/4318)      │
                    └──┬───────┬───┬───┘
                       │       │   │
              ┌────────▼──┐ ┌─▼─┐ ┌▼────────┐
              │ Prometheus │ │Tem│ │  Loki    │
              │ (metrics)  │ │po │ │  (logs)  │
              │ :9090      │ │:32│ │  :3100   │
              └────────┬───┘ │00 │ └──┬───────┘
                       │     └─┬─┘    │
                    ┌──▼───────▼──────▼──┐
                    │     Grafana         │
                    │     :3000           │
                    └─────────────────────┘
```

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Frontend Framework | Angular | 21.1.3 |
| UI Components | Angular Material | 21.1.3 |
| CSS Framework | Tailwind CSS | 4.1.18 |
| State Management | NgRx Signals | 21.0.1 |
| i18n | Transloco | 8.x |
| TypeScript | TypeScript | 5.9.3 |
| Backend Framework | ASP.NET Core | .NET 9 |
| ORM | Entity Framework Core | 9.x |
| CQRS | MediatR | Latest |
| Validation | FluentValidation | Latest |
| Logging | Serilog | Latest |
| Orchestration | .NET Aspire | Latest |
| Database | SQL Server | Latest |
| Cache | Redis | Latest |
| Metrics | Prometheus | v2.45.0 |
| Tracing | Grafana Tempo | 2.3.1 |
| Logs | Grafana Loki | 3.0.0 |
| Dashboards | Grafana | 10.3.3 |
| Telemetry | OpenTelemetry Collector | 0.139.0 |
