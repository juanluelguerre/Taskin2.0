using ElGuerre.Taskin.Application.Behaviors;
using System.Reflection;
using ElGuerre.Taskin.Infrastructure.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElGuerre.Taskin.Infrastructure.Middleware;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddTaskin(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddCustomMediatR();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        //services.AddDbContext<TaskinDbContext>(options =>
        //    options.UseInMemoryDatabase("TaskinDb"));
        services.AddDbContext<TaskinDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    private static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}