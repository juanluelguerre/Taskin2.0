using ElGuerre.Taskin.Application.Tasks.Commands;
using FluentValidation.TestHelper;

namespace ElGuerre.Taskin.Application.UnitTests.Validators.Tasks;

/// <summary>
/// Tests for UpdateTaskCommandValidator validation rules
/// </summary>
public class UpdateTaskCommandValidatorTests
{
    private readonly UpdateTaskCommandValidator _validator;

    public UpdateTaskCommandValidatorTests()
    {
        _validator = new UpdateTaskCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = "Valid task description",
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Doing
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
        var command = new UpdateTaskCommand
        {
            Id = Guid.Empty,
            Description = "Valid description",
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
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
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = "Valid description",
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Validate_WithEmptyDescription_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = string.Empty,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = null!,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Validate_WithDescriptionExceeding500Characters_ShouldHaveValidationError()
    {
        // Arrange
        var longDescription = new string('A', 501);
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = longDescription,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 500 characters");
    }

    [Fact]
    public void Validate_WithDescriptionExactly500Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactDescription = new string('A', 500);
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = exactDescription,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Status Validation

    [Theory]
    [InlineData(ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo)]
    [InlineData(ElGuerre.Taskin.Domain.Entities.TaskStatus.Doing)]
    [InlineData(ElGuerre.Taskin.Domain.Entities.TaskStatus.Done)]
    public void Validate_WithValidStatus_ShouldNotHaveValidationError(ElGuerre.Taskin.Domain.Entities.TaskStatus status)
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = "Valid description",
            Status = status
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_WithInvalidStatus_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = "Valid description",
            Status = (ElGuerre.Taskin.Domain.Entities.TaskStatus)999 // Invalid enum value
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status must be a valid TaskStatus");
    }

    #endregion

    #region Deadline Validation

    [Fact]
    public void Validate_WithNullDeadline_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = "Valid description",
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo,
            Deadline = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Deadline);
    }

    [Fact]
    public void Validate_WithFutureDeadline_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = "Valid description",
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo,
            Deadline = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Deadline);
    }

    [Fact]
    public void Validate_WithPastDeadline_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Description = "Valid description",
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo,
            Deadline = new DateTime(2020, 1, 1) // Clearly in the past
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        // FluentValidation reports errors on Deadline.Value for nullable DateTime properties
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Deadline.Value"
            && e.ErrorMessage == "Deadline must be in the future");
    }

    #endregion
}
