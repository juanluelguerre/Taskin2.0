using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Infrastructure.EntityFramework.EntityConfigurations;

internal class TaskEntityTypeConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(ElGuerre.Taskin.Domain.Entities.TaskPriority.Medium);

        builder.Property(t => t.Deadline);

        builder.Property(t => t.Tags)
            .HasMaxLength(500);

        builder.Property(t => t.AssigneeId)
            .HasMaxLength(100);

        builder.Property(t => t.AssigneeName)
            .HasMaxLength(200);

        builder.Property(t => t.EstimatedPomodoros)
            .HasDefaultValue(0);

        builder.Property(t => t.CompletedPomodoros)
            .HasDefaultValue(0);

        builder.Property(t => t.IsCompleted)
            .HasDefaultValue(false);

        builder.Property(t => t.CompletedAt);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Pomodoros)
            .WithOne(p => p.Task)
            .HasForeignKey(p => p.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}