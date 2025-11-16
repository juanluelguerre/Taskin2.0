using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using ElGuerre.Taskin.Domain.UnitTests.Builders;

namespace ElGuerre.Taskin.Domain.UnitTests.Entities;

/// <summary>
/// Tests for the Task domain entity
/// </summary>
public class TaskTests
{
    [Fact]
    public void NewTask_ShouldHaveEmptyPomodorosCollection()
    {
        // Arrange & Act
        var task = new Domain.Entities.Task { Description = "Test", Project = null! };

        // Assert
        task.Pomodoros.Should().NotBeNull();
        task.Pomodoros.Should().BeEmpty();
    }

    [Fact]
    public void NewTask_ShouldHaveDefaultStatus()
    {
        // Arrange & Act
        var task = new Domain.Entities.Task { Description = "Test", Project = null! };

        // Assert
        task.Status.Should().Be(default(ElGuerre.Taskin.Domain.Entities.TaskStatus));
    }

    [Fact]
    public void Task_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var description = "Test Task Description";
        var projectId = Guid.NewGuid();
        var status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Doing;
        var deadline = DateTime.UtcNow.AddDays(7);

        // Act
        var task = new Domain.Entities.Task
        {
            Description = description,
            Project = null!,
            ProjectId = projectId,
            Status = status,
            Deadline = deadline
        };

        // Assert
        task.Description.Should().Be(description);
        task.ProjectId.Should().Be(projectId);
        task.Status.Should().Be(status);
        task.Deadline.Should().Be(deadline);
    }

    [Fact]
    public void Task_ShouldInheritFromTrackedEntity()
    {
        // Arrange & Act
        var task = new Domain.Entities.Task { Description = "Test", Project = null! };

        // Assert
        task.Should().BeAssignableTo<TrackedEntity>("Task should inherit from TrackedEntity for audit tracking");
    }

    [Theory]
    [InlineData(ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo)]
    [InlineData(ElGuerre.Taskin.Domain.Entities.TaskStatus.Doing)]
    [InlineData(ElGuerre.Taskin.Domain.Entities.TaskStatus.Done)]
    public void Task_ShouldSupportAllStatusValues(ElGuerre.Taskin.Domain.Entities.TaskStatus status)
    {
        // Arrange & Act
        var task = new Domain.Entities.Task { Description = "Test", Project = null!, Status = status };

        // Assert
        task.Status.Should().Be(status);
    }

    [Fact]
    public void Task_ShouldHaveRequiredProjectRelationship()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var task = new Domain.Entities.Task
        {
            Description = "Test",
            Project = null!,
            ProjectId = projectId
        };

        // Assert
        task.ProjectId.Should().Be(projectId);
        task.Project.Should().BeNull("navigation property is null until loaded by EF");
    }

    [Fact]
    public void TaskFromTestDataBuilder_ShouldHaveValidRandomData()
    {
        // Arrange & Act
        var task = TestDataBuilder.CreateTask();

        // Assert
        task.Should().NotBeNull();
        task.Description.Should().NotBeNullOrWhiteSpace();
        task.ProjectId.Should().NotBeEmpty();
    }

    [Fact]
    public void TestDataBuilder_ShouldAllowTaskCustomization()
    {
        // Arrange
        var customDescription = "Custom Task Description";
        var customProjectId = Guid.NewGuid();

        // Act
        var task = TestDataBuilder.CreateTask(t =>
        {
            t.Description = customDescription;
            t.ProjectId = customProjectId;
        });

        // Assert
        task.Description.Should().Be(customDescription);
        task.ProjectId.Should().Be(customProjectId);
    }

    [Fact]
    public void TestDataBuilder_ShouldCreateMultipleTasks()
    {
        // Arrange
        var count = 10;

        // Act
        var tasks = TestDataBuilder.CreateTasks(count);

        // Assert
        tasks.Should().HaveCount(count);
        tasks.Should().OnlyHaveUniqueItems(t => t.Description);
    }

    [Fact]
    public void Task_WithDeadline_ShouldStoreDateCorrectly()
    {
        // Arrange
        var deadline = new DateTime(2025, 12, 31, 23, 59, 59);

        // Act
        var task = new Domain.Entities.Task
        {
            Description = "Test",
            Project = null!,
            Deadline = deadline
        };

        // Assert
        task.Deadline.Should().Be(deadline);
    }
}
