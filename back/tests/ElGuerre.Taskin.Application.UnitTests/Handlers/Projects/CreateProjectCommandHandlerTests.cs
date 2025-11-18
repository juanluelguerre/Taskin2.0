using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Projects.Commands;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;

namespace ElGuerre.Taskin.Application.UnitTests.Handlers.Projects;

/// <summary>
/// Tests for CreateProjectCommandHandler
/// </summary>
public class CreateProjectCommandHandlerTests
{
    private readonly IUnitOfWork unitOfWork;
    private readonly CreateProjectCommandHandler handler;
    private readonly DbSet<Project> projectsDbSet;

    public CreateProjectCommandHandlerTests()
    {
        var context = Substitute.For<ITaskinDbContext>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        var projects = new List<Project>();
        this.projectsDbSet = projects.BuildMockDbSet();

        context.Projects.Returns(this.projectsDbSet);
        this.handler = new CreateProjectCommandHandler(
            context, this.unitOfWork, new Observability.TaskinMetrics());
    }

    [Fact]
    public async SystemTask Handle_WithValidCommand_ShouldCreateProject()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            Description = "Test Description",
            Status = ProjectStatus.Active,
            DueDate = DateTime.UtcNow.AddDays(30),
            ImageUrl = "https://example.com/image.jpg",
            BackgroundColor = "#FF0000"
        };

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        this.projectsDbSet.Received(1).Add(
            Arg.Is<Project>(p =>
                p.Name == command.Name &&
                p.Description == command.Description &&
                p.Status == command.Status &&
                p.DueDate == command.DueDate &&
                p.ImageUrl == command.ImageUrl &&
                p.BackgroundColor == command.BackgroundColor));
        await this.unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_WithMinimalCommand_ShouldCreateProjectWithRequiredFieldsOnly()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Minimal Project"
        };

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        this.projectsDbSet.Received(1).Add(
            Arg.Is<Project>(p =>
                p.Name == command.Name &&
                p.Description == null &&
                p.ImageUrl == null &&
                p.BackgroundColor == null));
        await this.unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async SystemTask Handle_ShouldReturnProjectId()
    {
        // Arrange
        var command = new CreateProjectCommand { Name = "Test Project" };

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async SystemTask Handle_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var command = new CreateProjectCommand { Name = "Test Project" };

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await this.unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_ShouldAddProjectToDbSet()
    {
        // Arrange
        var command = new CreateProjectCommand { Name = "Test Project" };

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.projectsDbSet.Received(1).Add(Arg.Any<Project>());
    }
}
