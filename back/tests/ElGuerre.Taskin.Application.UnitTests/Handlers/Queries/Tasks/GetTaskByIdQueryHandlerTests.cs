using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Tasks.Queries;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Queries.Tasks;

/// <summary>
/// Tests for GetTaskByIdQueryHandler
/// </summary>
public class GetTaskByIdQueryHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly GetTaskByIdQueryHandler handler;
    private DbSet<DomainTask> tasksDbSet;

    public GetTaskByIdQueryHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();

        var tasks = new List<DomainTask>();
        this.tasksDbSet = tasks.BuildMockDbSet();

        this.context.Tasks.Returns(this.tasksDbSet);
        this.handler = new GetTaskByIdQueryHandler(this.context);
    }

    [Fact]
    public async SystemTask Handle_WithExistingTask_ShouldReturnTask()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var task = new DomainTask
        {
            Description = "Test Task",
            Status = DomainTaskStatus.Doing,
            Project = project
        };
        task.Pomodoros.Add(new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25,
            Task = task
        });

        var tasks = new List<DomainTask> { task };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.context.Tasks.Returns(this.tasksDbSet);

        var query = new GetTaskByIdQuery { Id = task.Id };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
        result.Description.Should().Be("Test Task");
        result.Status.Should().Be(DomainTaskStatus.Doing);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingTask_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var tasks = new List<DomainTask>();
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.context.Tasks.Returns(this.tasksDbSet);

        var query = new GetTaskByIdQuery { Id = taskId };

        // Act
        Func<SystemTask> act = async () => await this.handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<DomainTask>>();
    }

    [Fact]
    public async SystemTask Handle_ShouldIncludePomodoros()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var task = new DomainTask
        {
            Description = "Test Task",
            Status = DomainTaskStatus.Todo,
            Project = project
        };
        task.Pomodoros.Add(new Pomodoro
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            DurationInMinutes = 25,
            Task = task
        });
        task.Pomodoros.Add(new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 30,
            Task = task
        });

        var tasks = new List<DomainTask> { task };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.context.Tasks.Returns(this.tasksDbSet);

        var query = new GetTaskByIdQuery { Id = task.Id };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Pomodoros.Should().HaveCount(2);
    }
}
