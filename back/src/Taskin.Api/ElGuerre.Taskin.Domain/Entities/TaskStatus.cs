namespace ElGuerre.Taskin.Domain.Entities;

public enum TaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    // Keep old values for backward compatibility
    Todo = Pending,
    Doing = InProgress,
    Done = Completed
}
