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
/// Tests for DeletePomodoroCommandHandler
/// </summary>
public class DeletePomodoroCommandHandlerTests
{
    private readonly ITaskinDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeletePomodoroCommandHandler _handler;
    private DbSet<Pomodoro> _pomodorosDbSet;

    public DeletePomodoroCommandHandlerTests()
    {
        _context = Substitute.For<ITaskinDbContext>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        var pomodoros = new List<Pomodoro>();
        _pomodorosDbSet = pomodoros.BuildMockDbSet();

        _context.Pomodoros.Returns(_pomodorosDbSet);
        _handler = new DeletePomodoroCommandHandler(_context, _unitOfWork);
    }

    [Fact]
    public async SystemTask Handle_WithExistingPomodoro_ShouldDeletePomodoro()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var existingPomodoro = new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25,
            Task = new DomainTask { Description = "Test", Project = new Project { Name = "Test" } }
        };

        var command = new DeletePomodoroCommand { Id = pomodoroId };

        var pomodoros = new List<Pomodoro> { existingPomodoro };
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _pomodorosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(existingPomodoro);
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _pomodorosDbSet.Received(1).Remove(existingPomodoro);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingPomodoro_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var command = new DeletePomodoroCommand { Id = pomodoroId };

        var pomodoros = new List<Pomodoro>();
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        Func<SystemTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<Pomodoro>>();
        _pomodorosDbSet.DidNotReceive().Remove(Arg.Any<Pomodoro>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldCallFindAsyncWithCorrectId()
    {
        // Arrange
        var pomodoroId = Guid.NewGuid();
        var existingPomodoro = new Pomodoro
        {
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25,
            Task = new DomainTask { Description = "Test", Project = new Project { Name = "Test" } }
        };
        var command = new DeletePomodoroCommand { Id = pomodoroId };

        var pomodoros = new List<Pomodoro> { existingPomodoro };
        _pomodorosDbSet = pomodoros.BuildMockDbSet();
        _pomodorosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(existingPomodoro);
        _context.Pomodoros.Returns(_pomodorosDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _pomodorosDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(args => args.Length == 1 && (Guid)args[0] == pomodoroId),
            Arg.Any<CancellationToken>());
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
        var command = new DeletePomodoroCommand { Id = pomodoroId };

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
