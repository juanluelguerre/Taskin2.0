using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTaskStatsQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTaskStatsQuery, TaskStatsDto>
{
    public async Task<TaskStatsDto> Handle(GetTaskStatsQuery request, CancellationToken cancellationToken)
    {
        var tasks = await context.Tasks.ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var weekStart = now.AddDays(-(int)now.DayOfWeek);

        var totalTasks = tasks.Count;
        var pendingTasks = tasks.Count(t => t.Status == TaskStatus.Todo);
        var inProgressTasks = tasks.Count(t => t.Status == TaskStatus.Doing);
        var completedTasks = tasks.Count(t => t.Status == TaskStatus.Done);
        var overdueTasks = tasks.Count(t => t.Deadline.HasValue && t.Deadline.Value < now && !t.IsCompleted);
        var completedThisWeek = tasks.Count(t =>
            t.IsCompleted && t.CompletedAt.HasValue && t.CompletedAt.Value >= weekStart);

        var completedWithTime = tasks
            .Where(t => t.IsCompleted && t.CompletedAt.HasValue)
            .Select(t => (t.CompletedAt!.Value - t.CreatedOn.DateTime).TotalHours)
            .ToList();
        var avgCompletionTime = completedWithTime.Count > 0 ? completedWithTime.Average() : 0;

        var productivityScore = totalTasks > 0
            ? (int)Math.Round((double)completedTasks / totalTasks * 100)
            : 0;

        return new TaskStatsDto(
            totalTasks,
            pendingTasks,
            inProgressTasks,
            completedTasks,
            overdueTasks,
            completedThisWeek,
            Math.Round(avgCompletionTime, 1),
            productivityScore
        );
    }
}
