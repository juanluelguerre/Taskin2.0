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
/// Tests for UpdatePomodoroCommandHandler
/// </summary>
public class UpdatePomodoroCommandHandlerTests
{
    private readonly ITaskinDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdatePomodoroCommandHandler _handler;
    private DbSet<Pomodoro> _pomodorosDbSet;

    public UpdatePomodoroCommandHandlerTests()
    {
        _context = Substitute.For<ITaskinDbContext>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        var pomodoros = new List<Pomodoro>();
        _pomodorosDbSet = pomodoros.BuildMockDbSet();

        _context.Pomodoros.Returns(_pomodorosDbSet);
        _handler = new UpdatePomodoroCommandHandler(_context, _unitOfWork);
    }

    [Fact]
    public async SystemTask Handle_WithExistingPomodoro_ShouldUpdatePomodoro()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var existingPomodoro = new Pomodoro
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            DurationInMinutes = 25,
            Task = new DomainTask { Description = "Test", Project = new Project { Name = "Test" } }
        };

        var newStartTime = DateTime.UtcNow;
        var command = new UpdatePomodoroCommand
        {
            Id = pomodoroId,
            StartTime = newStartTime,
            DurationInMinutes = 50
        };

        var pomodoros = new List<Pomodoro> { existingPomodoro };
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _pomodorosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(existingPomodoro);
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingPomodoro.StartTime.Should().Be(newStartTime);
        existingPomodoro.DurationInMinutes.Should().Be(50);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingPomodoro_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var command = new UpdatePomodoroCommand
        {
            Id = pomodoroId,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        var pomodoros = new List<Pomodoro>();
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        Func<SystemTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<Pomodoro>>();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_WithNullStartTime_ShouldKeepExistingStartTime()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var existingStartTime = DateTime.UtcNow.AddHours(-2);
        var existingPomodoro = new Pomodoro
        {
            StartTime = existingStartTime,
            DurationInMinutes = 25,
            Task = new DomainTask { Description = "Test", Project = new Project { Name = "Test" } }
        };

        var command = new UpdatePomodoroCommand
        {
            Id = pomodoroId,
            StartTime = null,
            DurationInMinutes = 30
        };

        var pomodoros = new List<Pomodoro> { existingPomodoro };
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _pomodorosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(existingPomodoro);
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingPomodoro.StartTime.Should().Be(existingStartTime);
        existingPomodoro.DurationInMinutes.Should().Be(30);
    }

    [Fact]
    public async SystemTask Handle_WithNullDuration_ShouldKeepExistingDuration()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var existingPomodoro = new Pomodoro
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            DurationInMinutes = 25,
            Task = new DomainTask { Description = "Test", Project = new Project { Name = "Test" } }
        };

        var newStartTime = DateTime.UtcNow;
        var command = new UpdatePomodoroCommand
        {
            Id = pomodoroId,
            StartTime = newStartTime,
            DurationInMinutes = null
        };

        var pomodoros = new List<Pomodoro> { existingPomodoro };
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _pomodorosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(existingPomodoro);
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingPomodoro.StartTime.Should().Be(newStartTime);
        existingPomodoro.DurationInMinutes.Should().Be(25);
    }

    [Fact]
    public async SystemTask Handle_WithBothFieldsNull_ShouldKeepAllExistingValues()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var existingStartTime = DateTime.UtcNow.AddHours(-1);
        var existingPomodoro = new Pomodoro
        {
            StartTime = existingStartTime,
            DurationInMinutes = 25,
            Task = new DomainTask { Description = "Test", Project = new Project { Name = "Test" } }
        };

        var command = new UpdatePomodoroCommand
        {
            Id = pomodoroId,
            StartTime = null,
            DurationInMinutes = null
        };

        var pomodoros = new List<Pomodoro> { existingPomodoro };
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _pomodorosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(existingPomodoro);
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingPomodoro.StartTime.Should().Be(existingStartTime);
        existingPomodoro.DurationInMinutes.Should().Be(25);
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var existingPomodoro = new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25,
            Task = new DomainTask { Description = "Test", Project = new Project { Name = "Test" } }
        };
        var command = new UpdatePomodoroCommand
        {
            Id = pomodoroId,
            StartTime = DateTime.UtcNow.AddMinutes(5),
            DurationInMinutes = 30
        };

        var pomodoros = new List<Pomodoro> { existingPomodoro };
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _pomodorosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(existingPomodoro);
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
