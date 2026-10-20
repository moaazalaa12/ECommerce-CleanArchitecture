using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Domain.Interfaces.UserInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.UserRepositories
{
    public class WishlistItemRepository : GenericRepository<WishlistItem>, IWishlistItemRepository
    {
        public WishlistItemRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);
        }

        public async Task<WishlistItem?> GetByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);
        }

        public async Task<IReadOnlyList<WishlistItem>> GetUserWishlistWithProductAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(w => w.Product)
                    .ThenInclude(p => p.Images)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
