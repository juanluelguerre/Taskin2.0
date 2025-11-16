using ElGuerre.Taskin.Application.Tasks.Commands;
using ElGuerre.Taskin.Domain.Entities;
using FluentValidation.TestHelper;

namespace ElGuerre.Taskin.Application.UnitTests.Validators.Tasks;

/// <summary>
/// Tests for CreateTaskCommandValidator validation rules
/// </summary>
public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator;

    public CreateTaskCommandValidatorTests()
    {
        _validator = new CreateTaskCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Description = "Valid task description",
            ProjectId = Guid.NewGuid(),
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #region Description Validation

    [Fact]
    public void Validate_WithEmptyDescription_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Description = string.Empty,
            ProjectId = Guid.NewGuid(),
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
        var command = new CreateTaskCommand
        {
            Description = null!,
            ProjectId = Guid.NewGuid(),
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
        var command = new CreateTaskCommand
        {
            Description = longDescription,
            ProjectId = Guid.NewGuid(),
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
        var command = new CreateTaskCommand
        {
            Description = exactDescription,
            ProjectId = Guid.NewGuid(),
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region ProjectId Validation

    [Fact]
    public void Validate_WithEmptyProjectId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Description = "Valid description",
            ProjectId = Guid.Empty,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("ProjectId is required");
    }

    [Fact]
    public void Validate_WithValidProjectId_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Description = "Valid description",
            ProjectId = Guid.NewGuid(),
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
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
        var command = new CreateTaskCommand
        {
            Description = "Valid description",
            ProjectId = Guid.NewGuid(),
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
        var command = new CreateTaskCommand
        {
            Description = "Valid description",
            ProjectId = Guid.NewGuid(),
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
        var command = new CreateTaskCommand
        {
            Description = "Valid description",
            ProjectId = Guid.NewGuid(),
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
        var command = new CreateTaskCommand
        {
            Description = "Valid description",
            ProjectId = Guid.NewGuid(),
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
        var command = new CreateTaskCommand
        {
            Description = "Valid description",
            ProjectId = Guid.NewGuid(),
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
