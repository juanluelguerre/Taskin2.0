using ElGuerre.Taskin.Application.Behaviors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ElGuerre.Taskin.Application.UnitTests.Behaviors;

/// <summary>
/// Tests for ValidationBehavior
/// </summary>
public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<ValidationTestRequest, ValidationTestResponse> _behavior;
    private readonly List<IValidator<ValidationTestRequest>> _validators;
    private readonly RequestHandlerDelegate<ValidationTestResponse> _next;

    public ValidationBehaviorTests()
    {
        _validators = new List<IValidator<ValidationTestRequest>>();
        _next = Substitute.For<RequestHandlerDelegate<ValidationTestResponse>>();
        _next().Returns(new ValidationTestResponse { Success = true });
    }

    [Fact]
    public async SystemTask Handle_WithNoValidators_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<ValidationTestRequest, ValidationTestResponse>(_validators);
        var request = new ValidationTestRequest { Value = "test" };

        // Act
        var result = await behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        await _next.Received(1)();
    }

    [Fact]
    public async SystemTask Handle_WithValidatorsAndValidRequest_ShouldCallNext()
    {
        // Arrange
        var validator = Substitute.For<IValidator<ValidationTestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _validators.Add(validator);
        var behavior = new ValidationBehavior<ValidationTestRequest, ValidationTestResponse>(_validators);
        var request = new ValidationTestRequest { Value = "valid" };

        // Act
        var result = await behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        await _next.Received(1)();
        await validator.Received(1).ValidateAsync(
            Arg.Is<ValidationContext<ValidationTestRequest>>(ctx => ctx.InstanceToValidate == request),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async SystemTask Handle_WithValidatorsAndInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var validator = Substitute.For<IValidator<ValidationTestRequest>>();
        var validationFailure = new ValidationFailure("Value", "Value is required");
        var validationResult = new ValidationResult(new[] { validationFailure });

        validator.ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);

        _validators.Add(validator);
        var behavior = new ValidationBehavior<ValidationTestRequest, ValidationTestResponse>(_validators);
        var request = new ValidationTestRequest { Value = null };

        // Act
        Func<SystemTask> act = async () => await behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Value" && e.ErrorMessage == "Value is required"));
        await _next.DidNotReceive()();
    }

    [Fact]
    public async SystemTask Handle_WithMultipleValidatorsAndMixedResults_ShouldCollectAllFailures()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<ValidationTestRequest>>();
        var validator2 = Substitute.For<IValidator<ValidationTestRequest>>();

        var failure1 = new ValidationFailure("Value", "First error");
        var failure2 = new ValidationFailure("Value", "Second error");

        validator1.ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { failure1 }));

        validator2.ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { failure2 }));

        _validators.Add(validator1);
        _validators.Add(validator2);
        var behavior = new ValidationBehavior<ValidationTestRequest, ValidationTestResponse>(_validators);
        var request = new ValidationTestRequest { Value = "invalid" };

        // Act
        Func<SystemTask> act = async () => await behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
        exception.Which.Errors.Should().Contain(e => e.ErrorMessage == "First error");
        exception.Which.Errors.Should().Contain(e => e.ErrorMessage == "Second error");
        await _next.DidNotReceive()();
    }

    [Fact]
    public async SystemTask Handle_ShouldExecuteAllValidatorsInParallel()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<ValidationTestRequest>>();
        var validator2 = Substitute.For<IValidator<ValidationTestRequest>>();
        var validator3 = Substitute.For<IValidator<ValidationTestRequest>>();

        validator1.ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        validator2.ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        validator3.ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _validators.Add(validator1);
        _validators.Add(validator2);
        _validators.Add(validator3);
        var behavior = new ValidationBehavior<ValidationTestRequest, ValidationTestResponse>(_validators);
        var request = new ValidationTestRequest { Value = "test" };

        // Act
        await behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        await validator1.Received(1).ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>());
        await validator2.Received(1).ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>());
        await validator3.Received(1).ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>());
    }

}

// Test helper classes
public record ValidationTestRequest : IRequest<ValidationTestResponse>
{
    public string? Value { get; init; }
}

public record ValidationTestResponse
{
    public bool Success { get; init; }
}
