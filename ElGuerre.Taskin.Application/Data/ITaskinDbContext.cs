using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Data;

public interface ITaskinDbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Pomodoro> Pomodoros { get; set; }
}