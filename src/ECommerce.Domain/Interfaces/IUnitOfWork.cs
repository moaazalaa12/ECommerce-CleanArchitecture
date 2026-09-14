namespace ECommerce.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // CancellationToken allows long-running database saves to be aborted if the user cancels the HTTP request
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}