using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Task : TrackedEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public Guid ProjectId { get; set; }
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? Deadline { get; set; } // Keep for backward compatibility
    public int? EstimatedPomodoros { get; set; }
    public int CompletedPomodoros { get; set; } = 0;
    public List<string> Tags { get; set; } = [];
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public List<Pomodoro> Pomodoros { get; set; } = [];

    public required Project Project { get; set; }

    // Computed properties
    public string ProjectName => Project?.Name ?? "Unknown Project";
    
    public void MarkAsCompleted()
    {
        Status = TaskStatus.Done;
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        SetModificationInfo();
    }
    
    public void MarkAsIncomplete()
    {
        Status = TaskStatus.Todo;
        IsCompleted = false;
        CompletedAt = null;
        SetModificationInfo();
    }
    
    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            Tags.Add(tag.Trim());
            SetModificationInfo();
        }
    }
    
    public void RemoveTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag))
        {
            Tags.RemoveAll(t => string.Equals(t, tag.Trim(), StringComparison.OrdinalIgnoreCase));
            SetModificationInfo();
        }
    }
    
    public void UpdateProgress()
    {
        CompletedPomodoros = Pomodoros?.Count(p => p.Status == PomodoroStatus.Completed) ?? 0;
        SetModificationInfo();
    }
}