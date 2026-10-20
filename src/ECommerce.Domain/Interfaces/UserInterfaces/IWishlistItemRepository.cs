using ECommerce.Domain.Entities.UserEntities;

namespace ECommerce.Domain.Interfaces.UserInterfaces
{
    public interface IWishlistItemRepository : IGenericRepository<WishlistItem>
    {
        Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
        Task<WishlistItem?> GetByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WishlistItem>> GetUserWishlistWithProductAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
