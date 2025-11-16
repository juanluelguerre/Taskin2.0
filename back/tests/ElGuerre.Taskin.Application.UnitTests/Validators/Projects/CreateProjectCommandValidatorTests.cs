using ElGuerre.Taskin.Application.Projects.Commands;
using FluentValidation.TestHelper;

namespace ElGuerre.Taskin.Application.UnitTests.Validators.Projects;

/// <summary>
/// Tests for CreateProjectCommandValidator validation rules
/// </summary>
public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator validator = new();

    [Fact]
    public void Validate_WithValidName_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProjectCommand { Name = "Valid Project Name" };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateProjectCommand { Name = string.Empty };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Project name required.");
    }

    [Fact]
    public void Validate_WithNullName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateProjectCommand { Name = null! };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Project name required.");
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('A', 101);
        var command = new CreateProjectCommand { Name = longName };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("The name cannot exceed 100 characters.");
    }

    [Fact]
    public void Validate_WithNameExactly100Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactName = new string('A', 100);
        var command = new CreateProjectCommand { Name = exactName };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Valid Name")]
    [InlineData("Project 2024")]
    public void Validate_WithVariousValidNames_ShouldNotHaveValidationError(string name)
    {
        // Arrange
        var command = new CreateProjectCommand { Name = name };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
