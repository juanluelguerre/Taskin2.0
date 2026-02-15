namespace ElGuerre.Taskin.Application.Tasks.DTOs;

public record TaskListDto(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    Guid ProjectId,
    string? ProjectName,
    string? AssigneeId,
    string? AssigneeName,
    DateTime? DueDate,
    int EstimatedPomodoros,
    int CompletedPomodoros,
    string[] Tags,
    bool IsCompleted,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record TaskDetailsDto(
    Guid Id,
    string Title,
    string? Description,
    string? Notes,
    string Status,
    string Priority,
    Guid ProjectId,
    string? ProjectName,
    string? AssigneeId,
    string? AssigneeName,
    DateTime? DueDate,
    int EstimatedPomodoros,
    int CompletedPomodoros,
    string[] Tags,
    bool IsCompleted,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<PomodoroSummaryDto> Pomodoros
);

public record PomodoroSummaryDto(
    Guid Id,
    DateTime StartTime,
    int DurationInMinutes
);

public record TaskListResponse(
    List<TaskListDto> Items,
    int TotalCount,
    int Page,
    int Size
);

public record TaskStatsDto(
    int TotalTasks,
    int PendingTasks,
    int InProgressTasks,
    int CompletedTasks,
    int OverdueTasks,
    int TasksCompletedThisWeek,
    double AverageCompletionTime,
    int ProductivityScore
);

public record DashboardStatsDto(
    int ActiveProjects,
    int PendingTasks,
    int CompletedToday,
    int PomodorosToday,
    int WeeklyProgress,
    double FocusHours
);

public record RecentActivityDto(
    string Icon,
    string Title,
    string Time,
    DateTime Timestamp
);
