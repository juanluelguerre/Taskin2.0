using ElGuerre.Taskin.Application.Pomodoros.Commands;
using FluentValidation.TestHelper;

namespace ElGuerre.Taskin.Application.UnitTests.Validators.Pomodoros;

/// <summary>
/// Tests for UpdatePomodoroCommandValidator validation rules
/// </summary>
public class UpdatePomodoroCommandValidatorTests
{
    private readonly UpdatePomodoroCommandValidator _validator;

    public UpdatePomodoroCommandValidatorTests()
    {
        _validator = new UpdatePomodoroCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithOnlyIdProvided_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = null,
            DurationInMinutes = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #region Id Validation

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.Empty,
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }

    [Fact]
    public void Validate_WithValidId_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    #endregion

    #region StartTime Validation (Conditional)

    [Fact]
    public void Validate_WithNullStartTime_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = null,
            DurationInMinutes = 25
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
    }

    [Fact]
    public void Validate_WithValidProvidedStartTime_ShouldNotHaveValidationError_WhenNotDefault()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 25
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
    }

    #endregion

    #region DurationInMinutes Validation (Conditional)

    [Fact]
    public void Validate_WithNullDuration_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DurationInMinutes);
    }

    [Fact]
    public void Validate_WithProvidedZeroDuration_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 0
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        // FluentValidation reports errors on DurationInMinutes.Value for nullable properties
        result.Errors.Should().ContainSingle(e => e.PropertyName == "DurationInMinutes.Value"
            && e.ErrorMessage == "Duration must be greater than 0 minutes");
    }

    [Fact]
    public void Validate_WithProvidedNegativeDuration_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = -10
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.Errors.Should().ContainSingle(e => e.PropertyName == "DurationInMinutes.Value"
            && e.ErrorMessage == "Duration must be greater than 0 minutes");
    }

    [Fact]
    public void Validate_WithProvidedDurationExceeding480Minutes_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = 481
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.Errors.Should().ContainSingle(e => e.PropertyName == "DurationInMinutes.Value"
            && e.ErrorMessage == "Pomodoro session cannot exceed 8 hours (480 minutes)");
    }

    [Theory]
    [InlineData(1)] // Minimum valid
    [InlineData(25)] // Standard pomodoro
    [InlineData(50)] // Extended session
    [InlineData(240)] // 4 hours
    [InlineData(480)] // Maximum valid (8 hours)
    public void Validate_WithProvidedValidDuration_ShouldNotHaveValidationError(int duration)
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = duration
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DurationInMinutes);
    }

    #endregion

    #region Partial Update Tests

    [Fact]
    public void Validate_WithOnlyStartTimeUpdated_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DurationInMinutes = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithOnlyDurationUpdated_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdatePomodoroCommand
        {
            Id = Guid.NewGuid(),
            StartTime = null,
            DurationInMinutes = 30
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
