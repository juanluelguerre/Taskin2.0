using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Task : TrackedEntity
{
    public required string Description { get; set; }
    public Guid ProjectId { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime? Deadline { get; set; }
    public List<Pomodoro> Pomodoros { get; set; } = [];

    public required Project Project { get; set; }
}