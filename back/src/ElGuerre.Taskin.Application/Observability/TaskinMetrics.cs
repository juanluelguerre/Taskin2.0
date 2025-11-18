using System.Diagnostics.Metrics;

namespace ElGuerre.Taskin.Application.Observability;

/// <summary>
/// Custom business metrics for Taskin application
/// </summary>
public class TaskinMetrics
{
    private readonly Counter<long> _projectsCreated;
    private readonly Counter<long> _projectsDeleted;
    private readonly Counter<long> _tasksCreated;
    private readonly Counter<long> _tasksCompleted;
    private readonly Counter<long> _tasksDeleted;
    private readonly Counter<long> _pomodorosCreated;
    private readonly Counter<long> _pomodorosCompleted;
    private readonly Histogram<double> _pomodoroDuration;
    private readonly UpDownCounter<long> _activeProjects;
    private readonly UpDownCounter<long> _activeTasks;

    private static readonly Meter _meter = new("Taskin.Application", "1.0.0");

    public TaskinMetrics()
    {
        var meter = _meter;

        // Project metrics
        _projectsCreated = meter.CreateCounter<long>(
            "taskin_projects_created_total",
            unit: "projects",
            description: "Total number of projects created");

        _projectsDeleted = meter.CreateCounter<long>(
            "taskin_projects_deleted_total",
            unit: "projects",
            description: "Total number of projects deleted");

        _activeProjects = meter.CreateUpDownCounter<long>(
            "taskin_projects_active",
            unit: "projects",
            description: "Number of active projects");

        // Task metrics
        _tasksCreated = meter.CreateCounter<long>(
            "taskin_tasks_created_total",
            unit: "tasks",
            description: "Total number of tasks created");

        _tasksCompleted = meter.CreateCounter<long>(
            "taskin_tasks_completed_total",
            unit: "tasks",
            description: "Total number of tasks completed");

        _tasksDeleted = meter.CreateCounter<long>(
            "taskin_tasks_deleted_total",
            unit: "tasks",
            description: "Total number of tasks deleted");

        _activeTasks = meter.CreateUpDownCounter<long>(
            "taskin_tasks_active",
            unit: "tasks",
            description: "Number of active tasks");

        // Pomodoro metrics
        _pomodorosCreated = meter.CreateCounter<long>(
            "taskin_pomodoros_created_total",
            unit: "pomodoros",
            description: "Total number of pomodoros created");

        _pomodorosCompleted = meter.CreateCounter<long>(
            "taskin_pomodoros_completed_total",
            unit: "pomodoros",
            description: "Total number of pomodoros completed");

        _pomodoroDuration = meter.CreateHistogram<double>(
            "taskin_pomodoros_duration_minutes",
            unit: "minutes",
            description: "Duration of pomodoros in minutes");
    }

    // Project operations
    public void RecordProjectCreated() => _projectsCreated.Add(1);
    public void RecordProjectDeleted() => _projectsDeleted.Add(1);
    public void IncrementActiveProjects() => _activeProjects.Add(1);
    public void DecrementActiveProjects() => _activeProjects.Add(-1);

    // Task operations
    public void RecordTaskCreated() => _tasksCreated.Add(1);
    public void RecordTaskCompleted() => _tasksCompleted.Add(1);
    public void RecordTaskDeleted() => _tasksDeleted.Add(1);
    public void IncrementActiveTasks() => _activeTasks.Add(1);
    public void DecrementActiveTasks() => _activeTasks.Add(-1);

    // Pomodoro operations
    public void RecordPomodoroCreated() => _pomodorosCreated.Add(1);
    public void RecordPomodoroCompleted() => _pomodorosCompleted.Add(1);
    public void RecordPomodoroDuration(double minutes) => _pomodoroDuration.Record(minutes);
}
