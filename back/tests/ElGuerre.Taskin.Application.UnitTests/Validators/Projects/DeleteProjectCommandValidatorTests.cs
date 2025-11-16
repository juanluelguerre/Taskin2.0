using ElGuerre.Taskin.Application.Projects.Commands;
using FluentValidation.TestHelper;

namespace ElGuerre.Taskin.Application.UnitTests.Validators.Projects;

/// <summary>
/// Tests for DeleteProjectCommandValidator validation rules
/// </summary>
public class DeleteProjectCommandValidatorTests
{
    private readonly DeleteProjectCommandValidator validator = new();

    [Fact]
    public void Validate_WithValidId_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new DeleteProjectCommand { Id = Guid.NewGuid() };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteProjectCommand { Id = Guid.Empty };

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }
}
