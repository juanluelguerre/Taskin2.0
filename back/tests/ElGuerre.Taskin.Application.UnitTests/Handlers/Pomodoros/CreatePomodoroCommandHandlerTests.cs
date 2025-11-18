using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Pomodoros.Commands;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Pomodoros;

/// <summary>
/// Tests for CreatePomodoroCommandHandler
/// </summary>
public class CreatePomodoroCommandHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly IUnitOfWork unitOfWork;
    private readonly CreatePomodoroCommandHandler _handler;
    private readonly DbSet<Pomodoro> _pomodorosDbSet;
    private DbSet<DomainTask> _tasksDbSet;

    public CreatePomodoroCommandHandlerTests()
    {
        context = Substitute.For<ITaskinDbContext>();
        unitOfWork = Substitute.For<IUnitOfWork>();

        var pomodoros = new List<Pomodoro>();
        _pomodorosDbSet = pomodoros.BuildMockDbSet();

        var tasks = new List<DomainTask>();
        _tasksDbSet = tasks.BuildMockDbSet();

        context.Pomodoros.Returns(_pomodorosDbSet);
        context.Tasks.Returns(_tasksDbSet);
        _handler = new CreatePomodoroCommandHandler(context, unitOfWork, new ElGuerre.Taskin.Application.Observability.TaskinMetrics());
    }

    [Fact]
    public async SystemTask Handle_WithValidCommand_ShouldCreatePomodoro()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new DomainTask
        {
            Description = "Test Task",
            Status = DomainTaskStatus.Doing,
            Project = new Project { Name = "Test" }
        };

        var command = new CreatePomodoroCommand
        {
            TaskId = taskId,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        var tasks = new List<DomainTask> { task };
        _tasksDbSet = tasks.BuildMockDbSet();
        _tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(task);
        context.Tasks.Returns(_tasksDbSet);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _pomodorosDbSet.Received(1).Add(Arg.Is<Pomodoro>(p =>
            p.TaskId == command.TaskId &&
            p.StartTime == command.StartTime &&
            p.DurationInMinutes == command.DurationInMinutes &&
            p.Task == task));
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingTask_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new CreatePomodoroCommand
        {
            TaskId = taskId,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        var tasks = new List<DomainTask>();
        _tasksDbSet = tasks.BuildMockDbSet();
        context.Tasks.Returns(_tasksDbSet);

        // Act
        Func<SystemTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<DomainTask>>();
        _pomodorosDbSet.DidNotReceive().Add(Arg.Any<Pomodoro>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldReturnPomodoroId()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new DomainTask { Description = "Test Task", Status = DomainTaskStatus.Doing, Project = new Project { Name = "Test" } };
        var command = new CreatePomodoroCommand
        {
            TaskId = taskId,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        var tasks = new List<DomainTask> { task };
        _tasksDbSet = tasks.BuildMockDbSet();
        _tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(task);
        context.Tasks.Returns(_tasksDbSet);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async SystemTask Handle_WithDifferentDurations_ShouldCreatePomodoroWithCorrectDuration()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new DomainTask { Description = "Test Task", Status = DomainTaskStatus.Doing, Project = new Project { Name = "Test" } };
        var command = new CreatePomodoroCommand
        {
            TaskId = taskId,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 50 // Extended pomodoro
        };

        var tasks = new List<DomainTask> { task };
        _tasksDbSet = tasks.BuildMockDbSet();
        _tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(task);
        context.Tasks.Returns(_tasksDbSet);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _pomodorosDbSet.Received(1).Add(Arg.Is<Pomodoro>(p =>
            p.DurationInMinutes == 50));
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new DomainTask { Description = "Test Task", Status = DomainTaskStatus.Doing, Project = new Project { Name = "Test" } };
        var command = new CreatePomodoroCommand
        {
            TaskId = taskId,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        var tasks = new List<DomainTask> { task };
        _tasksDbSet = tasks.BuildMockDbSet();
        _tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(task);
        context.Tasks.Returns(_tasksDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
