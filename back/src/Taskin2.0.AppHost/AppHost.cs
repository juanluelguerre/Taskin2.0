var builder = DistributedApplication.CreateBuilder(args);

const string projectName = "taskin";

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
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=seq");

// Get absolute path to deploy directory
var deployPath = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "..", "..", "..", "deploy"));

// Add Prometheus for metrics collection
var prometheus = builder.AddContainer("prometheus", "prom/prometheus", "v2.45.0")
    .WithContainerName("taskin-prometheus")
    .WithVolume("taskin-prometheus-data", "/prometheus")
    .WithBindMount(Path.Combine(deployPath, "prometheus", "prometheus.yml"), "/etc/prometheus/prometheus.yml")
    .WithBindMount(Path.Combine(deployPath, "prometheus", "alerts"), "/etc/prometheus/alerts")
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
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=prometheus");

// Add Grafana for metrics visualization
var grafana = builder.AddContainer("grafana", "grafana/grafana", "10.0.0")
    .WithContainerName("taskin-grafana")
    .WithVolume("taskin-grafana-data", "/var/lib/grafana")
    .WithBindMount(Path.Combine(deployPath, "grafana", "provisioning"), "/etc/grafana/provisioning")
    .WithBindMount(Path.Combine(deployPath, "grafana", "dashboards"), "/etc/grafana/dashboards")
    .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http")
    // Security settings
    .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
    .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false")
    .WithEnvironment("GF_SERVER_ROOT_URL", "http://localhost:3000")
    // Data sources URLs
    .WithEnvironment("PROMETHEUS_URL", "http://prometheus:9090")
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
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=grafana");

// Add API project with resource references
var api = builder.AddProject<Projects.ElGuerre_Taskin_Api>("taskin-api")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithReference(seq)
    .WithEnvironment("Prometheus:Enabled", "true");

builder.Build().Run();
