# Prometheus & Grafana Setup Guide for Taskin 2.0

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Quick Start](#quick-start)
4. [Configuration](#configuration)
5. [Dashboards](#dashboards)
6. [Metrics Reference](#metrics-reference)
7. [Alerting](#alerting)
8. [Troubleshooting](#troubleshooting)
9. [Production Deployment](#production-deployment)

---

## Overview

Taskin 2.0 includes a comprehensive observability stack using:
- **Prometheus**: Metrics collection and storage
- **Grafana**: Metrics visualization and dashboards
- **OpenTelemetry**: Instrumentation and metrics export
- **.NET Aspire**: Orchestration and service management

### Features
- ✅ Automatic metrics collection from ASP.NET Core API
- ✅ Custom business metrics (Projects, Tasks, Pomodoros)
- ✅ Infrastructure metrics (.NET runtime, GC, memory)
- ✅ Database performance metrics (Entity Framework)
- ✅ Pre-built Grafana dashboards
- ✅ Alert rules for critical issues
- ✅ Integrated with .NET Aspire AppHost

---

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                 Taskin 2.0 Stack                    │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌──────────────┐         ┌──────────────┐         │
│  │  Taskin API  │────────▶│  Prometheus  │         │
│  │   (.NET 9)   │ /metrics│   (v2.45.0)  │         │
│  └──────────────┘         └──────┬───────┘         │
│         │                        │                  │
│         │                        │ scrapes          │
│         │                        │                  │
│         │                 ┌──────▼───────┐         │
│         │                 │   Grafana    │         │
│         └────references───│  (v10.0.0)   │         │
│                           └──────────────┘         │
│                                                     │
│  Infrastructure:                                    │
│  • SQL Server    • Redis    • Seq                   │
└─────────────────────────────────────────────────────┘
```

---

## Quick Start

### Prerequisites
- .NET 9 SDK installed
- Docker Desktop running
- .NET Aspire workload installed

### Starting the Stack

1. **Navigate to AppHost directory:**
   ```bash
   cd back/src/Taskin2.0.AppHost
   ```

2. **Run the Aspire AppHost:**
   ```bash
   dotnet run
   ```

3. **Access the services:**
   - Aspire Dashboard: http://localhost:15000
   - Taskin API: http://localhost:5000 (check Aspire dashboard for actual port)
   - Prometheus: http://localhost:9090
   - Grafana: http://localhost:3000
   - Metrics Endpoint: http://localhost:5000/metrics

4. **Login to Grafana:**
   - Username: `admin`
   - Password: `admin`

### Verify Everything Works

1. **Check Prometheus targets:**
   - Go to http://localhost:9090/targets
   - Verify `taskin-api` is UP

2. **View Grafana dashboards:**
   - Go to http://localhost:3000
   - Navigate to Dashboards → Taskin 2.0 folder
   - Open "Taskin - Business Metrics"

3. **Test metrics collection:**
   - Create a project via API
   - Wait 15-30 seconds
   - Check dashboard for updated metrics

---

## Configuration

### Environment-Specific Configuration

#### Development (default)
- File: `deploy/prometheus/prometheus.yml`
- Scrape interval: 15 seconds
- Retention: 15 days
- Authentication: Disabled

#### Production
- File: `deploy/prometheus/prometheus.prod.yml`
- Scrape interval: 30 seconds
- Retention: 30 days
- Authentication: Enabled

### Switching Configurations

To use production config, update `AppHost.cs`:
```csharp
.WithBindMount(Path.Combine(deployPath, "prometheus", "prometheus.prod.yml"), "/etc/prometheus/prometheus.yml")
```

### Customizing Scrape Targets

Edit `deploy/prometheus/prometheus.yml`:
```yaml
scrape_configs:
  - job_name: 'taskin-api'
    metrics_path: '/metrics'
    scrape_interval: 10s
    static_configs:
      - targets: ['taskin-api:8080']
```

---

## Dashboards

Taskin 2.0 includes a comprehensive set of dashboards organized into folders for easy navigation. The **Taskin 2.0 - Overview** dashboard is set as the default home dashboard.

### Dashboard Organization

Dashboards are organized into the following folders:

1. **General (Root)** - Overview dashboard (default home)
2. **Backend - ASP.NET Core** - Official .NET dashboards from Grafana.com
3. **Frontend - Angular** - Frontend RUM monitoring
4. **Business Metrics** - Custom Taskin business dashboards

### Overview Dashboard (Default Home)

**Dashboard:** Taskin 2.0 - Overview
**UID:** `taskin-overview`
**Location:** Root (opens by default)

The main entry point showing:
- **System Health**: API success rate, latency P95, request rate
- **Business Metrics**: Active projects, tasks, and pomodoros
- **Infrastructure**: Memory usage, GC collections
- **Quick Links**: Navigation to all specialized dashboards, logs, and monitoring tools

**Navigation Features:**
- Top navigation bar with quick links to detailed dashboards
- Direct links to Prometheus and Seq logs
- Panels with drill-down capabilities

---

### Backend Dashboards (ASP.NET Core)

Located in **Backend - ASP.NET Core** folder:

#### 1. ASP.NET Core (Official)
**Source:** Grafana.com Dashboard #19924
**Requirements:** .NET 8+, OpenTelemetry

Key metrics:
- Request duration percentiles (p50-p99.9)
- Error rate monitoring (4XX, 5XX)
- Active connections and requests
- Unhandled exceptions
- Protocol analysis (HTTP/1.1, HTTP/2, HTTP/3)
- Top requested/errored endpoints

#### 2. ASP.NET Core Endpoint (Official)
**Source:** Grafana.com Dashboard #19925
**Requirements:** .NET 8+, OpenTelemetry

Endpoint-specific analysis:
- Per-endpoint request duration
- Error rates by endpoint
- Security status (secured vs unsecured)
- HTTP status code distribution
- Exception tracking per endpoint

#### 3. .NET Runtime Metrics (Official)
**Source:** Grafana.com Dashboard #23179

Runtime performance monitoring:
- Memory management (working set, heap, generations)
- Garbage collection metrics
- Threading and thread pool
- JIT compilation
- Exception handling
- Assembly loading

#### 4. OpenTelemetry APM (Official)
**Source:** Grafana.com Dashboard #19419

Application Performance Monitoring:
- Distributed tracing metrics
- Span metrics from OpenTelemetry
- Service dependencies
- Latency distribution
- Error tracking across services

---

### Frontend Dashboards (Angular)

Located in **Frontend - Angular** folder:

#### Grafana Faro - Frontend Monitoring (Official)
**Source:** Grafana.com Dashboard #17766
**Requirements:** Grafana Faro Web SDK

Real User Monitoring (RUM):
- Web Vitals (LCP, FID/INP, CLS, TTFB)
- Page load performance
- User interactions tracking
- Frontend errors and exceptions
- Session tracking
- Browser and device metrics

**Note:** Requires Grafana Faro SDK implementation in Angular (see Frontend Integration section).

---

### Custom Business Dashboards

Located in **Business Metrics** folder:

#### 1. Taskin - Business Metrics
**UID:** `taskin-business-metrics`

Application-level business metrics:
- Projects: Creation/deletion rate, active count
- Tasks: Activity trends, completion rate
- Pomodoros: Completion rate, duration distribution
- Weekly summaries and trends

**Best for:** Product managers, stakeholders

#### 2. Taskin - API Performance
**UID:** `taskin-api-performance`

HTTP and API health monitoring:
- Request rate and success rate
- Latency percentiles (P50, P95, P99)
- Per-endpoint performance
- HTTP status code distribution
- Error rate tracking

**Best for:** Developers, DevOps engineers

#### 3. Taskin - Infrastructure
**UID:** `taskin-infrastructure`

System-level .NET metrics:
- Process memory (working set, private, virtual)
- GC heap size by generation
- GC collection rate
- Thread count and ThreadPool metrics
- CPU usage
- Database query performance

**Best for:** DevOps engineers, SREs

---

### Accessing Dashboards

#### Default Access
When you open Grafana at http://localhost:3000, the **Taskin 2.0 - Overview** dashboard loads automatically as the home dashboard.

#### Navigation Methods

1. **Via Folder Browser:**
   - Click "Dashboards" in the left sidebar
   - Browse folders: Backend - ASP.NET Core, Frontend - Angular, Business Metrics

2. **Via Search:**
   - Press `/` or click search icon
   - Type dashboard name
   - Use tags: `taskin`, `aspnetcore`, `frontend`, `business`

3. **Via Dashboard Links:**
   - Overview dashboard has quick links to all specialized dashboards
   - Dashboard top navigation bar provides cross-linking

4. **Direct URLs:**
   - Overview (Home): http://localhost:3000/d/taskin-overview
   - ASP.NET Core: http://localhost:3000/d/aspnet-core
   - ASP.NET Endpoints: http://localhost:3000/d/aspnet-core-endpoint
   - .NET Runtime: http://localhost:3000/d/dotnet-runtime
   - OpenTelemetry APM: http://localhost:3000/d/opentelemetry-apm
   - Faro Frontend: http://localhost:3000/d/faro-frontend
   - Business Metrics: http://localhost:3000/d/taskin-business-metrics
   - API Performance: http://localhost:3000/d/taskin-api-performance
   - Infrastructure: http://localhost:3000/d/taskin-infrastructure

---

### Dashboard Navigation Features

#### Data Links to Logs
All dashboards include data links that allow you to:
- Click on any metric and select "View logs in Seq"
- Automatically filter logs by the same time range
- Navigate to Prometheus for detailed metric exploration

#### Explore Integration
- Click "Explore" on any panel to open Grafana Explore view
- Compare metrics side-by-side
- Build complex queries interactively

#### Cross-Dashboard Linking
Dashboards include:
- Top navigation links to related dashboards
- Panel titles link to detailed views
- Variables sync across dashboards for consistent filtering

---

## Metrics Reference

### Business Metrics

#### Projects
| Metric Name | Type | Description |
|-------------|------|-------------|
| `taskin_projects_created_total` | Counter | Total projects created |
| `taskin_projects_deleted_total` | Counter | Total projects deleted |
| `taskin_projects_active` | UpDownCounter | Current active projects |

#### Tasks
| Metric Name | Type | Description |
|-------------|------|-------------|
| `taskin_tasks_created_total` | Counter | Total tasks created |
| `taskin_tasks_completed_total` | Counter | Total tasks completed |
| `taskin_tasks_deleted_total` | Counter | Total tasks deleted |
| `taskin_tasks_active` | UpDownCounter | Current active tasks |

#### Pomodoros
| Metric Name | Type | Description |
|-------------|------|-------------|
| `taskin_pomodoros_created_total` | Counter | Total pomodoros started |
| `taskin_pomodoros_completed_total` | Counter | Total pomodoros completed |
| `taskin_pomodoros_duration_minutes` | Histogram | Pomodoro duration in minutes |

### System Metrics (OpenTelemetry)

#### HTTP Metrics
- `http_server_request_duration_seconds` - Request duration histogram
- `http_server_request_duration_seconds_count` - Total request count
- `http_server_request_duration_seconds_sum` - Total request duration

#### Runtime Metrics
- `process_working_set_bytes` - Process working set memory
- `process_private_memory_bytes` - Process private memory
- `process_virtual_memory_bytes` - Process virtual memory
- `process_cpu_seconds_total` - Total CPU time
- `process_num_threads` - Number of threads
- `dotnet_gc_heap_size_bytes` - GC heap size by generation
- `dotnet_gc_collections_count` - GC collection count
- `dotnet_threadpool_num_threads` - ThreadPool thread count

#### Database Metrics (Entity Framework)
- `db_client_operation_duration_seconds` - Database query duration
- `db_client_connections_usage` - Active database connections

### Example PromQL Queries

**Request rate (requests per second):**
```promql
rate(http_server_request_duration_seconds_count[5m])
```

**P95 latency:**
```promql
histogram_quantile(0.95,
  sum(rate(http_server_request_duration_seconds_bucket[5m])) by (le)
)
```

**Error rate:**
```promql
sum(rate(http_server_request_duration_seconds_count{http_response_status_code=~"5.."}[5m]))
/
sum(rate(http_server_request_duration_seconds_count[5m]))
```

**Active projects:**
```promql
taskin_projects_active
```

---

## Alerting

### Alert Rules

Alert rules are defined in `deploy/prometheus/alerts/api-alerts.yml`.

#### Critical Alerts

**TaskinApiDown**
- **Condition:** API is unreachable for 2 minutes
- **Severity:** Critical
- **Action:** Immediate investigation required

**HighServerErrorRate**
- **Condition:** More than 5% of requests return 5xx errors
- **Severity:** Critical
- **Action:** Check logs, investigate application errors

**VeryHighApiLatencyP99**
- **Condition:** P99 latency exceeds 3 seconds for 3 minutes
- **Severity:** Critical
- **Action:** Performance investigation needed

#### Warning Alerts

**HighApiLatencyP95**
- **Condition:** P95 latency exceeds 1 second for 5 minutes
- **Severity:** Warning
- **Action:** Monitor closely, may need optimization

**HighClientErrorRate**
- **Condition:** More than 15% of requests return 4xx errors
- **Severity:** Warning
- **Action:** Check for API misuse or client issues

**SlowDatabaseQueries**
- **Condition:** P95 database query duration exceeds 500ms
- **Severity:** Warning
- **Action:** Review query performance, check indexes

### Viewing Active Alerts

1. **In Prometheus:**
   - Go to http://localhost:9090/alerts
   - View all active and pending alerts

2. **In Grafana:**
   - Install Prometheus Alert integration
   - Add alert panels to dashboards

### Customizing Alerts

Edit `deploy/prometheus/alerts/api-alerts.yml`:
```yaml
- alert: MyCustomAlert
  expr: metric_name > threshold
  for: 5m
  labels:
    severity: warning
  annotations:
    summary: "Alert description"
```

Reload Prometheus configuration:
```bash
curl -X POST http://localhost:9090/-/reload
```

---

## Troubleshooting

### Prometheus Not Scraping Metrics

**Symptoms:** No data in Grafana, Prometheus targets show DOWN

**Solutions:**
1. Check API is running: `curl http://localhost:5000/health`
2. Check metrics endpoint: `curl http://localhost:5000/metrics`
3. Verify Prometheus config: http://localhost:9090/config
4. Check Prometheus targets: http://localhost:9090/targets
5. Review Prometheus logs in Docker

### Grafana Dashboards Empty

**Symptoms:** Dashboards load but show no data

**Solutions:**
1. Verify Prometheus datasource:
   - Go to Configuration → Data Sources
   - Test Prometheus connection
2. Check time range (default: last 6 hours)
3. Verify metrics exist in Prometheus:
   - Go to http://localhost:9090/graph
   - Query `taskin_projects_created_total`
4. Check dashboard panel queries

### Metrics Not Updating

**Symptoms:** Metrics stuck at old values

**Solutions:**
1. Create test data (projects, tasks, pomodoros)
2. Wait for scrape interval (15-30 seconds)
3. Refresh Grafana dashboard
4. Check MediatR handlers are calling `TaskinMetrics` methods
5. Verify `TaskinMetrics` is registered as singleton in DI

### Container Issues

**Symptoms:** Containers not starting or crashing

**Solutions:**
1. Check Docker Desktop is running
2. Review container logs:
   ```bash
   docker logs taskin-prometheus
   docker logs taskin-grafana
   ```
3. Verify bind mount paths exist
4. Check port conflicts (9090, 3000)
5. Restart Aspire AppHost

### Configuration Files Not Loading

**Symptoms:** Dashboards missing, alerts not working

**Solutions:**
1. Verify deploy directory structure:
   ```
   deploy/
   ├── prometheus/
   │   ├── prometheus.yml
   │   └── alerts/
   │       └── api-alerts.yml
   └── grafana/
       ├── provisioning/
       └── dashboards/
   ```
2. Check bind mounts in AppHost.cs
3. Restart containers
4. Check file permissions (Linux/Mac)

---

## Production Deployment

### Security Considerations

#### 1. Enable Grafana Authentication
Update AppHost.cs:
```csharp
.WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", Environment.GetEnvironmentVariable("GRAFANA_PASSWORD"))
.WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false")
```

#### 2. Secure Prometheus
Add authentication using reverse proxy (nginx, Traefik)

#### 3. Use HTTPS
Configure TLS certificates for all endpoints

#### 4. Restrict Network Access
Use firewall rules to limit access to monitoring endpoints

### Performance Tuning

#### Prometheus
```yaml
global:
  scrape_interval: 30s      # Less frequent scraping
  evaluation_interval: 30s

storage:
  tsdb:
    retention.time: 30d
    retention.size: 50GB
```

#### Grafana
```yaml
GF_DATABASE_TYPE: postgres  # Use PostgreSQL instead of SQLite
GF_DATABASE_HOST: postgres:5432
GF_DATABASE_NAME: grafana
```

### High Availability

#### Prometheus
- Use Thanos or Cortex for long-term storage
- Set up Prometheus federation
- Configure remote write to external storage

#### Grafana
- Use external database (PostgreSQL)
- Set up multiple Grafana instances behind load balancer
- Configure shared storage for dashboards

### Backup and Recovery

#### Backing Up
```bash
# Prometheus data
docker run --rm -v taskin-prometheus-data:/data -v $(pwd):/backup alpine tar czf /backup/prometheus-backup.tar.gz /data

# Grafana data
docker run --rm -v taskin-grafana-data:/data -v $(pwd):/backup alpine tar czf /backup/grafana-backup.tar.gz /data
```

#### Restoring
```bash
# Prometheus
docker run --rm -v taskin-prometheus-data:/data -v $(pwd):/backup alpine sh -c "cd /data && tar xzf /backup/prometheus-backup.tar.gz --strip 1"

# Grafana
docker run --rm -v taskin-grafana-data:/data -v $(pwd):/backup alpine sh -c "cd /data && tar xzf /backup/grafana-backup.tar.gz --strip 1"
```

### Monitoring the Monitoring Stack

#### Prometheus Self-Monitoring
Prometheus automatically monitors itself. Check:
- http://localhost:9090/metrics
- Scrape duration, sample counts, memory usage

#### Grafana Monitoring
Install Grafana self-monitoring dashboard:
- Dashboard ID: 3590 (from grafana.com)

---

## Frontend Integration (Grafana Faro)

### Overview

Grafana Faro is the Web SDK for Real User Monitoring (RUM) that captures observability signals from frontend applications. It provides:
- **Web Vitals**: LCP, FID/INP, CLS, TTFB
- **Performance Monitoring**: Page loads, resource timing
- **Error Tracking**: JavaScript errors and exceptions
- **User Sessions**: Session tracking and user flows
- **Custom Events**: Business-specific frontend events

### Architecture

```
┌─────────────────────────────────────────────────┐
│            Angular Application                   │
│  ┌──────────────────────────────────────────┐   │
│  │     Grafana Faro Web SDK                 │   │
│  │  - Web Vitals Instrumentation            │   │
│  │  - Errors Instrumentation                │   │
│  │  - Performance Instrumentation           │   │
│  └──────────────┬───────────────────────────┘   │
└─────────────────┼───────────────────────────────┘
                  │ HTTP POST
                  ▼
         ┌────────────────┐
         │ Faro Collector │  (Self-hosted)
         │ (Grafana Alloy)│
         └────────┬───────┘
                  │
        ┌─────────┴─────────┐
        │                   │
        ▼                   ▼
  ┌─────────┐        ┌──────────┐
  │ Grafana │◄───────│Prometheus│
  └─────────┘        └──────────┘
```

### Installation

#### 1. Install Grafana Faro SDK

Navigate to the Angular project and install the SDK:

```bash
cd ui/src
npm install @grafana/faro-web-sdk @grafana/faro-web-tracing
```

#### 2. Configure Faro in Angular

Create or update `ui/src/app/core/monitoring/faro.config.ts`:

```typescript
import {
  initializeFaro,
  getWebInstrumentations,
  LogLevel
} from '@grafana/faro-web-sdk';
import { TracingInstrumentation } from '@grafana/faro-web-tracing';

export function initializeFaroRUM() {
  const faro = initializeFaro({
    url: 'http://localhost:12345/collect', // Faro collector endpoint
    app: {
      name: 'taskin-frontend',
      version: '2.0.0',
      environment: 'development' // or 'production'
    },

    // Instrumentations
    instrumentations: [
      ...getWebInstrumentations({
        captureConsole: true,
        captureConsoleDisabledLevels: [LogLevel.DEBUG, LogLevel.TRACE]
      }),
      new TracingInstrumentation()
    ],

    // Session tracking
    session: {
      trackResources: true,
      trackWebVitals: true
    },

    // Performance
    batching: {
      enabled: true,
      sendTimeout: 5000,
      itemLimit: 50
    },

    // Privacy settings
    beforeSend: (item) => {
      // Filter sensitive data here
      return item;
    }
  });

  return faro;
}
```

#### 3. Initialize in Application

Update `ui/src/app/app.config.ts`:

```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { initializeFaroRUM } from './core/monitoring/faro.config';
import { environment } from '../environments/environment';

// Initialize Faro RUM if enabled
if (environment.faroEnabled) {
  initializeFaroRUM();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([/* your interceptors */]))
  ]
};
```

#### 4. Environment Configuration

Update `ui/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  faroEnabled: true,
  faroCollectorUrl: 'http://localhost:12345/collect',
  apiUrl: 'http://localhost:5000'
};
```

And `ui/src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  faroEnabled: true,
  faroCollectorUrl: 'https://your-faro-collector.com/collect',
  apiUrl: 'https://api.taskin.com'
};
```

### Setting Up Grafana Alloy (Faro Collector)

#### Option 1: Add to AppHost (Recommended)

Update `back/src/Taskin2.0.AppHost/AppHost.cs`:

```csharp
// Add Grafana Alloy for Faro RUM collection
var alloy = builder.AddContainer("alloy", "grafana/alloy", "latest")
    .WithContainerName("taskin-alloy")
    .WithBindMount(Path.Combine(deployPath, "alloy", "config.alloy"), "/etc/alloy/config.alloy")
    .WithHttpEndpoint(port: 12345, targetPort: 12345, name: "faro-collector")
    .WithHttpEndpoint(port: 12346, targetPort: 12346, name: "alloy-ui")
    .WithEnvironment("ALLOY_MODE", "flow")
    .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=alloy");
```

#### Option 2: Docker Compose

Create `deploy/alloy/docker-compose.yml`:

```yaml
services:
  alloy:
    image: grafana/alloy:latest
    container_name: taskin-alloy
    ports:
      - "12345:12345"  # Faro collector
      - "12346:12346"  # Alloy UI
    volumes:
      - ./config.alloy:/etc/alloy/config.alloy
    command:
      - run
      - /etc/alloy/config.alloy
      - --server.http.listen-addr=0.0.0.0:12346
```

#### Configure Alloy

Create `deploy/alloy/config.alloy`:

```hcl
// Faro receiver for frontend RUM data
faro.receiver "frontend" {
  server {
    listen_address = "0.0.0.0"
    listen_port    = 12345
    cors_allowed_origins = ["http://localhost:4200"]
  }

  output {
    logs   = [loki.write.local.receiver]
    traces = [otel.exporter.prometheus.default.receiver]
  }
}

// Export to Loki for logs (or use Grafana Cloud)
loki.write "local" {
  endpoint {
    url = "http://loki:3100/loki/api/v1/push"
  }
}

// Export traces as metrics to Prometheus
otel.exporter.prometheus "default" {
  forward_to = [prometheus.remote_write.local.receiver]
}

prometheus.remote_write "local" {
  endpoint {
    url = "http://prometheus:9090/api/v1/write"
  }
}
```

### Custom Events and Tracking

#### Track Custom Business Events

```typescript
import { faro } from '@grafana/faro-web-sdk';

// Track project creation
faro.api.pushEvent('project_created', {
  project_id: projectId,
  user_id: userId
});

// Track task completion
faro.api.pushEvent('task_completed', {
  task_id: taskId,
  duration: durationMinutes,
  pomodoros_used: pomodorosCount
});

// Track errors
faro.api.pushError(new Error('Custom error'), {
  context: 'task-creation',
  additional_data: { taskId }
});
```

#### Add User Context

```typescript
// Set user information (after login)
faro.api.setUser({
  id: user.id,
  username: user.username,
  email: user.email,
  attributes: {
    role: user.role,
    plan: user.plan
  }
});
```

### Monitoring Frontend Performance

With Faro enabled, you can monitor:

1. **Web Vitals Dashboard** (Faro Frontend - Dashboard #17766)
   - LCP: Largest Contentful Paint
   - INP: Interaction to Next Paint (replaces FID)
   - CLS: Cumulative Layout Shift
   - TTFB: Time to First Byte

2. **Error Tracking**
   - JavaScript errors
   - Network errors
   - Resource loading failures

3. **User Sessions**
   - Session duration
   - Page views per session
   - User flows and navigation

4. **Performance Metrics**
   - Page load time
   - Resource loading time
   - API call duration (via tracing)

### Verification

1. **Check Faro Collector:**
   ```bash
   curl http://localhost:12345/health
   ```

2. **View in Grafana:**
   - Navigate to http://localhost:3000
   - Open "Frontend - Angular" folder
   - Select "Grafana Faro - Frontend Monitoring" dashboard

3. **Test Event Tracking:**
   ```typescript
   // In Angular DevTools console
   faro.api.pushEvent('test_event', { test: true });
   ```

---

## Additional Resources

### Official Documentation
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Grafana Faro Web SDK](https://grafana.com/docs/grafana-cloud/monitor-applications/frontend-observability/)
- [Grafana Alloy](https://grafana.com/docs/alloy/latest/)

### Related Taskin Documentation
- [ASPIRE_TELEMETRY_SETUP.md](./ASPIRE_TELEMETRY_SETUP.md) - OpenTelemetry configuration details
- [CLAUDE.md](../CLAUDE.md) - Project setup and development guidelines

### Official Dashboards from Grafana.com
Included in this setup:
- **ASP.NET Core:** Dashboard #19924
- **ASP.NET Core Endpoint:** Dashboard #19925
- **.NET Runtime Metrics:** Dashboard #23179
- **OpenTelemetry APM:** Dashboard #19419
- **Grafana Faro - Frontend Monitoring:** Dashboard #17766

---

## Support

For issues or questions:
1. Check [Troubleshooting](#troubleshooting) section
2. Review Prometheus/Grafana logs
3. Consult official documentation
4. Open an issue in the project repository

---

**Last Updated:** 2025-01-16
**Version:** 1.0.0
**Maintainer:** Taskin Development Team
