using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTaskStatsQuery : IRequest<TaskStatsResponse>
{
    public Guid? ProjectId { get; set; } // Optional filter by project
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