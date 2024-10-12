namespace ElGuerre.Taskin.Domain.SeedWork;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<ITransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(ITransaction? transaction,
        CancellationToken cancellationToken = default);
}