using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetRecentActivityQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetRecentActivityQuery, List<RecentActivityDto>>
{
    public async Task<List<RecentActivityDto>> Handle(GetRecentActivityQuery request, CancellationToken cancellationToken)
    {
        var activities = new List<RecentActivityDto>();

        // 1. Recently completed tasks
        var completedTasks = await context.Tasks
            .Where(t => t.IsCompleted && t.CompletedAt != null)
            .OrderByDescending(t => t.CompletedAt)
            .Take(request.Limit)
            .Select(t => new { t.Title, t.CompletedAt })
            .ToListAsync(cancellationToken);

        foreach (var task in completedTasks)
        {
            activities.Add(new RecentActivityDto(
                Icon: "check_circle",
                Title: $"Completed task \"{task.Title}\"",
                Time: FormatRelativeTime(task.CompletedAt!.Value),
                Timestamp: task.CompletedAt.Value
            ));
        }

        // 2. Recently created tasks (not completed)
        var recentTasks = await context.Tasks
            .Where(t => !t.IsCompleted)
            .OrderByDescending(t => t.CreatedOn)
            .Take(request.Limit)
            .Select(t => new { t.Title, t.CreatedOn, t.Status })
            .ToListAsync(cancellationToken);

        foreach (var task in recentTasks)
        {
            var icon = task.Status == TaskStatus.Doing ? "play_circle" : "add_task";
            var verb = task.Status == TaskStatus.Doing ? "Started working on" : "Created task";

            activities.Add(new RecentActivityDto(
                Icon: icon,
                Title: $"{verb} \"{task.Title}\"",
                Time: FormatRelativeTime(task.CreatedOn.DateTime),
                Timestamp: task.CreatedOn.DateTime
            ));
        }

        // 3. Recent pomodoro sessions
        var recentPomodoros = await context.Pomodoros
            .OrderByDescending(p => p.StartTime)
            .Take(request.Limit)
            .Include(p => p.Task)
            .Select(p => new { p.StartTime, p.DurationInMinutes, TaskTitle = p.Task.Title })
            .ToListAsync(cancellationToken);

        foreach (var pomo in recentPomodoros)
        {
            activities.Add(new RecentActivityDto(
                Icon: "timer",
                Title: $"Completed {pomo.DurationInMinutes}min pomodoro on \"{pomo.TaskTitle}\"",
                Time: FormatRelativeTime(pomo.StartTime),
                Timestamp: pomo.StartTime
            ));
        }

        // 4. Recently created projects
        var recentProjects = await context.Projects
            .OrderByDescending(p => p.CreatedOn)
            .Take(5)
            .Select(p => new { p.Name, p.CreatedOn })
            .ToListAsync(cancellationToken);

        foreach (var project in recentProjects)
        {
            activities.Add(new RecentActivityDto(
                Icon: "folder",
                Title: $"Project \"{project.Name}\" created",
                Time: FormatRelativeTime(project.CreatedOn.DateTime),
                Timestamp: project.CreatedOn.DateTime
            ));
        }

        // Sort all activities by timestamp descending and take the limit
        return activities
            .OrderByDescending(a => a.Timestamp)
            .Take(request.Limit)
            .ToList();
    }

    private static string FormatRelativeTime(DateTime utcTime)
    {
        var diff = DateTime.UtcNow - utcTime;

        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
        if (diff.TotalDays < 30) return $"{(int)(diff.TotalDays / 7)}w ago";
        return utcTime.ToString("MMM dd, yyyy");
    }
}
