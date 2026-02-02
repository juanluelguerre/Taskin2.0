using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Task : TrackedEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? Deadline { get; set; }
    public string? Tags { get; set; }
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int EstimatedPomodoros { get; set; }
    public int CompletedPomodoros { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<Pomodoro> Pomodoros { get; set; } = [];

    public required Project Project { get; set; }
}