using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.UnitTests.SeedWork;

/// <summary>
/// Tests for the TrackedEntity base class audit functionality
/// </summary>
public class TrackedEntityTests
{
    // Test helper: Create a concrete TrackedEntity implementation for testing
    private class TestTrackedEntity : TrackedEntity
    {
        public string? Name { get; set; }
    }

    [Fact]
    public void SetCreationInfo_ShouldSetCreatedOnToCurrentUtcTime()
    {
        // Arrange
        var entity = new TestTrackedEntity();
        var beforeCall = DateTimeOffset.UtcNow;

        // Act
        entity.SetCreationInfo();
        var afterCall = DateTimeOffset.UtcNow;

        // Assert
        entity.CreatedOn.Should().BeOnOrAfter(beforeCall);
        entity.CreatedOn.Should().BeOnOrBefore(afterCall);
        entity.CreatedOn.Offset.Should().Be(TimeSpan.Zero, "should use UTC time");
    }

    [Fact]
    public void SetModificationInfo_ShouldSetLastModifiedOnToCurrentUtcTime()
    {
        // Arrange
        var entity = new TestTrackedEntity();
        var beforeCall = DateTimeOffset.UtcNow;

        // Act
        entity.SetModificationInfo();
        var afterCall = DateTimeOffset.UtcNow;

        // Assert
        entity.LastModifiedOn.Should().NotBeNull();
        entity.LastModifiedOn!.Value.Should().BeOnOrAfter(beforeCall);
        entity.LastModifiedOn!.Value.Should().BeOnOrBefore(afterCall);
        entity.LastModifiedOn!.Value.Offset.Should().Be(TimeSpan.Zero, "should use UTC time");
    }

    [Fact]
    public void SetCreationInfo_ShouldNotSetLastModifiedOn()
    {
        // Arrange
        var entity = new TestTrackedEntity();

        // Act
        entity.SetCreationInfo();

        // Assert
        entity.CreatedOn.Should().NotBe(default(DateTimeOffset));
        entity.LastModifiedOn.Should().BeNull("creation should not set modification timestamp");
    }

    [Fact]
    public void SetModificationInfo_ShouldNotChangeCreatedOn()
    {
        // Arrange
        var entity = new TestTrackedEntity();
        entity.SetCreationInfo();
        var originalCreatedOn = entity.CreatedOn;

        // Simulate some time passing
        Thread.Sleep(10);

        // Act
        entity.SetModificationInfo();

        // Assert
        entity.CreatedOn.Should().Be(originalCreatedOn, "modification should not change creation timestamp");
        entity.LastModifiedOn.Should().NotBeNull();
        entity.LastModifiedOn!.Value.Should().BeAfter(originalCreatedOn);
    }

    [Fact]
    public void SetModificationInfo_CalledMultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var entity = new TestTrackedEntity();
        entity.SetModificationInfo();
        var firstModification = entity.LastModifiedOn!.Value;

        Thread.Sleep(10);

        // Act
        entity.SetModificationInfo();

        // Assert
        entity.LastModifiedOn.Should().NotBeNull();
        entity.LastModifiedOn!.Value.Should().BeAfter(firstModification,
            "subsequent modifications should update the timestamp");
    }

    [Fact]
    public void NewEntity_ShouldHaveCreatedOnSetToNow()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var entity = new TestTrackedEntity();
        var afterCreation = DateTimeOffset.UtcNow;

        // Assert
        entity.CreatedOn.Should().BeOnOrAfter(beforeCreation, "constructor should set CreatedOn to current UTC time");
        entity.CreatedOn.Should().BeOnOrBefore(afterCreation);
        entity.LastModifiedOn.Should().BeNull();
    }

    [Fact]
    public void CreatedOn_ShouldBeInUtc()
    {
        // Arrange
        var entity = new TestTrackedEntity();

        // Act
        entity.SetCreationInfo();

        // Assert
        entity.CreatedOn.Offset.Should().Be(TimeSpan.Zero);
        entity.CreatedOn.UtcDateTime.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
    }
}
