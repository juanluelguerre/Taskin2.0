using ElGuerre.Taskin.Domain.SeedWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace ElGuerre.Taskin.Infrastructure;

public sealed class Transaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public Transaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
        TransactionId = Guid.NewGuid();
    }

    public Guid TransactionId { get; }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }
}