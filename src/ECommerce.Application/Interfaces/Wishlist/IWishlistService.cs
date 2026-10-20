using ECommerce.Application.DTOs.Wishlist;

namespace ECommerce.Application.Interfaces.Wishlist
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
        Task RemoveFromWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WishlistItemDto>> GetUserWishlistAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> IsInWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default); // 🆕 Recommended
    }
}
