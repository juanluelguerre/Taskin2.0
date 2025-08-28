namespace ElGuerre.Taskin.Domain.SeedWork;

public abstract class TrackedEntity : Entity
{
    protected TrackedEntity()
    {
        CreatedOn = DateTimeOffset.UtcNow;
    }

    protected TrackedEntity(Guid id) : base(id)
    {
        CreatedOn = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset CreatedOn { get; private set; }
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