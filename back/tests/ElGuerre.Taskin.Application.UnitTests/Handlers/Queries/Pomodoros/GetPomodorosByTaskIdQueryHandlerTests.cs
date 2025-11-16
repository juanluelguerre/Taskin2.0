using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Pomodoros.Queries;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Queries.Pomodoros;

/// <summary>
/// Tests for GetPomodorosByTaskIdQueryHandler
/// </summary>
public class GetPomodorosByTaskIdQueryHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly GetPomodorosByTaskIdQueryHandler handler;
    private DbSet<Pomodoro> pomodorosDbSet;

    public GetPomodorosByTaskIdQueryHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();

        var pomodoros = new List<Pomodoro>();
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();

        this.context.Pomodoros.Returns(this.pomodorosDbSet);
        this.handler = new GetPomodorosByTaskIdQueryHandler(this.context);
    }

    [Fact]
    public async SystemTask Handle_WithExistingPomodoros_ShouldReturnPomodoros()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };
        var task = new DomainTask
        {
            Description = "Test Task",
            Status = DomainTaskStatus.Doing,
            Project = project
        };
        var pomodoro1 = new Pomodoro
        {
            StartTime = DateTime.UtcNow.AddHours(-2),
            DurationInMinutes = 25,
            TaskId = taskId,
            Task = task
        };
        var pomodoro2 = new Pomodoro
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            DurationInMinutes = 30,
            TaskId = taskId,
            Task = task
        };

        var pomodoros = new List<Pomodoro> { pomodoro1, pomodoro2 };
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();
        this.context.Pomodoros.Returns(this.pomodorosDbSet);

        var query = new GetPomodorosByTaskIdQuery { TaskId = taskId };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.DurationInMinutes == 25);
        result.Should().Contain(p => p.DurationInMinutes == 30);
    }

    [Fact]
    public async SystemTask Handle_WithNoPomodoros_ShouldReturnEmptyList()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var pomodoros = new List<Pomodoro>();
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();
        this.context.Pomodoros.Returns(this.pomodorosDbSet);

        var query = new GetPomodorosByTaskIdQuery { TaskId = taskId };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async SystemTask Handle_ShouldFilterByTaskId()
    {
        // Arrange
        var taskId1 = Guid.NewGuid();
        var taskId2 = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };
        var task1 = new DomainTask
        {
            Description = "Task 1",
            Status = DomainTaskStatus.Doing,
            Project = project
        };
        var task2 = new DomainTask
        {
            Description = "Task 2",
            Status = DomainTaskStatus.Todo,
            Project = project
        };

        var pomodoro1 = new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25,
            TaskId = taskId1,
            Task = task1
        };
        var pomodoro2 = new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 30,
            TaskId = taskId2,
            Task = task2
        };

        var pomodoros = new List<Pomodoro> { pomodoro1, pomodoro2 };
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();
        this.context.Pomodoros.Returns(this.pomodorosDbSet);

        var query = new GetPomodorosByTaskIdQuery { TaskId = taskId1 };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().TaskId.Should().Be(taskId1);
        result.First().DurationInMinutes.Should().Be(25);
    }
}
