using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using ElGuerre.Taskin.Domain.UnitTests.Builders;

namespace ElGuerre.Taskin.Domain.UnitTests.Entities;

/// <summary>
/// Tests for the Project domain entity
/// </summary>
public class ProjectTests
{
    [Fact]
    public void NewProject_ShouldHaveEmptyTasksCollection()
    {
        // Arrange & Act
        var project = new Project { Name = "Test" };

        // Assert
        project.Tasks.Should().NotBeNull();
        project.Tasks.Should().BeEmpty();
    }

    [Fact]
    public void NewProject_ShouldHaveDefaultStatus()
    {
        // Arrange & Act
        var project = new Project { Name = "Test" };

        // Assert
        project.Status.Should().Be(default(ProjectStatus));
    }

    [Fact]
    public void Project_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var name = "Test Project";
        var description = "Test Description";
        var status = ProjectStatus.Active;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var imageUrl = "https://example.com/image.jpg";
        var backgroundColor = "#FF0000";

        // Act
        var project = new Project
        {
            Name = name,
            Description = description,
            Status = status,
            DueDate = dueDate,
            ImageUrl = imageUrl,
            BackgroundColor = backgroundColor
        };

        // Assert
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
        project.Status.Should().Be(status);
        project.DueDate.Should().Be(dueDate);
        project.ImageUrl.Should().Be(imageUrl);
        project.BackgroundColor.Should().Be(backgroundColor);
    }

    [Fact]
    public void Project_ShouldInheritFromTrackedEntity()
    {
        // Arrange & Act
        var project = new Project { Name = "Test" };

        // Assert
        project.Should().BeAssignableTo<TrackedEntity>("Project should inherit from TrackedEntity for audit tracking");
    }

    [Theory]
    [InlineData(ProjectStatus.Active)]
    [InlineData(ProjectStatus.Completed)]
    [InlineData(ProjectStatus.OnHold)]
    public void Project_ShouldSupportAllStatusValues(ProjectStatus status)
    {
        // Arrange & Act
        var project = new Project { Name = "Test", Status = status };

        // Assert
        project.Status.Should().Be(status);
    }

    [Fact]
    public void ProjectFromTestDataBuilder_ShouldHaveValidRandomData()
    {
        // Arrange & Act
        var project = TestDataBuilder.CreateProject();

        // Assert
        project.Should().NotBeNull();
        project.Name.Should().NotBeNullOrWhiteSpace();
        project.Description.Should().NotBeNullOrWhiteSpace();
        project.BackgroundColor.Should().MatchRegex("^#[0-9A-Fa-f]{6}$");
    }

    [Fact]
    public void TestDataBuilder_ShouldAllowCustomization()
    {
        // Arrange
        var customName = "Custom Project Name";

        // Act
        var project = TestDataBuilder.CreateProject(p => p.Name = customName);

        // Assert
        project.Name.Should().Be(customName);
        project.Description.Should().NotBeNullOrWhiteSpace("other properties should still be random");
    }

    [Fact]
    public void TestDataBuilder_ShouldCreateMultipleProjects()
    {
        // Arrange
        var count = 5;

        // Act
        var projects = TestDataBuilder.CreateProjects(count);

        // Assert
        projects.Should().HaveCount(count);
        projects.Should().OnlyHaveUniqueItems(p => p.Name);
    }
}
