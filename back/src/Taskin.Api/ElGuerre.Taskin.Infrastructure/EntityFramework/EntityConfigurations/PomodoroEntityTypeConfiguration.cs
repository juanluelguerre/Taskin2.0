using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElGuerre.Taskin.Infrastructure.EntityFramework.EntityConfigurations;

public class PomodoroEntityTypeConfiguration : IEntityTypeConfiguration<Pomodoro>
{
    public void Configure(EntityTypeBuilder<Pomodoro> builder)
    {
        builder.ToTable("Pomodoros");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.StartTime)
            .IsRequired();

        builder.Property(p => p.DurationInMinutes)
            .IsRequired();

        builder.HasOne(p => p.Task)
            .WithMany(t => t.Pomodoros)
            .HasForeignKey(p => p.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}