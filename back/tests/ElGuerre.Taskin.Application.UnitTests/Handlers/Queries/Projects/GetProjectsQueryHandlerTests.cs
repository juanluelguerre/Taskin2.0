using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Projects.Queries;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Queries.Projects;

/// <summary>
/// Tests for GetProjectsQueryHandler
/// </summary>
public class GetProjectsQueryHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly GetProjectsQueryHandler handler;
    private DbSet<Project> projectsDbSet;

    public GetProjectsQueryHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();

        this.context.Projects.Returns(this.projectsDbSet);
        this.handler = new GetProjectsQueryHandler(this.context);
    }

    [Fact]
    public async SystemTask Handle_WithNoFilters_ShouldReturnAllProjects()
    {
        // Arrange
        var project1 = new Project { Name = "Project 1", Status = ProjectStatus.Active };
        var project2 = new Project { Name = "Project 2", Status = ProjectStatus.Completed };

        var projects = new List<Project> { project1, project2 };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 1, Size = 10 };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.Page.Should().Be(1);
        result.Size.Should().Be(10);
    }

    [Fact]
    public async SystemTask Handle_WithSearchFilter_ShouldReturnMatchingProjects()
    {
        // Arrange
        var project1 = new Project { Name = "Angular Project", Description = "Frontend project" };
        var project2 = new Project { Name = "Backend API", Description = "API project" };

        var projects = new List<Project> { project1, project2 };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 1, Size = 10, Search = "angular" };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("Angular Project");
    }

    [Fact]
    public async SystemTask Handle_WithStatusFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var project1 = new Project { Name = "Active Project", Status = ProjectStatus.Active };
        var project2 = new Project { Name = "Completed Project", Status = ProjectStatus.Completed };

        var projects = new List<Project> { project1, project2 };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 1, Size = 10, Status = "Active" };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("Active Project");
    }

    [Fact]
    public async SystemTask Handle_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var project1 = new Project { Name = "Project 1" };
        var project2 = new Project { Name = "Project 2" };
        var project3 = new Project { Name = "Project 3" };

        var projects = new List<Project> { project3, project2, project1 };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 2, Size = 2 };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Total.Should().Be(3);
        result.Page.Should().Be(2);
        result.Size.Should().Be(2);
    }

    [Fact]
    public async SystemTask Handle_WithSortingByName_ShouldReturnSortedProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Name = "Zebra Project" },
            new Project { Name = "Alpha Project" },
            new Project { Name = "Beta Project" }
        };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 1, Size = 10, Sort = "name", Order = "asc" };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.First().Name.Should().Be("Alpha Project");
        result.Data.Last().Name.Should().Be("Zebra Project");
    }

    [Fact]
    public async SystemTask Handle_ShouldCalculateProgressCorrectly()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        project.Tasks.Add(new DomainTask { Description = "Task 1", Status = DomainTaskStatus.Done, Project = project });
        project.Tasks.Add(new DomainTask { Description = "Task 2", Status = DomainTaskStatus.Done, Project = project });
        project.Tasks.Add(new DomainTask { Description = "Task 3", Status = DomainTaskStatus.Todo, Project = project });
        project.Tasks.Add(new DomainTask { Description = "Task 4", Status = DomainTaskStatus.Doing, Project = project });

        var projects = new List<Project> { project };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 1, Size = 10 };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Progress.Should().Be(50); // 2 out of 4 = 50%
        result.Data.First().TotalTasks.Should().Be(4);
        result.Data.First().CompletedTasks.Should().Be(2);
    }

    [Fact]
    public async SystemTask Handle_ShouldReturnCorrectTotal()
    {
        // Arrange
        var projects = Enumerable.Range(1, 25).Select(i => new Project { Name = $"Project {i}" }).ToList();
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 1, Size = 10 };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.Total.Should().Be(25);
    }

    [Fact]
    public async SystemTask Handle_WithEmptyResult_ShouldReturnEmptyCollection()
    {
        // Arrange
        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectsQuery { Page = 1, Size = 10 };

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Total.Should().Be(0);
    }
}
