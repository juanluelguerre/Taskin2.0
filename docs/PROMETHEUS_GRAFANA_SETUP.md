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

### 1. Taskin - Business Metrics
**UID:** `taskin-business-metrics`

Visualizes application-level business metrics:
- **Projects Activity**: Creation/deletion rate over time
- **Active Projects Gauge**: Current number of active projects
- **Tasks Activity**: Creation/completion/deletion trends
- **Task Completion Rate**: Percentage of tasks completed
- **Pomodoros Completed**: Hourly completion rate
- **Pomodoro Duration Distribution**: P50, P95, P99 latencies
- **Weekly Summary**: 7-day aggregated stats

**Best for:** Product managers, stakeholders

### 2. Taskin - API Performance
**UID:** `taskin-api-performance`

Monitors API health and performance:
- **HTTP Request Rate**: Requests per second by status code
- **Success Rate Gauge**: Percentage of 2xx responses
- **Latency Percentiles**: P50, P95, P99 response times
- **Latency by Endpoint**: Performance breakdown per route
- **HTTP Status Codes**: Distribution of response codes
- **Error Rates**: 4xx and 5xx error percentages

**Best for:** Developers, DevOps engineers

### 3. Taskin - Infrastructure
**UID:** `taskin-infrastructure`

Shows system-level metrics:
- **Process Memory Usage**: Working set, private, virtual memory
- **GC Heap Size**: Memory per generation
- **Garbage Collection Rate**: Collections per second
- **Thread Count**: Total threads and ThreadPool threads
- **CPU Usage**: Process CPU percentage
- **Database Metrics**: Query latency, rate, connections
- **Service Health**: Uptime and status

**Best for:** DevOps engineers, SREs

### Accessing Dashboards

1. **Via Grafana UI:**
   - Navigate to http://localhost:3000
   - Go to Dashboards → Browse
   - Select "Taskin 2.0" folder

2. **Direct Links:**
   - Business: http://localhost:3000/d/taskin-business-metrics
   - Performance: http://localhost:3000/d/taskin-api-performance
   - Infrastructure: http://localhost:3000/d/taskin-infrastructure

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

## Additional Resources

### Official Documentation
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)

### Related Taskin Documentation
- [ASPIRE_TELEMETRY_SETUP.md](./ASPIRE_TELEMETRY_SETUP.md) - OpenTelemetry configuration details
- [CLAUDE.md](../CLAUDE.md) - Project setup and development guidelines

### Community Dashboards
Import pre-built dashboards from [Grafana.com](https://grafana.com/grafana/dashboards/):
- ASP.NET Core: Dashboard #10427
- .NET Runtime: Dashboard #14894
- Prometheus Stats: Dashboard #3662

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
