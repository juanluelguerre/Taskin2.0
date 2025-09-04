using ElGuerre.Taskin.Domain.Entities;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;
using DomainTaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.DTOs;

public class TaskListResponse
{
    public List<DomainTask> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

public class TaskStatsResponse
{
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int CancelledTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TasksCompletedThisWeek { get; set; }
    public double AverageCompletionTime { get; set; } // in days
    public int ProductivityScore { get; set; } // 0-100
}

public class TaskFilters
{
    public DomainTaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public Guid? ProjectId { get; set; }
    public string? AssigneeId { get; set; }
    public List<string>? Tags { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsCompleted { get; set; }
}