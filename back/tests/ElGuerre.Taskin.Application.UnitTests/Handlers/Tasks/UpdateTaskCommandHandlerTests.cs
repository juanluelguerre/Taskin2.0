using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.Commands;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Tasks;

/// <summary>
/// Tests for UpdateTaskCommandHandler
/// </summary>
public class UpdateTaskCommandHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly IUnitOfWork unitOfWork;
    private readonly UpdateTaskCommandHandler handler;
    private DbSet<DomainTask> tasksDbSet;

    public UpdateTaskCommandHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        var tasks = new List<DomainTask>();
        this.tasksDbSet = tasks.BuildMockDbSet();

        this.context.Tasks.Returns(this.tasksDbSet);
        this.handler = new UpdateTaskCommandHandler(this.context, this.unitOfWork, new ElGuerre.Taskin.Application.Observability.TaskinMetrics());
    }

    [Fact]
    public async SystemTask Handle_WithExistingTask_ShouldUpdateTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new DomainTask
        {
            Description = "Old Description",
            Status = DomainTaskStatus.Todo,
            Project = new Project { Name = "Test Project" }
        };

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Description = "New Description",
            Status = DomainTaskStatus.Doing,
            Deadline = DateTime.UtcNow.AddDays(5)
        };

        var tasks = new List<DomainTask> { existingTask };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingTask);
        this.context.Tasks.Returns(this.tasksDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        existingTask.Description.Should().Be("New Description");
        existingTask.Status.Should().Be(DomainTaskStatus.Doing);
        existingTask.Deadline.Should().Be(command.Deadline);
        await this.unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingTask_ShouldThrowException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Description = "Test Task",
            Status = DomainTaskStatus.Todo
        };

        var tasks = new List<DomainTask>();
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.context.Tasks.Returns(this.tasksDbSet);

        // Act
        Func<SystemTask> act = async () =>
            await this.handler.Handle(command, CancellationToken.None);

        // Assert
        // Note: Current implementation throws generic Exception, not EntityNotFoundException<Task>
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Task not found");
        await this.unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_WithDefaultStatus_ShouldKeepExistingStatus()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new DomainTask
        {
            Description = "Test",
            Status = DomainTaskStatus.Doing,
            Project = new Project { Name = "Test" }
        };

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Description = "Updated Description",
            Status = default // TaskStatus.Todo is default (0)
        };

        var tasks = new List<DomainTask> { existingTask };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingTask);
        this.context.Tasks.Returns(this.tasksDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        existingTask.Status.Should().Be(DomainTaskStatus.Doing);
    }

    [Fact]
    public async SystemTask Handle_WithNullDeadline_ShouldKeepExistingDeadline()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingDeadline = DateTime.UtcNow.AddDays(10);
        var existingTask = new DomainTask
        {
            Description = "Test",
            Status = DomainTaskStatus.Todo,
            Deadline = existingDeadline,
            Project = new Project { Name = "Test" }
        };

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Description = "Updated Description",
            Status = DomainTaskStatus.Doing,
            Deadline = null
        };

        var tasks = new List<DomainTask> { existingTask };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingTask);
        this.context.Tasks.Returns(this.tasksDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        existingTask.Deadline.Should().Be(existingDeadline);
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new DomainTask { Description = "Test", Status = DomainTaskStatus.Todo, Project = new Project { Name = "Test" } };
        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Description = "Updated",
            Status = DomainTaskStatus.Todo
        };

        var tasks = new List<DomainTask> { existingTask };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingTask);
        this.context.Tasks.Returns(this.tasksDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await this.unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
