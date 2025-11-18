using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.Commands;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Tasks;

/// <summary>
/// Tests for DeleteTaskCommandHandler
/// </summary>
public class DeleteTaskCommandHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly IUnitOfWork unitOfWork;
    private readonly DeleteTaskCommandHandler handler;
    private DbSet<DomainTask> tasksDbSet;

    public DeleteTaskCommandHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        var tasks = new List<DomainTask>();
        this.tasksDbSet = tasks.BuildMockDbSet();

        this.context.Tasks.Returns(this.tasksDbSet);
        this.handler = new DeleteTaskCommandHandler(this.context, this.unitOfWork, new ElGuerre.Taskin.Application.Observability.TaskinMetrics());
    }

    [Fact]
    public async SystemTask Handle_WithExistingTask_ShouldDeleteTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new DomainTask
        {
            Description = "Task to Delete",
            Status = DomainTaskStatus.Todo,
            Project = new Project { Name = "Test" }
        };

        var command = new DeleteTaskCommand { Id = taskId };

        var tasks = new List<DomainTask> { existingTask };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingTask);
        this.context.Tasks.Returns(this.tasksDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.tasksDbSet.Received(1).Remove(existingTask);
        await this.unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingTask_ShouldThrowException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand { Id = taskId };

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
        this.tasksDbSet.DidNotReceive().Remove(Arg.Any<DomainTask>());
        await this.unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldCallFindAsyncWithCorrectId()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new DomainTask { Description = "Test", Status = DomainTaskStatus.Todo, Project = new Project { Name = "Test" } };
        var command = new DeleteTaskCommand { Id = taskId };

        var tasks = new List<DomainTask> { existingTask };
        this.tasksDbSet = tasks.BuildMockDbSet();
        this.tasksDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingTask);
        this.context.Tasks.Returns(this.tasksDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await this.tasksDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(args => args.Length == 1 && (Guid)args[0] == taskId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new DomainTask
        {
            Description = "Test", Status = DomainTaskStatus.Todo,
            Project = new Project { Name = "Test" }
        };
        var command = new DeleteTaskCommand { Id = taskId };

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
