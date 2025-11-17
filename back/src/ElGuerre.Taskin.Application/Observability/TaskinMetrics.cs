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
        // Project metrics
        _projectsCreated = _meter.CreateCounter<long>(
            "taskin.projects.created",
            unit: "projects",
            description: "Number of projects created");

        _projectsDeleted = _meter.CreateCounter<long>(
            "taskin.projects.deleted",
            unit: "projects",
            description: "Number of projects deleted");

        _activeProjects = _meter.CreateUpDownCounter<long>(
            "taskin.projects.active",
            unit: "projects",
            description: "Number of active projects");

        // Task metrics
        _tasksCreated = _meter.CreateCounter<long>(
            "taskin.tasks.created",
            unit: "tasks",
            description: "Number of tasks created");

        _tasksCompleted = _meter.CreateCounter<long>(
            "taskin.tasks.completed",
            unit: "tasks",
            description: "Number of tasks completed");

        _tasksDeleted = _meter.CreateCounter<long>(
            "taskin.tasks.deleted",
            unit: "tasks",
            description: "Number of tasks deleted");

        _activeTasks = _meter.CreateUpDownCounter<long>(
            "taskin.tasks.active",
            unit: "tasks",
            description: "Number of active tasks");

        // Pomodoro metrics
        _pomodorosCreated = _meter.CreateCounter<long>(
            "taskin.pomodoros.created",
            unit: "pomodoros",
            description: "Number of pomodoros created");

        _pomodorosCompleted = _meter.CreateCounter<long>(
            "taskin.pomodoros.completed",
            unit: "pomodoros",
            description: "Number of pomodoros completed");

        _pomodoroDuration = _meter.CreateHistogram<double>(
            "taskin.pomodoros.duration",
            unit: "minutes",
            description: "Duration of pomodoros in minutes");
    }

    // Project operations
    public void RecordProjectCreated() => _projectsCreated.Add(1);
    public void RecordProjectDeleted() => _projectsDeleted.Add(1);
    public void UpdateActiveProjects(int count) => _activeProjects.Add(count - _activeProjects.GetType().GetHashCode()); // Simple update

    // Task operations
    public void RecordTaskCreated() => _tasksCreated.Add(1);
    public void RecordTaskCompleted() => _tasksCompleted.Add(1);
    public void RecordTaskDeleted() => _tasksDeleted.Add(1);
    public void UpdateActiveTasks(int count) => _activeTasks.Add(count - _activeTasks.GetType().GetHashCode()); // Simple update

    // Pomodoro operations
    public void RecordPomodoroCreated() => _pomodorosCreated.Add(1);
    public void RecordPomodoroCompleted() => _pomodorosCompleted.Add(1);
    public void RecordPomodoroDuration(double minutes) => _pomodoroDuration.Record(minutes);
}
