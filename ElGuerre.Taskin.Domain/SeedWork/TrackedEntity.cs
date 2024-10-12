namespace ElGuerre.Taskin.Domain.SeedWork;

public abstract class TrackedEntity : Entity
{
    protected TrackedEntity()
    {
    }

    protected TrackedEntity(Guid id) : base(id)
    {
    }

    public DateTimeOffset CreatedOn { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastModifiedOn { get; private set; }


    public void SetCreationInfo()
    {
        this.CreatedOn = DateTimeOffset.UtcNow;
        this.LastModifiedOn = null;
    }

    public void SetModificationInfo()
    {
        this.LastModifiedOn = DateTimeOffset.UtcNow;
    }
}