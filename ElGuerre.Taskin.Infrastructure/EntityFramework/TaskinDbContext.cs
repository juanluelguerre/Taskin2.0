using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Infrastructure.EntityFramework;

public class TaskinDbContext(DbContextOptions<TaskinDbContext> options)
    : DbContext(options), ITaskinDbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Pomodoro> Pomodoros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine,
            new[]
            {
                DbLoggerCategory.Database.Command.Name
            },
            LogLevel.Information);
#endif
    }
}