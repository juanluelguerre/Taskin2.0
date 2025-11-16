# Telemetry Configuration with Aspire 13.0.0

This document summarizes the telemetry, logs, and traces configuration implemented in the Taskin 2.0 application using Aspire 13.0.0.

## Changes Made

### 1. NuGet Packages Added

#### API (ElGuerre.Taskin.Api)
- `Aspire.Microsoft.EntityFrameworkCore.SqlServer` 13.0.0
- `Aspire.StackExchange.Redis` 13.0.0

#### ServiceDefaults (Taskin2.0.ServiceDefaults)
- `OpenTelemetry.Instrumentation.EntityFrameworkCore` 1.14.0-beta.2
- `Aspire.Seq` 13.0.0
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` 1.14.0-beta.1

#### AppHost (Taskin2.0.AppHost)
- `Aspire.Hosting.SqlServer` 13.0.0
- `Aspire.Hosting.Redis` 13.0.0
- `Aspire.Hosting.Seq` 13.0.0

#### Infrastructure (ElGuerre.Taskin.Infrastructure)
- `Aspire.Microsoft.EntityFrameworkCore.SqlServer` 13.0.0
- `Microsoft.EntityFrameworkCore.SqlServer` 9.0.11
- `Microsoft.EntityFrameworkCore.Relational` 9.0.11
- `Microsoft.Extensions.Configuration.Abstractions` 9.0.11
- `Microsoft.Extensions.DependencyInjection` 9.0.11

### 2. Custom Telemetry Classes

#### TelemetryConstants.cs (`back/src/ElGuerre.Taskin.Api/Observability/`)
Defines constants for OpenTelemetry:
- `ActivitySource`: Activity source for distributed tracing
- `Meter`: Meter for custom metrics
- Names: "Taskin.Api" for both

#### TaskinMetrics.cs (`back/src/ElGuerre.Taskin.Api/Observability/`)
Custom business metrics:
- **Projects**: `taskin.projects.created`, `taskin.projects.deleted`, `taskin.projects.active`
- **Tasks**: `taskin.tasks.created`, `taskin.tasks.completed`, `taskin.tasks.deleted`, `taskin.tasks.active`
- **Pomodoros**: `taskin.pomodoros.created`, `taskin.pomodoros.completed`, `taskin.pomodoros.duration`

### 3. Updated ServiceDefaults

**File**: `back/src/Taskin2.0.ServiceDefaults/Extensions.cs`

#### OpenTelemetry Changes:
- **Tracing**:
  - Added `AddSource("Taskin.Api")` and `AddSource("Taskin.Application")`
  - Added `AddEntityFrameworkCoreInstrumentation()` with query enrichment

- **Exporters**:
  - **Seq**: Integration via connection string from AppHost
  - **Prometheus**: Configured to export metrics to `/metrics` endpoint

#### Mapped Endpoints:
- `/health`: Health checks
- `/alive`: Liveness probe
- `/metrics`: Prometheus metrics (when enabled)

### 4. Configured AppHost

**File**: `back/src/Taskin2.0.AppHost/AppHost.cs`

Resources added:
```csharp
// SQL Server with persistent volume
var sqlServer = builder.AddSqlServer("sql-server")
    .WithDataVolume("taskin-sql-data")
    .AddDatabase("taskin-db");

// Redis with persistent volume
var redis = builder.AddRedis("redis")
    .WithDataVolume("taskin-redis-data");

// Seq for structured logging
var seq = builder.AddSeq("seq")
    .WithDataVolume("taskin-seq-data");

// API with resource references
var api = builder.AddProject<Projects.ElGuerre_Taskin_Api>("taskin-api")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithReference(seq)
    .WithEnvironment("Prometheus:Enabled", "true");
```

### 5. Integration in Program.cs

**File**: `back/src/ElGuerre.Taskin.Api/Program.cs`

Main changes:
```csharp
// Aspire ServiceDefaults
builder.AddServiceDefaults();

// SQL Server DbContext with Aspire
builder.AddSqlServerDbContext<TaskinDbContext>("taskin-db");

// Custom metrics
builder.Services.AddSingleton<TaskinMetrics>();

// Custom activity sources
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(TelemetryConstants.ActivitySourceName))
    .WithMetrics(metrics => metrics.AddMeter(TelemetryConstants.MeterName));

