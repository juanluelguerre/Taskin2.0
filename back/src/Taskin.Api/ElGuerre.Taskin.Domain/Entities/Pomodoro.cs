using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Pomodoro : TrackedEntity
{
    public Guid TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }

    // A pomodoro required a Task. A pomodoro cannot exist without a task.
    public required Task Task { get; set; }
}