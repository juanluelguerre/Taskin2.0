using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.SeedWork;
using ElGuerre.Taskin.Infrastructure.EntityFramework;

namespace ElGuerre.Taskin.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TaskinDbContext _context;

    public UnitOfWork(TaskinDbContext context)
    {
        _context = context;
    }

    public async Task<ITransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new Transaction(transaction);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(ITransaction? transaction, CancellationToken cancellationToken = default)
    {
        if (transaction is Transaction txn)
        {
            await txn.CommitAsync(cancellationToken);
        }
    }
}