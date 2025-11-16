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

// Add API project with resource references
var api = builder.AddProject<Projects.ElGuerre_Taskin_Api>("taskin-api")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithReference(seq)
    .WithEnvironment("Prometheus:Enabled", "true");

builder.Build().Run();
