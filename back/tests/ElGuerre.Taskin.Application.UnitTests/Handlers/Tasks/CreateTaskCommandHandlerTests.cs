using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Tasks.Commands;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Tasks;

/// <summary>
/// Tests for CreateTaskCommandHandler
/// </summary>
public class CreateTaskCommandHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly IUnitOfWork unitOfWork;
    private readonly CreateTaskCommandHandler handler;
    private readonly DbSet<DomainTask> _tasksDbSet;
    private DbSet<Project> projectsDbSet;

    public CreateTaskCommandHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        var tasks = new List<DomainTask>();
        _tasksDbSet = tasks.BuildMockDbSet();

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();

        this.context.Tasks.Returns(_tasksDbSet);
        this.context.Projects.Returns(this.projectsDbSet);
        this.handler = new CreateTaskCommandHandler(this.context, this.unitOfWork);
    }

    [Fact]
    public async SystemTask Handle_WithValidCommand_ShouldCreateTask()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };

        var command = new CreateTaskCommand
        {
            Description = "Test Task",
            ProjectId = projectId,
            Status = DomainTaskStatus.Todo,
            Deadline = DateTime.UtcNow.AddDays(7)
        };

        var projects = new List<Project> { project };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(project);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _tasksDbSet.Received(1).Add(Arg.Is<DomainTask>(t =>
            t.Description == command.Description &&
            t.ProjectId == command.ProjectId &&
            t.Status == command.Status &&
            t.Deadline == command.Deadline &&
            t.Project == project));
        await this.unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingProject_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new CreateTaskCommand
        {
            Description = "Test Task",
            ProjectId = projectId,
            Status = DomainTaskStatus.Todo
        };

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        Func<SystemTask> act = async () =>
            await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<Project>>();
        _tasksDbSet.DidNotReceive().Add(Arg.Any<DomainTask>());
        await this.unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldReturnTaskId()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };
        var command = new CreateTaskCommand
        {
            Description = "Test Task",
            ProjectId = projectId,
            Status = DomainTaskStatus.Todo
        };

        var projects = new List<Project> { project };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(project);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async SystemTask Handle_WithMinimalCommand_ShouldCreateTaskWithRequiredFieldsOnly()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };
        var command = new CreateTaskCommand
        {
            Description = "Minimal Task",
            ProjectId = projectId,
            Status = DomainTaskStatus.Todo
        };

        var projects = new List<Project> { project };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(project);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _tasksDbSet.Received(1).Add(Arg.Is<DomainTask>(t =>
            t.Description == command.Description &&
            t.ProjectId == command.ProjectId &&
            t.Deadline == null));
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Name = "Test Project" };
        var command = new CreateTaskCommand
        {
            Description = "Test Task",
            ProjectId = projectId,
            Status = DomainTaskStatus.Todo
        };

        var projects = new List<Project> { project };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(project);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await this.unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
