using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.DTOs;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetDashboardStatsQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var weekStart = now.AddDays(-(int)now.DayOfWeek).Date;

        var activeProjects = await context.Projects
            .CountAsync(p => p.Status == ProjectStatus.Active, cancellationToken);

        var pendingTasks = await context.Tasks
            .CountAsync(t => t.Status == TaskStatus.Todo || t.Status == TaskStatus.Doing, cancellationToken);

        var completedToday = await context.Tasks
            .CountAsync(t => t.IsCompleted && t.CompletedAt.HasValue && t.CompletedAt.Value >= todayStart, cancellationToken);

        var pomodorosToday = await context.Pomodoros
            .CountAsync(p => p.StartTime >= todayStart, cancellationToken);

        // Weekly progress: % of tasks completed this week out of all tasks worked on this week
        var totalTasks = await context.Tasks.CountAsync(cancellationToken);
        var completedTasks = await context.Tasks.CountAsync(t => t.Status == TaskStatus.Done, cancellationToken);
        var weeklyProgress = totalTasks > 0 ? (int)Math.Round((double)completedTasks / totalTasks * 100) : 0;

        // Focus hours: total pomodoro minutes this week / 60
        var weeklyPomodoroMinutes = await context.Pomodoros
            .Where(p => p.StartTime >= weekStart)
            .SumAsync(p => p.DurationInMinutes, cancellationToken);
        var focusHours = Math.Round(weeklyPomodoroMinutes / 60.0, 1);

        return new DashboardStatsDto(
            activeProjects,
            pendingTasks,
            completedToday,
            pomodorosToday,
            weeklyProgress,
            focusHours
        );
    }
}
