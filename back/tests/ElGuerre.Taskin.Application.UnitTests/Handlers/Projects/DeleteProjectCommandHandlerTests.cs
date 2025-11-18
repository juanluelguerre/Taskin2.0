using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Projects.Commands;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Projects;

/// <summary>
/// Tests for DeleteProjectCommandHandler
/// </summary>
public class DeleteProjectCommandHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly IUnitOfWork unitOfWork;
    private readonly DeleteProjectCommandHandler handler;
    private DbSet<Project> projectsDbSet;

    public DeleteProjectCommandHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();

        this.context.Projects.Returns(this.projectsDbSet);
        this.handler = new DeleteProjectCommandHandler(this.context, this.unitOfWork, new ElGuerre.Taskin.Application.Observability.TaskinMetrics());
    }

    [Fact]
    public async SystemTask Handle_WithExistingProject_ShouldDeleteProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project
        {
            Name = "Project to Delete",
            Description = "Will be deleted"
        };

        var command = new DeleteProjectCommand { Id = projectId };

        var projects = new List<Project> { existingProject };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingProject);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.projectsDbSet.Received(1).Remove(existingProject);
        await this.unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingProject_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new DeleteProjectCommand { Id = projectId };

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        Func<SystemTask> act = async () =>
            await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<Project>>();
        this.projectsDbSet.DidNotReceive().Remove(Arg.Any<Project>());
        await this.unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldCallFindAsyncWithCorrectId()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project { Name = "Test" };
        var command = new DeleteProjectCommand { Id = projectId };

        var projects = new List<Project> { existingProject };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingProject);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await this.projectsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(args => args.Length == 1 && (Guid)args[0] == projectId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project { Name = "Test" };
        var command = new DeleteProjectCommand { Id = projectId };

        var projects = new List<Project> { existingProject };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingProject);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await this.unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
