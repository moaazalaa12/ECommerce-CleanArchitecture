using ECommerce.Domain.Entities.CatalogEntities;

namespace ECommerce.Domain.Interfaces.CatalogInterfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<Review?> GetUserReviewForProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<(Review Review, string UserName)>> GetProductReviewsWithUserNameAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
