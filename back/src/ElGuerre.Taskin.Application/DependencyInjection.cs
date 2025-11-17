using ElGuerre.Taskin.Application.Observability;
using Microsoft.Extensions.DependencyInjection;

namespace ElGuerre.Taskin.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register TaskinMetrics as a singleton
        services.AddSingleton<TaskinMetrics>();

        return services;
    }
}
