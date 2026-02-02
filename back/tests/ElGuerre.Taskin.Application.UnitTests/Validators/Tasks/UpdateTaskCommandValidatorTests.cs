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
            Title = "Valid task title",
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
            Title = "Valid title",
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
            Title = "Valid title",
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    #endregion

    #region Title Validation

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = string.Empty,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Validate_WithNullTitle_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = null!,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Validate_WithTitleExceeding200Characters_ShouldHaveValidationError()
    {
        // Arrange
        var longTitle = new string('A', 201);
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = longTitle,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 200 characters");
    }

    [Fact]
    public void Validate_WithTitleExactly200Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactTitle = new string('A', 200);
        var command = new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Title = exactTitle,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Todo
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
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
            Title = "Valid title",
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
            Title = "Valid title",
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
            Title = "Valid title",
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
            Title = "Valid title",
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
            Title = "Valid title",
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
