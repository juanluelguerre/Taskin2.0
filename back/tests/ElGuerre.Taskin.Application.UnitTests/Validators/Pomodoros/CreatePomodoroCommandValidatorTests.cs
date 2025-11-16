using ElGuerre.Taskin.Application.Pomodoros.Commands;
using FluentValidation.TestHelper;

namespace ElGuerre.Taskin.Application.UnitTests.Validators.Pomodoros;

/// <summary>
/// Tests for CreatePomodoroCommandValidator validation rules
/// </summary>
public class CreatePomodoroCommandValidatorTests
{
    private readonly CreatePomodoroCommandValidator validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25 // Standard pomodoro
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #region TaskId Validation

    [Fact]
    public void Validate_WithEmptyTaskId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.Empty,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskId)
            .WithErrorMessage("TaskId is required");
    }

    [Fact]
    public void Validate_WithValidTaskId_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TaskId);
    }

    #endregion

    #region StartTime Validation

    [Fact]
    public void Validate_WithDefaultStartTime_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = default(DateTime),
            DurationInMinutes = 25
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartTime)
            .WithErrorMessage("StartTime is required");
    }

    [Fact]
    public void Validate_WithValidStartTime_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
    }

    #endregion

    #region DurationInMinutes Validation

    [Fact]
    public void Validate_WithZeroDuration_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 0
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes)
            .WithErrorMessage("Duration must be greater than 0 minutes");
    }

    [Fact]
    public void Validate_WithNegativeDuration_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = -10
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes)
            .WithErrorMessage("Duration must be greater than 0 minutes");
    }

    [Fact]
    public void Validate_WithDurationExceeding480Minutes_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 481 // 8 hours + 1 minute
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes)
            .WithErrorMessage("Pomodoro session cannot exceed 8 hours (480 minutes)");
    }

    [Theory]
    [InlineData(1)] // Minimum valid
    [InlineData(25)] // Standard pomodoro
    [InlineData(50)] // Extended session
    [InlineData(240)] // 4 hours
    [InlineData(480)] // Maximum valid (8 hours)
    public void Validate_WithValidDuration_ShouldNotHaveValidationError(int duration)
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = duration
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DurationInMinutes);
    }

    [Fact]
    public void Validate_WithStandardPomodoroDuration_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreatePomodoroCommand
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25 // Standard pomodoro technique
        };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
