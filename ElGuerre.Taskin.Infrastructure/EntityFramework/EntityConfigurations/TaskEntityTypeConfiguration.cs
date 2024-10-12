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

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Deadline);

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