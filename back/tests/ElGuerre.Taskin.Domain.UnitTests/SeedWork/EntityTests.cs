using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.UnitTests.SeedWork;

/// <summary>
/// Tests for the Entity base class equality and identity logic
/// </summary>
public class EntityTests
{
    // Test helper: Create a concrete Entity implementation for testing
    private class TestEntity : Entity
    {
        public string? Name { get; set; }
    }

    [Fact]
    public void NewEntity_ShouldHaveGeneratedId()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().NotBeEmpty("entity constructor should generate a new Guid");
        entity.IsTransient().Should().BeFalse("entity with generated Guid should not be transient");
    }

    [Fact]
    public void IsTransient_WhenIdIsSet_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity();
        // Use reflection to set Id since it's private set
        typeof(Entity).GetProperty(nameof(Entity.Id))!
            .SetValue(entity, Guid.NewGuid());

        // Act
        var result = entity.IsTransient();

        // Assert
        result.Should().BeFalse("entity with non-default Guid should not be transient");
    }

    [Fact]
    public void Equals_WhenEntitiesHaveSameId_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity1, id);
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity2, id);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue("entities with same Id should be equal");
    }

    [Fact]
    public void Equals_WhenEntitiesHaveDifferentIds_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity1, Guid.NewGuid());
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity2, Guid.NewGuid());

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse("entities with different Ids should not be equal");
    }

    [Fact]
    public void Equals_WhenComparedToNull_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity();
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity, Guid.NewGuid());

        // Act
        var result = entity.Equals(null);

        // Assert
        result.Should().BeFalse("entity should not equal null");
    }

    [Fact]
    public void Equals_WhenBothAreTransient_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity { Name = "Test1" };
        var entity2 = new TestEntity { Name = "Test2" };

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse("transient entities should not be equal even if same type");
    }

    [Fact]
    public void OperatorEquals_WhenEntitiesHaveSameId_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity1, id);
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity2, id);

        // Act
        var result = entity1 == entity2;

        // Assert
        result.Should().BeTrue("operator == should return true for entities with same Id");
    }

    [Fact]
    public void OperatorNotEquals_WhenEntitiesHaveDifferentIds_ReturnsTrue()
    {
        // Arrange
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity1, Guid.NewGuid());
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity2, Guid.NewGuid());

        // Act
        var result = entity1 != entity2;

        // Assert
        result.Should().BeTrue("operator != should return true for entities with different Ids");
    }

    [Fact]
    public void GetHashCode_ForEntitiesWithSameId_ReturnsSameHashCode()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity1, id);
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity2, id);

        // Act
        var hash1 = entity1.GetHashCode();
        var hash2 = entity2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2, "entities with same Id should have same hash code");
    }

    [Fact]
    public void GetHashCode_ForTransientEntity_ReturnsConsistentHashCode()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        var hash1 = entity.GetHashCode();
        var hash2 = entity.GetHashCode();

        // Assert
        hash1.Should().Be(hash2, "transient entity should return consistent hash code");
    }
}
