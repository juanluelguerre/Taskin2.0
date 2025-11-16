using ElGuerre.Taskin.Application.Projects.Commands;
using ElGuerre.Taskin.Domain.Entities;
using FluentValidation.TestHelper;

namespace ElGuerre.Taskin.Application.UnitTests.Validators.Projects;

/// <summary>
/// Tests for UpdateProjectCommandValidator validation rules
/// </summary>
public class UpdateProjectCommandValidatorTests
{
    private readonly UpdateProjectCommandValidator _validator;

    public UpdateProjectCommandValidatorTests()
    {
        _validator = new UpdateProjectCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Project Name",
            Description = "Valid description",
            Status = ProjectStatus.Active
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
        var command = new UpdateProjectCommand
        {
            Id = Guid.Empty,
            Name = "Valid Name"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }

    #endregion

    #region Name Validation

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Validate_WithNullName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = null!
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Validate_WithNameExceeding200Characters_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('A', 201);
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = longName
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot exceed 200 characters");
    }

    [Fact]
    public void Validate_WithNameExactly200Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactName = new string('A', 200);
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = exactName
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region ImageUrl Validation

    [Fact]
    public void Validate_WithNullImageUrl_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            ImageUrl = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ImageUrl);
    }

    [Fact]
    public void Validate_WithEmptyImageUrl_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            ImageUrl = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ImageUrl);
    }

    [Theory]
    [InlineData("https://example.com/image.jpg")]
    [InlineData("http://example.com/image.png")]
    [InlineData("https://cdn.example.com/assets/project-image.svg")]
    public void Validate_WithValidImageUrl_ShouldNotHaveValidationError(string imageUrl)
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            ImageUrl = imageUrl
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ImageUrl);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("invalid url")]
    [InlineData("just a string")]
    public void Validate_WithInvalidImageUrl_ShouldHaveValidationError(string imageUrl)
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            ImageUrl = imageUrl
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
            .WithErrorMessage("ImageUrl must be a valid URL");
    }

    #endregion

    #region BackgroundColor Validation

    [Fact]
    public void Validate_WithNullBackgroundColor_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            BackgroundColor = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BackgroundColor);
    }

    [Fact]
    public void Validate_WithEmptyBackgroundColor_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            BackgroundColor = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BackgroundColor);
    }

    [Theory]
    [InlineData("#FF0000")] // Red
    [InlineData("#00FF00")] // Green
    [InlineData("#0000FF")] // Blue
    [InlineData("#FFFFFF")] // White
    [InlineData("#000000")] // Black
    [InlineData("#1a2b3c")] // Lowercase hex
    [InlineData("#1A2B3C")] // Uppercase hex
    public void Validate_WithValidHexColor_ShouldNotHaveValidationError(string color)
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            BackgroundColor = color
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BackgroundColor);
    }

    [Theory]
    [InlineData("FF0000")] // Missing #
    [InlineData("#FF00")] // Too short
    [InlineData("#FF00000")] // Too long
    [InlineData("#GGGGGG")] // Invalid hex characters
    [InlineData("red")] // Color name
    [InlineData("#FF-00-00")] // Invalid format
    public void Validate_WithInvalidHexColor_ShouldHaveValidationError(string color)
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            BackgroundColor = color
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BackgroundColor)
            .WithErrorMessage("BackgroundColor must be a valid hex color (e.g., #FF0000)");
    }

    #endregion
}
