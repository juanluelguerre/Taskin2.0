using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Projects.Queries;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Queries.Projects;

/// <summary>
/// Tests for GetProjectByIdQueryHandler
/// </summary>
public class GetProjectByIdQueryHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly GetProjectByIdQueryHandler handler;
    private DbSet<Project> projectsDbSet;

    public GetProjectByIdQueryHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();

        this.context.Projects.Returns(this.projectsDbSet);
        this.handler = new GetProjectByIdQueryHandler(this.context);
    }

    [Fact]
    public async SystemTask Handle_WithExistingProject_ShouldReturnProjectDetails()
    {
        // Arrange
        var project = new Project
        {
            Name = "Test Project",
            Description = "Test Description",
            Status = ProjectStatus.Active
        };
        project.Tasks.Add(new DomainTask
        {
            Description = "Task 1",
            Status = DomainTaskStatus.Done,
            Project = project
        });
        project.Tasks.Add(new DomainTask
        {
            Description = "Task 2",
            Status = DomainTaskStatus.Todo,
            Project = project
        });

        var projects = new List<Project> { project };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectByIdQuery { Id = project.Id };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.Name.Should().Be("Test Project");
        result.Description.Should().Be("Test Description");
        result.Status.Should().Be("active");
        result.TotalTasks.Should().Be(2);
        result.CompletedTasks.Should().Be(1);
        result.Progress.Should().Be(50);
        result.Tasks.Should().HaveCount(2);
    }

    [Fact]
    public async SystemTask Handle_WithNonExistingProject_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectByIdQuery { Id = projectId };

        // Act
        Func<SystemTask> act = async () => await this.handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException<Project>>();
    }

    [Fact]
    public async SystemTask Handle_ShouldIncludeTasks()
    {
        // Arrange
        var project = new Project
        {
            Name = "Test Project"
        };
        project.Tasks.Add(new DomainTask
        {
            Description = "Task 1",
            Status = DomainTaskStatus.Done,
            Project = project
        });
        project.Tasks.Add(new DomainTask
        {
            Description = "Task 2",
            Status = DomainTaskStatus.Doing,
            Project = project
        });

        var projects = new List<Project> { project };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectByIdQuery { Id = project.Id };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Tasks.Should().HaveCount(2);
        result.Tasks.Should().Contain(t => t.Title == "Task 1" && t.Status == "done");
        result.Tasks.Should().Contain(t => t.Title == "Task 2" && t.Status == "doing");
    }
}
