using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Pomodoros.Queries;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Queries.Pomodoros;

/// <summary>
/// Tests for GetPomodoroByIdQueryHandler
/// </summary>
public class GetPomodoroByIdQueryHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly GetPomodoroByIdQueryHandler handler;
    private DbSet<Pomodoro> pomodorosDbSet;

    public GetPomodoroByIdQueryHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();

        var pomodoros = new List<Pomodoro>();
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();

        this.context.Pomodoros.Returns(this.pomodorosDbSet);
        this.handler = new GetPomodoroByIdQueryHandler(this.context);
    }

    [Fact]
    public async SystemTask Handle_WithExistingPomodoro_ShouldReturnPomodoro()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var task = new DomainTask
        {
            Description = "Test Task",
            Status = DomainTaskStatus.Doing,
            Project = project
        };
        var pomodoro = new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25,
            Task = task
        };

        var pomodoros = new List<Pomodoro> { pomodoro };
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();
        this.context.Pomodoros.Returns(this.pomodorosDbSet);

        var query = new GetPomodoroByIdQuery { Id = pomodoro.Id };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(pomodoro.Id);
        result.DurationInMinutes.Should().Be(25);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingPomodoro_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var pomodoros = new List<Pomodoro>();
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();
        this.context.Pomodoros.Returns(this.pomodorosDbSet);

        var query = new GetPomodoroByIdQuery { Id = pomodoroId };

        // Act
        Func<SystemTask> act = async () => await this.handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<Pomodoro>>();
    }

    [Fact]
    public async SystemTask Handle_ShouldCallFirstOrDefaultAsync()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var task = new DomainTask
        {
            Description = "Test Task",
            Status = DomainTaskStatus.Todo,
            Project = project
        };
        var pomodoro = new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 30,
            Task = task
        };

        var pomodoros = new List<Pomodoro> { pomodoro };
        this.pomodorosDbSet = pomodoros.BuildMockDbSet();
        this.context.Pomodoros.Returns(this.pomodorosDbSet);

        var query = new GetPomodoroByIdQuery { Id = pomodoro.Id };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(pomodoro);
    }
}
