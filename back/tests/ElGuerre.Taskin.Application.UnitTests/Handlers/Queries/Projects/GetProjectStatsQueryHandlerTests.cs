using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Projects.Queries;
using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Queries.Projects;

/// <summary>
/// Tests for GetProjectStatsQueryHandler
/// </summary>
public class GetProjectStatsQueryHandlerTests
{
    private readonly ITaskinDbContext context;
    private readonly GetProjectStatsQueryHandler handler;
    private DbSet<Project> projectsDbSet;

    public GetProjectStatsQueryHandlerTests()
    {
        this.context = Substitute.For<ITaskinDbContext>();

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();

        this.context.Projects.Returns(this.projectsDbSet);
        this.handler = new GetProjectStatsQueryHandler(this.context);
    }

    [Fact]
    public async SystemTask Handle_WithProjects_ShouldReturnCorrectStats()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Name = "Project 1", Status = ProjectStatus.Active },
            new Project { Name = "Project 2", Status = ProjectStatus.Active },
            new Project { Name = "Project 3", Status = ProjectStatus.Completed },
            new Project { Name = "Project 4", Status = ProjectStatus.OnHold }
        };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectStatsQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(4);
        result.Active.Should().Be(2);
        result.Completed.Should().Be(1);
        result.OnHold.Should().Be(1);
    }

    [Fact]
    public async SystemTask Handle_WithNoProjects_ShouldReturnZeroStats()
    {
        // Arrange
        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectStatsQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(0);
        result.Active.Should().Be(0);
        result.Completed.Should().Be(0);
        result.OnHold.Should().Be(0);
    }

    [Fact]
    public async SystemTask Handle_ShouldCountByStatus()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Name = "Project 1", Status = ProjectStatus.Active },
            new Project { Name = "Project 2", Status = ProjectStatus.Active },
            new Project { Name = "Project 3", Status = ProjectStatus.Active },
            new Project { Name = "Project 4", Status = ProjectStatus.Completed },
            new Project { Name = "Project 5", Status = ProjectStatus.Completed },
            new Project { Name = "Project 6", Status = ProjectStatus.OnHold }
        };
        this.projectsDbSet = projects.BuildMockDbSet();
        this.context.Projects.Returns(this.projectsDbSet);

        var query = new GetProjectStatsQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Total.Should().Be(6);
        result.Active.Should().Be(3);
        result.Completed.Should().Be(2);
        result.OnHold.Should().Be(1);
    }
}
