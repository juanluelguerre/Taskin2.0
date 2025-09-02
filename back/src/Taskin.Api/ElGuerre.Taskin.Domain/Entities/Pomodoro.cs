using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Pomodoro : TrackedEntity
{
    public Guid TaskId { get; set; }
    public PomodoroStatus Status { get; set; } = PomodoroStatus.Pending;
    public PomodoroType Type { get; set; } = PomodoroType.Work;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int PlannedDurationInMinutes { get; set; } = 25;
    public int? ActualDurationInMinutes { get; set; }
    public int PausedTimeInSeconds { get; set; } = 0;
    public string? Notes { get; set; }
    public int Interruptions { get; set; } = 0;
    
    // Keep for backward compatibility
    public int DurationInMinutes 
    { 
        get => PlannedDurationInMinutes;
        set => PlannedDurationInMinutes = value;
    }

    // A pomodoro required a Task. A pomodoro cannot exist without a task.
    public required Task Task { get; set; }
    
    // Business methods
    public void Start()
    {
        Status = PomodoroStatus.InProgress;
        StartTime = DateTime.UtcNow;
        SetModificationInfo();
    }
    
    public void Complete()
    {
        Status = PomodoroStatus.Completed;
        EndTime = DateTime.UtcNow;
        if (StartTime.HasValue)
        {
            ActualDurationInMinutes = (int)(EndTime.Value - StartTime.Value).TotalMinutes;
        }
        SetModificationInfo();
    }
    
    public void Cancel()
    {
        Status = PomodoroStatus.Cancelled;
        EndTime = DateTime.UtcNow;
        SetModificationInfo();
    }
    
    public void Pause()
    {
        if (Status == PomodoroStatus.InProgress)
        {
            Status = PomodoroStatus.Paused;
            SetModificationInfo();
        }
    }
    
    public void Resume()
    {
        if (Status == PomodoroStatus.Paused)
        {
            Status = PomodoroStatus.InProgress;
            SetModificationInfo();
        }
    }
}