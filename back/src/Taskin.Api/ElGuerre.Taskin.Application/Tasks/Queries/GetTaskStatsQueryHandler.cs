using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTaskStatsQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTaskStatsQuery, TaskStatsResponse>
{
    public async Task<TaskStatsResponse> Handle(GetTaskStatsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Tasks.AsQueryable();
        
        // Apply project filter if specified
        if (request.ProjectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == request.ProjectId.Value);
        }

        var tasks = await query.ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var weekStart = now.AddDays(-(int)now.DayOfWeek);

        var totalTasks = tasks.Count;
        var pendingTasks = tasks.Count(t => t.Status == ElGuerre.Taskin.Domain.Entities.TaskStatus.Pending);
        var inProgressTasks = tasks.Count(t => t.Status == ElGuerre.Taskin.Domain.Entities.TaskStatus.InProgress);
        var completedTasks = tasks.Count(t => t.Status == ElGuerre.Taskin.Domain.Entities.TaskStatus.Completed);
        var cancelledTasks = tasks.Count(t => t.Status == ElGuerre.Taskin.Domain.Entities.TaskStatus.Cancelled);
        
        var overdueTasks = tasks.Count(t => 
            t.DueDate.HasValue && 
            t.DueDate.Value < now && 
            !t.IsCompleted);

        var completedThisWeek = tasks.Count(t => 
            t.CompletedAt.HasValue && 
            t.CompletedAt.Value >= weekStart);

        // Calculate average completion time for completed tasks
        var completedTasksWithDates = tasks
            .Where(t => t.IsCompleted && t.CompletedAt.HasValue)
            .ToList();

        double averageCompletionTime = 0;
        if (completedTasksWithDates.Count > 0)
        {
            var totalDays = completedTasksWithDates
                .Sum(t => (t.CompletedAt!.Value - t.CreatedOn).TotalDays);
            averageCompletionTime = totalDays / completedTasksWithDates.Count;
        }

        // Calculate productivity score (simple algorithm)
        var productivityScore = CalculateProductivityScore(
            totalTasks, 
            completedTasks, 
            overdueTasks, 
            completedThisWeek);

        return new TaskStatsResponse
        {
            TotalTasks = totalTasks,
            PendingTasks = pendingTasks,
            InProgressTasks = inProgressTasks,
            CompletedTasks = completedTasks,
            CancelledTasks = cancelledTasks,
            OverdueTasks = overdueTasks,
            TasksCompletedThisWeek = completedThisWeek,
            AverageCompletionTime = Math.Round(averageCompletionTime, 1),
            ProductivityScore = productivityScore
        };
    }

    private static int CalculateProductivityScore(int total, int completed, int overdue, int completedThisWeek)
    {
        if (total == 0) return 100;

        var completionRate = (double)completed / total;
        var overdueRate = (double)overdue / total;
        var weeklyActivity = Math.Min(completedThisWeek / 5.0, 1.0); // Max 1 task per day average

        // Simple scoring algorithm: 
        // - 60% weight on completion rate
        // - 20% penalty for overdue tasks
        // - 20% bonus for recent activity
        var score = (completionRate * 60) - (overdueRate * 20) + (weeklyActivity * 20);
        
        return Math.Max(0, Math.Min(100, (int)Math.Round(score)));
    }
}