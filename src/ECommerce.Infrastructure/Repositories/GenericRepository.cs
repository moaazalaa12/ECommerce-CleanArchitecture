using System.Linq.Expressions;
using System.Security.Cryptography;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class GenericRepository<T>: IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _dbContext = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }


    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        // EF Core tracks changes automatically if the entity was fetched from the DB.
        // Update() is primarily used when dealing with disconnected entities (e.g., mapped from DTOs).
        _dbSet.Update(entity);
    }
    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
