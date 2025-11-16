namespace ElGuerre.Taskin.Domain.SeedWork;

public abstract class Entity
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected Entity(Guid id)
    {
        this.Id = id;
    }

    public Guid Id { get; protected set; }

    public bool IsTransient()
    {
        return this.Id == default;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity item)
            return false;

        if (object.ReferenceEquals(this, item))
            return true;

        if (this.GetType() != item.GetType())
            return false;

        if (item.IsTransient() || this.IsTransient())
            return false;

        return item.Id == this.Id;
    }

    public override int GetHashCode()
    {
        if (IsTransient())
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        return this.Id.GetHashCode() ^ 31;
    }

    public static bool operator ==(Entity left, Entity right)
    {
        return left?.Equals(right) ?? object.Equals(right, null);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}