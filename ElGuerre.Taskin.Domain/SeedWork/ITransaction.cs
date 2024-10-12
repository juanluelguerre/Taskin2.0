namespace ElGuerre.Taskin.Domain.SeedWork;

public interface ITransaction : IDisposable, IAsyncDisposable
{
    Guid TransactionId { get; }
}