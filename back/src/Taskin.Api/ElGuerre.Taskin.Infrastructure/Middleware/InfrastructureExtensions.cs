using ElGuerre.Taskin.Application.Behaviors;
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Projects.Commands;
using ElGuerre.Taskin.Domain.SeedWork;
using FluentValidation;
using FluentValidation.AspNetCore;
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
        services.AddDomainServices();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TaskinDbContext>(options =>
            options.UseInMemoryDatabase("TaskinDb"));
        //services.AddDbContext<TaskinDbContext>(options =>
        //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITaskinDbContext>(provider => provider.GetRequiredService<TaskinDbContext>());

        return services;
    }

    private static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(CreateProjectCommand).Assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(CreateProjectCommand).Assembly);

        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}