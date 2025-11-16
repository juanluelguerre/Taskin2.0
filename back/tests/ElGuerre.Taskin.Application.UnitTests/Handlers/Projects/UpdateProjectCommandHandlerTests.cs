using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Projects.Commands;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Projects;

/// <summary>
/// Tests for UpdateProjectCommandHandler
/// </summary>
public class UpdateProjectCommandHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly IUnitOfWork unitOfWork;
    private readonly UpdateProjectCommandHandler handler;
    private DbSet<Project> projectsDbSet;

    public UpdateProjectCommandHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();

        this.context.Projects.Returns(this.projectsDbSet);
        this.handler = new UpdateProjectCommandHandler(this.context, this.unitOfWork);
    }

    [Fact]
    public async SystemTask Handle_WithExistingProject_ShouldUpdateProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project
        {
            Name = "Old Name",
            Description = "Old Description",
            Status = ProjectStatus.Active
        };

        var command = new UpdateProjectCommand
        {
            Id = projectId,
            Name = "New Name",
            Description = "New Description",
            Status = ProjectStatus.OnHold
        };

        var projects = new List<Project> { existingProject };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingProject);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        existingProject.Name.Should().Be("New Name");
        existingProject.Description.Should().Be("New Description");
        existingProject.Status.Should().Be(ProjectStatus.OnHold);
        await this.unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingProject_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new UpdateProjectCommand
        {
            Id = projectId,
            Name = "Test Project"
        };

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        Func<SystemTask> act = async () =>
            await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<Project>>();
        await this.unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldUpdateOptionalFields()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project { Name = "Test" };

        var command = new UpdateProjectCommand
        {
            Id = projectId,
            Name = "Updated Name",
            ImageUrl = "https://example.com/new-image.jpg",
            BackgroundColor = "#00FF00",
            DueDate = DateTime.UtcNow.AddDays(60)
        };

        var projects = new List<Project> { existingProject };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingProject);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        existingProject.ImageUrl.Should().Be(command.ImageUrl);
        existingProject.BackgroundColor.Should().Be(command.BackgroundColor);
        existingProject.DueDate.Should().Be(command.DueDate);
    }

    [Fact]
    public async SystemTask Handle_WithNullStatus_ShouldNotUpdateStatus()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project
        {
            Name = "Test",
            Status = ProjectStatus.Active
        };

        var command = new UpdateProjectCommand
        {
            Id = projectId,
            Name = "Updated Name",
            Status = null
        };

        var projects = new List<Project> { existingProject };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.projectsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingProject);
        this.context.Projects.Returns(this.projectsDbSet);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        existingProject.Status.Should().Be(ProjectStatus.Active);
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project { Name = "Test" };
        var command = new UpdateProjectCommand { Id = projectId, Name = "Updated" };

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
