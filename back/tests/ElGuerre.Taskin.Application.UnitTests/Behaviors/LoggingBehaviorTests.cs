using ElGuerre.Taskin.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElGuerre.Taskin.Application.UnitTests.Behaviors;

/// <summary>
/// Tests for LoggingBehavior
/// </summary>
public class LoggingBehaviorTests
{
    private readonly ILogger<LoggingBehavior<TestRequest, TestResponse>> _logger;
    private readonly LoggingBehavior<TestRequest, TestResponse> _behavior;
    private readonly RequestHandlerDelegate<TestResponse> _next;

    public LoggingBehaviorTests()
    {
        _logger = Substitute.For<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        _behavior = new LoggingBehavior<TestRequest, TestResponse>(_logger);
        _next = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        _next().Returns(new TestResponse { Success = true });
    }

    [Fact]
    public async SystemTask Handle_ShouldLogRequestStart()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };

        // Act
        await _behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Handling TestRequest")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async SystemTask Handle_WithSuccessfulExecution_ShouldLogCompletion()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };

        // Act
        var result = await _behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Handled TestRequest")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        await _next.Received(1)();
    }

    [Fact]
    public async SystemTask Handle_WithException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedException = new InvalidOperationException("Test exception");

        _next().Returns<TestResponse>(_ => throw expectedException);

        // Act
        Func<SystemTask> act = async () => await _behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Error handling TestRequest")),
            expectedException,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async SystemTask Handle_ShouldCallNextDelegate()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };

        // Act
        await _behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        await _next.Received(1)();
    }

    [Fact]
    public async SystemTask Handle_ShouldReturnResponseFromNext()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedResponse = new TestResponse { Success = true, Message = "Expected" };
        _next().Returns(expectedResponse);

        // Act
        var result = await _behavior.Handle(request, _next, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Expected");
    }

    [Fact]
    public async SystemTask Handle_WithCancellation_ShouldPropagateCancellation()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _next().Returns<TestResponse>(_ => throw new OperationCanceledException());

        // Act
        Func<SystemTask> act = async () => await _behavior.Handle(request, _next, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

}

// Test helper classes
public record TestRequest : IRequest<TestResponse>
{
    public string? Value { get; init; }
}

public record TestResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
}
