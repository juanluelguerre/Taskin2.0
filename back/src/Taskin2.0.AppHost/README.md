# Taskin 2.0 AppHost - Aspire Configuration

This project uses .NET Aspire to orchestrate the Taskin application and its dependencies, including an optional observability stack.

## Port Configuration

### API Service
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

**Important**: The frontend Angular application is configured to connect to `https://localhost:5001`. If you change the AppHost port configuration, you must also update the frontend environment files:
- Development: `ui/src/src/environments/environment.ts`
- Production: `ui/src/src/environments/environment.prod.ts`

### Observability Services (when enabled)

| Service | Port | Description |
|---------|------|-------------|
| Grafana | 3000 | Metrics and logs visualization |
| Prometheus | 9090 | Metrics collection and storage |
| Tempo | 3200 | Distributed tracing backend |
| Loki | 3100 | Log aggregation system |
| Grafana Alloy | 12345 (Faro), 12347 (HTTP) | Frontend RUM proxy and telemetry collector |
| OTel Collector | 4317, 4318, 8889, 8888 | OpenTelemetry data collection |
| Seq | 5341 | Structured logging |
| SQL Server | 1433 | Database |
| Redis | 6379 | Caching |

## Running the Application

### Start with Observability Stack

```bash
cd back/src/Taskin2.0.AppHost
dotnet run
```

This will start:
- The Taskin API on port 5001
- SQL Server database
- Redis cache
- Seq for logging
- Full observability stack (Grafana, Prometheus, Tempo, Loki, etc.)

Access the Aspire Dashboard at the URL shown in the console output (typically `http://localhost:15xxx`).

### Start without Observability Stack

To run with minimal dependencies (API, database, cache only):

1. Edit `appsettings.Development.json`
2. Set `"Observability": { "EnableProductionStack": false }`
3. Run `dotnet run`

Or use a custom launch profile in `Properties/launchSettings.json`.

## Launch Profiles

The AppHost supports multiple launch profiles (defined in `Properties/launchSettings.json`):

- **`https`**: Default profile, runs on port 5001 with current configuration
- **`https-production-stack`**: Explicitly enables full observability stack
- **`http`**: HTTP-only mode (not recommended for production)

To use a specific profile:

```bash
dotnet run --launch-profile https-production-stack
```

## Configuration

### Observability Stack

Control the observability stack via `appsettings.json`:

```json
{
  "Observability": {
    "EnableProductionStack": true,    // Set to false to disable
    "DeployPath": "../deploy"         // Path to Docker Compose files
  }
}
```

When `EnableProductionStack` is `true`:
- All observability containers are started via Docker Compose
- API is configured to send telemetry to the OTel Collector
- Frontend can send RUM data to Grafana Alloy

### Environment Variables

The AppHost automatically configures these environment variables for the API:

- `ConnectionStrings__DefaultConnection`: SQL Server connection
- `ConnectionStrings__RedisConnection`: Redis connection
- `OTEL_EXPORTER_OTLP_ENDPOINT`: OpenTelemetry endpoint (when observability enabled)
- `OTEL_SERVICE_NAME`: Service name for tracing
- Various Grafana/observability endpoints

## Accessing Services

Once running:

- **API**: https://localhost:5001
- **API Swagger**: https://localhost:5001/swagger
- **Grafana**: http://localhost:3000 (admin/admin)
- **Prometheus**: http://localhost:9090
- **Seq**: http://localhost:5341
- **Aspire Dashboard**: Check console output for dynamic port

## Frontend Integration

The Angular frontend (`ui/src/`) must be configured to use the correct API port:

**Development** (`environment.ts`):
```typescript
export const environment = {
  apiUrl: 'https://localhost:5001',  // Must match AppHost port
  faro: {
    url: 'http://localhost:12345/collect',  // Grafana Alloy endpoint
    // ...
  }
};
```

## Troubleshooting

### Frontend Can't Connect to API

**Error**: `net::ERR_CONNECTION_REFUSED` at `https://localhost:6001` or `5001`

**Solution**:
1. Verify AppHost is running: `dotnet run --project Taskin2.0.AppHost`
2. Check Aspire Dashboard shows API as "Running"
3. Verify frontend `environment.ts` has `apiUrl: 'https://localhost:5001'`
4. Ensure no firewall is blocking port 5001

### Observability Stack Not Starting

**Issue**: Docker containers fail to start

**Solution**:
1. Ensure Docker Desktop is running
2. Check `DeployPath` points to correct location with `docker-compose.yml` files
3. Review AppHost console output for Docker errors
4. Try disabling observability: set `EnableProductionStack: false`

### Port Already in Use

**Error**: Port 5001 or other ports already bound

**Solution**:
1. Stop other applications using those ports
2. Change AppHost port in `Properties/launchSettings.json`
3. Update frontend `environment.ts` to match new port
4. Restart both AppHost and frontend

## Development Workflow

### Typical Development Setup

1. **Start Backend**:
   ```bash
   cd back/src/Taskin2.0.AppHost
   dotnet run
   ```

2. **Start Frontend**:
   ```bash
   cd ui/src
   npm start
   ```

3. **Access Application**: http://localhost:4200

### Debugging with Observability

When debugging observability features:
1. Ensure `EnableProductionStack: true` in `appsettings.json`
2. Start AppHost (all containers will launch)
3. Monitor metrics in Grafana: http://localhost:3000
4. View logs in Seq: http://localhost:5341
5. Check traces in Tempo via Grafana

### Lightweight Development

For faster startup without observability:
1. Set `EnableProductionStack: false` in `appsettings.Development.json`
2. Only API, SQL Server, Redis, and Seq will start
3. Significantly reduces resource usage and startup time

## Architecture

The AppHost uses .NET Aspire's orchestration capabilities to:
- Manage service dependencies (SQL Server, Redis)
- Configure service discovery and networking
- Handle environment variable injection
- Coordinate container lifecycle
- Provide unified monitoring via Aspire Dashboard

Services are defined in `AppHost.cs` using the Aspire builder pattern:

```csharp
var apiBuilder = builder.AddProject<Projects.ElGuerre_Taskin_Api>("taskin-api")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithReference(seq);
```

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Grafana Documentation](https://grafana.com/docs/)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)
