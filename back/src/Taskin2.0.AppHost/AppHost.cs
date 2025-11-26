using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

const string projectName = "taskin";

// ============================================================================
// CONFIGURATION
// ============================================================================
var enableProductionStack = builder.Configuration.GetValue<bool>("Observability:EnableProductionStack");
var deployPath = builder.Configuration.GetValue<string>("Observability:DeployPath") ?? "../deploy";
var absoluteDeployPath = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "..", deployPath));

// ============================================================================
// CORE INFRASTRUCTURE (Always enabled)
// ============================================================================

// Add SQL Server with persistent volume, consistent container name, and Docker Compose labels
var sqlServer = builder.AddSqlServer("sql-server")
    .WithContainerName("taskin-sqlserver")
    .WithDataVolume("taskin-sql-data")
    .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=sql-server")
    .AddDatabase("taskin-db");

// Add Redis for caching with consistent container name and Docker Compose labels
var redis = builder.AddRedis("redis")
    .WithContainerName("taskin-redis")
    .WithDataVolume("taskin-redis-data")
    .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=redis");

// Add Seq for structured logging with consistent container name and Docker Compose labels
var seq = builder.AddSeq("seq")
    .WithContainerName("taskin-seq")
    .WithDataVolume("taskin-seq-data")
    .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=seq")
    .WithContainerRuntimeArgs("-p", "5341:80");

// ============================================================================
// PRODUCTION OBSERVABILITY STACK (Conditional)
// ============================================================================
IResourceBuilder<ContainerResource>? tempo = null;
IResourceBuilder<ContainerResource>? loki = null;
IResourceBuilder<ContainerResource>? otelCollector = null;
IResourceBuilder<ContainerResource>? prometheus = null;
IResourceBuilder<ContainerResource>? grafana = null;

