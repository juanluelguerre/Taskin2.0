using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using ElGuerre.Taskin.Domain.UnitTests.Builders;

namespace ElGuerre.Taskin.Domain.UnitTests.Entities;

/// <summary>
/// Tests for the Pomodoro domain entity
/// </summary>
public class PomodoroTests
{
    [Fact]
    public void NewPomodoro_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var pomodoro = new Pomodoro { Task = null! };

        // Assert
        pomodoro.TaskId.Should().BeEmpty();
        pomodoro.StartTime.Should().Be(default(DateTime));
        pomodoro.DurationInMinutes.Should().Be(0);
    }

    [Fact]
    public void Pomodoro_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var duration = 25; // Standard pomodoro

        // Act
        var pomodoro = new Pomodoro
        {
            Task = null!,
            TaskId = taskId,
            StartTime = startTime,
            DurationInMinutes = duration
        };

        // Assert
        pomodoro.TaskId.Should().Be(taskId);
        pomodoro.StartTime.Should().Be(startTime);
        pomodoro.DurationInMinutes.Should().Be(duration);
    }

    [Fact]
    public void Pomodoro_ShouldInheritFromTrackedEntity()
    {
        // Arrange & Act
        var pomodoro = new Pomodoro { Task = null! };

        // Assert
        pomodoro.Should().BeAssignableTo<TrackedEntity>("Pomodoro should inherit from TrackedEntity for audit tracking");
    }

    [Fact]
    public void Pomodoro_ShouldHaveRequiredTaskRelationship()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act
        var pomodoro = new Pomodoro
        {
            Task = null!,
            TaskId = taskId,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        // Assert
        pomodoro.TaskId.Should().Be(taskId);
        pomodoro.Task.Should().BeNull("navigation property is null until loaded by EF");
    }

    [Theory]
    [InlineData(25)]  // Standard pomodoro
    [InlineData(15)]  // Short break
    [InlineData(30)]  // Long break
    [InlineData(50)]  // Extended session
    [InlineData(480)] // Maximum duration
    public void Pomodoro_ShouldAcceptVariousDurationValues(int duration)
    {
        // Arrange & Act
        var pomodoro = new Pomodoro { Task = null!, DurationInMinutes = duration };

        // Assert
        pomodoro.DurationInMinutes.Should().Be(duration);
    }

    [Fact]
    public void PomodoroFromTestDataBuilder_ShouldHaveValidRandomData()
    {
        // Arrange & Act
        var pomodoro = TestDataBuilder.CreatePomodoro();

        // Assert
        pomodoro.Should().NotBeNull();
        pomodoro.TaskId.Should().NotBeEmpty();
        pomodoro.StartTime.Should().NotBe(default(DateTime));
        pomodoro.DurationInMinutes.Should().BeGreaterThan(0);
        pomodoro.DurationInMinutes.Should().BeLessOrEqualTo(480);
    }

    [Fact]
    public void TestDataBuilder_ShouldAllowPomodoroCustomization()
    {
        // Arrange
        var customTaskId = Guid.NewGuid();
        var customDuration = 30;

        // Act
        var pomodoro = TestDataBuilder.CreatePomodoro(p =>
        {
            p.TaskId = customTaskId;
            p.DurationInMinutes = customDuration;
        });

        // Assert
        pomodoro.TaskId.Should().Be(customTaskId);
        pomodoro.DurationInMinutes.Should().Be(customDuration);
        pomodoro.StartTime.Should().NotBe(default(DateTime), "other properties should still be random");
    }

    [Fact]
    public void TestDataBuilder_ShouldCreateMultiplePomodoros()
    {
        // Arrange
        var count = 15;

        // Act
        var pomodoros = TestDataBuilder.CreatePomodoros(count);

        // Assert
        pomodoros.Should().HaveCount(count);
        pomodoros.All(p => p.DurationInMinutes > 0).Should().BeTrue();
        pomodoros.All(p => p.DurationInMinutes <= 480).Should().BeTrue();
    }

    [Fact]
    public void Pomodoro_WithStartTime_ShouldStoreDateTimeCorrectly()
    {
        // Arrange
        var startTime = new DateTime(2025, 11, 16, 14, 30, 0);

        // Act
        var pomodoro = new Pomodoro
        {
            Task = null!,
            StartTime = startTime,
            TaskId = Guid.NewGuid(),
            DurationInMinutes = 25
        };

        // Assert
        pomodoro.StartTime.Should().Be(startTime);
    }

    [Fact]
    public void Pomodoro_StandardSession_ShouldBe25Minutes()
    {
        // Arrange
        var standardDuration = 25;

        // Act
        var pomodoro = new Pomodoro { Task = null!, DurationInMinutes = standardDuration };

        // Assert
        pomodoro.DurationInMinutes.Should().Be(25, "standard pomodoro technique uses 25-minute sessions");
    }
}