// Aspire endpoints
app.MapDefaultEndpoints();
```

### 6. MediatR Instrumentation

**File**: `back/src/ElGuerre.Taskin.Application/Behaviors/LoggingBehavior.cs`

Added distributed tracing:
- Activity source: "Taskin.Application"
- Spans for each MediatR command/query
- Custom tags: `messaging.system`, `messaging.operation`, `messaging.destination`
- Error capture with `ActivityStatusCode.Error`

### 7. Database Configuration

- **Migrated from**: In-Memory Database
- **Migrated to**: SQL Server with Aspire integration
- Connection string managed automatically by Aspire
- Resource name: "taskin-db"

## ✅ Configuration Completed

All changes have been implemented and the application is ready to run with Aspire 13.0.0.

### Issues Resolved

1. **DbContext Pooling**: Removed `OnConfiguring` method that conflicted with Aspire pooling
2. **EF Core Versions**: Updated to 9.0.11 for compatibility with Aspire 13.0.0
3. **Initial Migration**: Successfully created (`InitialMigration`)
4. **Seeder**: Updated to use `MigrateAsync()` instead of `EnsureCreatedAsync()`

## Running the Application

### 3. Run with Aspire Dashboard
```bash
# Option 1: From Visual Studio
# Set Taskin2.0.AppHost as startup project
# Press F5

# Option 2: From CLI
cd back/src/Taskin2.0.AppHost
dotnet run
```

### 4. Access the Aspire Dashboard

Once running, the Aspire Dashboard will be available at:
- **URL**: `https://localhost:17281` (or configured port)
- **Available panels**:
  - **Resources**: View status of SQL Server, Redis, Seq, and API
  - **Logs**: Structured logs from all services
  - **Traces**: Distributed tracing with OpenTelemetry
  - **Metrics**: ASP.NET Core, runtime, and custom metrics

### 5. Verify Telemetry

#### a) Custom Metrics
Business metrics will be available in:
- Aspire Dashboard → Metrics → Taskin.Api
- Prometheus endpoint: `http://localhost:<api-port>/metrics`

Metrics to look for:
- `taskin_projects_created_total`
- `taskin_tasks_completed_total`
- `taskin_pomodoros_duration_seconds`

#### b) Distributed Traces
1. Execute operations in the API (create project, task, pomodoro)
2. Go to Aspire Dashboard → Traces
3. View spans from:
   - ASP.NET Core request
   - MediatR command/query handling
   - Entity Framework queries

#### c) Structured Logs
1. Aspire Dashboard → Logs (in-memory logs)
2. Seq Dashboard: `http://localhost:<seq-port>` (persistent logs)

### 6. Configure Seq (Optional)

To visualize logs in Seq:
1. Access Seq dashboard (URL shown in Aspire Dashboard)
2. Configure filters by:
   - `SourceContext` = "Taskin.Application"
   - `SourceContext` = "Taskin.Api"
   - Level = Warning/Error

### 7. Add Instrumentation to Handlers

To record business metrics in MediatR handlers, inject `TaskinMetrics`:

```csharp
public class CreateProjectCommandHandler
{
    private readonly TaskinMetrics _metrics;

    public CreateProjectCommandHandler(TaskinMetrics metrics)
    {
        _metrics = metrics;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        // ... create project

        _metrics.RecordProjectCreated();
        _metrics.IncrementActiveProjects();

        return projectDto;
    }
}
```

## Technologies Used

- **.NET Aspire 13.0.0**: Resource orchestration and observability
- **OpenTelemetry**: Open standard for telemetry
- **Serilog**: Structured logging (coexists with OpenTelemetry)
- **Seq**: Log storage and visualization
- **Prometheus**: Metric scraping and storage
- **SQL Server**: Relational database
- **Redis**: Distributed cache

## Application Endpoints

Once deployed, the application exposes:

- **API**: `https://localhost:<api-port>`
- **Swagger**: `https://localhost:<api-port>/swagger`
- **Health**: `https://localhost:<api-port>/health`
- **Liveness**: `https://localhost:<api-port>/alive`
- **Metrics (Prometheus)**: `https://localhost:<api-port>/metrics`

## Production Observability

For production, consider:

1. **Azure Application Insights**: Uncomment configuration in ServiceDefaults
   ```csharp
   if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
   {
       builder.Services.AddOpenTelemetry().UseAzureMonitor();
   }
   ```

2. **Grafana + Prometheus**: For advanced metric visualization

3. **Seq in production**: Configure ingestion keys and retention policies

## Troubleshooting

### Error: "AddSqlServerDbContext not found"
- Verify that `Aspire.Microsoft.EntityFrameworkCore.SqlServer` is installed
- Verify using: `using Microsoft.Extensions.Hosting;`

### Error: Package downgrade
- Ensure all projects use EF Core 9.0.11
- Update packages to version compatible with Aspire 13.0.0

### Files locked during build
- Stop the application if it's running
- Close Visual Studio
- Run `dotnet clean` before `dotnet build`

### Metrics don't appear
- Verify that `Prometheus:Enabled` is set to `true` in configuration
- Verify that `TaskinMetrics` is registered as Singleton
- Verify that the Meter is registered in OpenTelemetry

## References

- [Aspire 13.0 Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [Seq Documentation](https://docs.datalust.co/docs)
- [Prometheus](https://prometheus.io/docs/introduction/overview/)
