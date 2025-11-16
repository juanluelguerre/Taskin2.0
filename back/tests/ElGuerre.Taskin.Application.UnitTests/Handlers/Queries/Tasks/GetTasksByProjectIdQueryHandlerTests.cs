using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.Queries;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Queries.Tasks;

/// <summary>
/// Tests for GetTasksByProjectIdQueryHandler
/// </summary>
public class GetTasksByProjectIdQueryHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly GetTasksByProjectIdQueryHandler handler;
    private DbSet<DomainTask> tasksDbSet;

    public GetTasksByProjectIdQueryHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();

        var tasks = new List<DomainTask>();
        this.tasksDbSet = tasks.BuildMockDbSet();

        this.context.Tasks.Returns(this.tasksDbSet);
        this.handler = new GetTasksByProjectIdQueryHandler(this.context);
    }

    [Fact]
    public async SystemTask Handle_WithExistingTasks_ShouldReturnTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };
        var task1 = new DomainTask
        {
            Description = "Task 1",
            Status = DomainTaskStatus.Todo,
            ProjectId = projectId,
            Project = project
        };
        var task2 = new DomainTask
        {
            Description = "Task 2",
            Status = DomainTaskStatus.Doing,
            ProjectId = projectId,
            Project = project
        };

        var tasks = new List<DomainTask> { task1, task2 };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.context.Tasks.Returns(this.tasksDbSet);

        var query = new GetTasksByProjectIdQuery { ProjectId = projectId };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Description == "Task 1");
        result.Should().Contain(t => t.Description == "Task 2");
    }

    [Fact]
    public async SystemTask Handle_WithNoTasks_ShouldReturnEmptyList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var tasks = new List<DomainTask>();
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.context.Tasks.Returns(this.tasksDbSet);

        var query = new GetTasksByProjectIdQuery { ProjectId = projectId };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async SystemTask Handle_ShouldIncludePomodoros()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };
        var task = new DomainTask
        {
            Description = "Task with Pomodoros",
            Status = DomainTaskStatus.Doing,
            ProjectId = projectId,
            Project = project
        };
        task.Pomodoros.Add(new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25,
            Task = task
        });
        task.Pomodoros.Add(new Pomodoro
        {
            StartTime = DateTime.UtcNow.AddMinutes(30),
            DurationInMinutes = 25,
            Task = task
        });

        var tasks = new List<DomainTask> { task };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.context.Tasks.Returns(this.tasksDbSet);

        var query = new GetTasksByProjectIdQuery { ProjectId = projectId };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Pomodoros.Should().HaveCount(2);
    }
}
