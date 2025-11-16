# Docker Container Grouping in Aspire

## Implemented Configuration

The Docker containers for the Taskin 2.0 application are configured to group under the "taskin" project in Docker Desktop, similar to a docker-compose stack.

## Container Names

All containers use the `taskin-*` prefix:

```
CONTAINER NAME       IMAGE                                    PORTS
taskin-sqlserver     mcr.microsoft.com/mssql/server:2022      1433->1433
taskin-redis         redis:latest                             6379->6379
taskin-seq           datalust/seq:latest                      5341->80
```

## Configuration in AppHost

**File**: `back/src/Taskin2.0.AppHost/AppHost.cs`

### Implementation with WithContainerRuntimeArgs

```csharp
var builder = DistributedApplication.CreateBuilder(args);

const string projectName = "taskin";

// SQL Server with container name and Docker Compose labels
var sqlServer = builder.AddSqlServer("sql-server")
    .WithContainerName("taskin-sqlserver")
    .WithDataVolume("taskin-sql-data")
    .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=sql-server")
    .AddDatabase("taskin-db");

// Redis with container name and Docker Compose labels
var redis = builder.AddRedis("redis")
    .WithContainerName("taskin-redis")
    .WithDataVolume("taskin-redis-data")
    .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=redis");

// Seq with container name and Docker Compose labels
var seq = builder.AddSeq("seq")
    .WithContainerName("taskin-seq")
    .WithDataVolume("taskin-seq-data")
    .WithContainerRuntimeArgs("--label", $"com.docker.compose.project={projectName}")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.service=seq");

// API with resource references
var api = builder.AddProject<Projects.ElGuerre_Taskin_Api>("taskin-api")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithReference(seq)
    .WithEnvironment("Prometheus:Enabled", "true");

builder.Build().Run();
```

**How It Works:**
- `WithContainerRuntimeArgs()` adds arguments directly to the `docker run` command
- The labels `com.docker.compose.project` and `com.docker.compose.service` are the same ones used by Docker Compose
- Docker Desktop automatically detects these labels and groups the containers under the "taskin" project
- Each container must configure its labels individually using the fluent API method

## Advantages of This Configuration

### 1. **Visualization in Docker Desktop**
Containers appear grouped under the "taskin-" prefix:
```
└─ taskin-sqlserver
└─ taskin-redis
└─ taskin-seq
```

### 2. **Simplified Management**
```bash
# Stop all Taskin containers
docker stop taskin-sqlserver taskin-redis taskin-seq

# Remove all Taskin containers
docker rm taskin-sqlserver taskin-redis taskin-seq

# View logs from a specific container
docker logs taskin-sqlserver

# Inspect shared network
docker network ls | grep aspire
```

### 3. **Automatic Shared Network**
Aspire automatically creates a bridge network for the containers:
```
Network Name: aspire-<session/persistent>-network-<hash>-taskin2-apphost
```

All containers connect to this network, allowing internal communication using resource names.

## Persistent Volumes

Data persists in named volumes:

```
VOLUME NAME              DESCRIPTION
taskin-sql-data          SQL Server database
taskin-redis-data        Redis cache
taskin-seq-data          Seq logs
```

### Volume Management

```bash
# List volumes
docker volume ls | grep taskin

# Inspect a volume
docker volume inspect taskin-sql-data

# Backup the database
docker run --rm \
  -v taskin-sql-data:/data \
  -v $(pwd):/backup \
  busybox tar czf /backup/taskin-db-backup.tar.gz /data

# Remove volumes (WARNING! Data will be lost)
docker volume rm taskin-sql-data taskin-redis-data taskin-seq-data
```

## Container Communication

Within the Aspire network, containers can communicate using:

- **By Aspire resource name**:
  - `sql-server:1433`
  - `redis:6379`
  - `seq:5341`

- **By Docker container name**:
  - `taskin-sqlserver:1433`
  - `taskin-redis:6379`
  - `taskin-seq:5341`

## Useful Commands

### View Container Status
```bash
docker ps --filter "name=taskin-"
```

### View Network Resources
```bash
docker network inspect $(docker network ls --filter "name=aspire" --format "{{.Name}}")
```

### Usage Statistics
```bash
docker stats taskin-sqlserver taskin-redis taskin-seq
```

### Clean Everything (Complete Reset)
```bash
# Stop AppHost first, then:
docker stop taskin-sqlserver taskin-redis taskin-seq
docker rm taskin-sqlserver taskin-redis taskin-seq
docker volume rm taskin-sql-data taskin-redis-data taskin-seq-data
```

## Troubleshooting

### Container Name Conflict

If you get a "container name already in use" error:

```bash
# Stop and remove existing containers
docker stop taskin-sqlserver taskin-redis taskin-seq 2>/dev/null
docker rm taskin-sqlserver taskin-redis taskin-seq 2>/dev/null

# Restart AppHost
```

### Port Conflict

If ports are already in use:

```bash
# See what process is using the port (Windows)
netstat -ano | findstr :1433
netstat -ano | findstr :6379
netstat -ano | findstr :5341

# On Linux/Mac
lsof -i :1433
lsof -i :6379
lsof -i :5341
```

### Recreate Containers with Clean Data

```bash
# Stop AppHost
# Remove only containers (keeps volumes)
docker rm -f taskin-sqlserver taskin-redis taskin-seq

# Restart AppHost - will recreate with existing data
```

## Alternative: Docker Compose

If you prefer to use Docker Compose directly, you can export the configuration:

```bash
# From .NET Aspire 9.2+
aspire publish --output-path ./publish
```

This generates a `docker-compose.yaml` that you can use:

```bash
cd publish
docker-compose up -d
```

## Monitoring

### Aggregated Logs
```bash
# View logs from all containers
docker logs -f taskin-sqlserver &
docker logs -f taskin-redis &
docker logs -f taskin-seq &
```

### Health Checks
You can verify health status from:
- **Aspire Dashboard**: https://localhost:17281
- **Seq Dashboard**: http://localhost:<seq-port>

## Best Practices

1. **Naming Convention**: Maintain the `taskin-` prefix for all related containers

2. **Volumes**: Always use named volumes for persistent data

3. **Networks**: Let Aspire manage networks automatically

4. **Backups**: Make regular backups of data volumes:
   ```bash
   ./backup-volumes.sh  # Custom script
   ```

5. **Cleanup**: Remove unused resources periodically:
   ```bash
   docker system prune -a --volumes
   ```

## References

- [Aspire Orchestration Overview](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/app-host-overview)
- [Aspire Networking](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/networking-overview)
- [Docker Compose Integration](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/docker-integration)