if (enableProductionStack)
{
    Console.WriteLine($"[Taskin AppHost] Production observability stack ENABLED");
    Console.WriteLine($"[Taskin AppHost] Deploy path: {absoluteDeployPath}");

    // Add Tempo for distributed tracing
    tempo = builder.AddContainer("tempo", "grafana/tempo", "2.3.1")
        .WithContainerName("taskin-tempo")
        .WithVolume("taskin-tempo-data", "/var/tempo")
        .WithBindMount(Path.Combine(absoluteDeployPath, "tempo", "tempo.yaml"), "/etc/tempo/tempo.yaml")
        .WithHttpEndpoint(port: 3200, targetPort: 3200, name: "http")
        .WithEndpoint(port: 4317, targetPort: 4317, name: "otlp-grpc")
        .WithEndpoint(port: 4318, targetPort: 4318, name: "otlp-http")
        .WithArgs("-config.file=/etc/tempo/tempo.yaml")
        .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
        .WithContainerRuntimeArgs("--label", "com.docker.compose.service=tempo");

    // Add Loki for log aggregation (3.0+ has native OTLP support)
    loki = builder.AddContainer("loki", "grafana/loki", "3.0.0")
        .WithContainerName("taskin-loki")
        .WithVolume("taskin-loki-data", "/loki")
        .WithBindMount(Path.Combine(absoluteDeployPath, "loki", "local-config.yaml"), "/etc/loki/local-config.yaml")
        .WithHttpEndpoint(port: 3100, targetPort: 3100, name: "http")
        .WithArgs("-config.file=/etc/loki/local-config.yaml")
        .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
        .WithContainerRuntimeArgs("--label", "com.docker.compose.service=loki");

    // Add OpenTelemetry Collector
    otelCollector = builder.AddContainer("otel-collector", "otel/opentelemetry-collector-contrib", "0.139.0")
        .WithContainerName("taskin-otel-collector")
        .WithBindMount(Path.Combine(absoluteDeployPath, "otel-collector", "config.yaml"), "/etc/otelcol-contrib/config.yaml")
        .WithHttpEndpoint(port: 4317, targetPort: 4317, name: "otlp-grpc")
        .WithHttpEndpoint(port: 4318, targetPort: 4318, name: "otlp-http")
        .WithHttpEndpoint(port: 8889, targetPort: 8889, name: "prometheus")
        .WithHttpEndpoint(port: 8888, targetPort: 8888, name: "metrics")
        // Pass Aspire Dashboard OTLP endpoint to forward telemetry back to Aspire
        // Note: Using host.docker.internal since OTel Collector runs in Docker
        .WithEnvironment("ASPIRE_DASHBOARD_OTLP_ENDPOINT",
            builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] ?? "http://host.docker.internal:18889")
        .WithArgs("--config=/etc/otelcol-contrib/config.yaml")
        .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
        .WithContainerRuntimeArgs("--label", "com.docker.compose.service=otel-collector")
        // Important: OTel Collector needs to talk to Tempo and Loki
        .WaitFor(tempo)
        .WaitFor(loki);

    // Add Prometheus for metrics collection
    prometheus = builder.AddContainer("prometheus", "prom/prometheus", "v2.45.0")
        .WithContainerName("taskin-prometheus")
        .WithVolume("taskin-prometheus-data", "/prometheus")
        .WithBindMount(Path.Combine(absoluteDeployPath, "prometheus", "prometheus.yml"), "/etc/prometheus/prometheus.yml")
        .WithBindMount(Path.Combine(absoluteDeployPath, "prometheus", "alerts"), "/etc/prometheus/alerts")
        .WithHttpEndpoint(port: 9090, targetPort: 9090, name: "http")
        .WithArgs(
            "--config.file=/etc/prometheus/prometheus.yml",
            "--storage.tsdb.path=/prometheus",
            "--storage.tsdb.retention.time=15d",
            "--storage.tsdb.retention.size=5GB",
            "--web.console.libraries=/usr/share/prometheus/console_libraries",
            "--web.console.templates=/usr/share/prometheus/consoles",
            "--web.enable-lifecycle"
        )
        .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
        .WithContainerRuntimeArgs("--label", "com.docker.compose.service=prometheus")
        // Prometheus scrapes OTel Collector
        .WaitFor(otelCollector);

    // Add Grafana for metrics visualization
    grafana = builder.AddContainer("grafana", "grafana/grafana", "10.3.3")
        .WithContainerName("taskin-grafana")
        .WithVolume("taskin-grafana-data", "/var/lib/grafana")
        .WithBindMount(Path.Combine(absoluteDeployPath, "grafana", "provisioning"), "/etc/grafana/provisioning")
        .WithBindMount(Path.Combine(absoluteDeployPath, "grafana", "dashboards"), "/etc/grafana/dashboards")
        .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http")
        // Security settings
        .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
        .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
        .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false")
        .WithEnvironment("GF_SERVER_ROOT_URL", "http://localhost:3000")
        // Data sources URLs
        .WithEnvironment("PROMETHEUS_URL", "http://prometheus:9090")
        .WithEnvironment("TEMPO_URL", "http://tempo:3200")
        .WithEnvironment("LOKI_URL", "http://loki:3100")
        .WithEnvironment("SEQ_URL", "http://seq:5341")
        // Enable features for better navigation
        .WithEnvironment("GF_FEATURE_TOGGLES_ENABLE", "tempoSearch,correlations,exploreMetrics")
        .WithEnvironment("GF_EXPLORE_ENABLED", "true")
        // Analytics and unified alerting
        .WithEnvironment("GF_ANALYTICS_REPORTING_ENABLED", "false")
        .WithEnvironment("GF_UNIFIED_ALERTING_ENABLED", "true")
        // Default home dashboard
        .WithEnvironment("GF_DASHBOARDS_DEFAULT_HOME_DASHBOARD_PATH", "/etc/grafana/dashboards/taskin-overview.json")
        .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
        .WithContainerRuntimeArgs("--label", "com.docker.compose.service=grafana")
        // Grafana depends on all datasources
        .WaitFor(prometheus)
        .WaitFor(tempo)
        .WaitFor(loki);
}
else
{
    Console.WriteLine($"[Taskin AppHost] Production observability stack DISABLED - using Aspire Dashboard only");
}

// ============================================================================
// APPLICATION SERVICES
// ============================================================================

var apiBuilder = builder.AddProject<Projects.ElGuerre_Taskin_Api>("taskin-api", launchProfileName: "https")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithReference(seq);

if (enableProductionStack && otelCollector != null)
{
    Console.WriteLine($"[Taskin AppHost] API configured for production observability (OTel Collector)");

    // Get the OTel Collector HTTP endpoint and create a reference expression
    // This will resolve to the actual localhost:PORT at runtime
    var otlpEndpoint = otelCollector.GetEndpoint("otlp-http");
    var otlpEndpointUrl = ReferenceExpression.Create($"{otlpEndpoint}");

    apiBuilder
        .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpointUrl)
        .WithEnvironment("OTEL_SERVICE_NAME", "taskin-api")
        .WithEnvironment("OTEL_RESOURCE_ATTRIBUTES", "service.namespace=taskin,deployment.environment=development")
        .WithEnvironment("Prometheus:Enabled", "false")
        .WaitFor(otelCollector);
}
else
{
    Console.WriteLine($"[Taskin AppHost] API configured for development (Aspire Dashboard)");
}

builder.Build().Run();
