using ElGuerre.Taskin.Api.Extensions;
using ElGuerre.Taskin.Api.Observability;
using ElGuerre.Taskin.Application;
using ElGuerre.Taskin.Infrastructure.EntityFramework;
using ElGuerre.Taskin.Infrastructure.Middleware;
using ElGuerre.Taskin.Application.Observability;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire ServiceDefaults (OpenTelemetry, Health Checks, Service Discovery)
builder.AddServiceDefaults();

// Add SQL Server DbContext with Aspire integration
builder.AddSqlServerDbContext<TaskinDbContext>("taskin-db");

// OpenTelemetry logging configured via ServiceDefaults
// Logs will automatically export to Aspire Dashboard and Seq

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTaskin(builder.Configuration);
builder.Services.AddProblemDetails(options => { });

// Add custom activity source for distributed tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(TelemetryConstants.ActivitySourceName))
    .WithMetrics(metrics => metrics.AddMeter(TelemetryConstants.MeterName))
    .WithMetrics(metrics => metrics.AddMeter("Taskin.Application")); // Add Application metrics meter

builder.Services.AddApplicationServices();

// Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

var app = builder.Build();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
}

app.UseTasking();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Request logging handled by OpenTelemetry instrumentation in ServiceDefaults
app.UseHttpsRedirection();
app.UseCors("AllowAngularDev");
app.UseAuthorization();
app.MapControllers();

// Map Aspire default endpoints (health checks, Prometheus metrics)
app.MapDefaultEndpoints();

app.Run();