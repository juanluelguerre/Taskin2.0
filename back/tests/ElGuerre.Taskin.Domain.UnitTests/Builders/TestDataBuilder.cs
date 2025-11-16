using Bogus;
using ElGuerre.Taskin.Domain.Entities;

namespace ElGuerre.Taskin.Domain.UnitTests.Builders;

/// <summary>
/// Test data builder using Bogus for generating test entities
/// </summary>
public static class TestDataBuilder
{
    private static readonly Faker<Project> ProjectFaker = new Faker<Project>()
        .CustomInstantiator(f => new Project
        {
            Name = f.Commerce.ProductName() // Required property
        })
        .RuleFor(p => p.Description, f => f.Lorem.Sentence())
        .RuleFor(p => p.Status, f => f.PickRandom<ProjectStatus>())
        .RuleFor(p => p.DueDate, f => f.Date.Future())
        .RuleFor(p => p.ImageUrl, f => f.Internet.Avatar())
        .RuleFor(p => p.BackgroundColor, f => $"#{f.Random.Hexadecimal(6, String.Empty)}");

    private static readonly Faker<Domain.Entities.Task> TaskFaker = new Faker<Domain.Entities.Task>()
        .CustomInstantiator(f => new Domain.Entities.Task
        {
            Description = f.Lorem.Sentence(), // Required property
            Project = null! // Required navigation property - null in unit tests
        })
        .RuleFor(t => t.Status, f => f.PickRandom<ElGuerre.Taskin.Domain.Entities.TaskStatus>())
        .RuleFor(t => t.Deadline, f => f.Date.Future())
        .RuleFor(t => t.ProjectId, f => Guid.NewGuid());

    private static readonly Faker<Pomodoro> PomodoroFaker = new Faker<Pomodoro>()
        .CustomInstantiator(f => new Pomodoro
        {
            Task = null! // Required navigation property - null in unit tests
        })
        .RuleFor(p => p.StartTime, f => f.Date.Recent())
        .RuleFor(p => p.DurationInMinutes, f => f.Random.Int(1, 480))
        .RuleFor(p => p.TaskId, f => Guid.NewGuid());

    /// <summary>
    /// Creates a single Project with random data
    /// </summary>
    public static Project CreateProject(Action<Project>? customize = null)
    {
        var project = ProjectFaker.Generate();
        customize?.Invoke(project);
        return project;
    }

    /// <summary>
    /// Creates multiple Projects with random data
    /// </summary>
    public static List<Project> CreateProjects(int count, Action<Project>? customize = null)
    {
        var projects = ProjectFaker.Generate(count);
        if (customize != null)
        {
            projects.ForEach(customize);
        }
        return projects;
    }

    /// <summary>
    /// Creates a single Task with random data
    /// </summary>
    public static Domain.Entities.Task CreateTask(Action<Domain.Entities.Task>? customize = null)
    {
        var task = TaskFaker.Generate();
        customize?.Invoke(task);
        return task;
    }

    /// <summary>
    /// Creates multiple Tasks with random data
    /// </summary>
    public static List<Domain.Entities.Task> CreateTasks(int count, Action<Domain.Entities.Task>? customize = null)
    {
        var tasks = TaskFaker.Generate(count);
        if (customize != null)
        {
            tasks.ForEach(customize);
        }
        return tasks;
    }

    /// <summary>
    /// Creates a single Pomodoro with random data
    /// </summary>
    public static Pomodoro CreatePomodoro(Action<Pomodoro>? customize = null)
    {
        var pomodoro = PomodoroFaker.Generate();
        customize?.Invoke(pomodoro);
        return pomodoro;
    }

    /// <summary>
    /// Creates multiple Pomodoros with random data
    /// </summary>
    public static List<Pomodoro> CreatePomodoros(int count, Action<Pomodoro>? customize = null)
    {
        var pomodoros = PomodoroFaker.Generate(count);
        if (customize != null)
        {
            pomodoros.ForEach(customize);
        }
        return pomodoros;
    }
}
