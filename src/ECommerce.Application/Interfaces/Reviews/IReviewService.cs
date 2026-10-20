using ECommerce.Application.DTOs.Reviews;

namespace ECommerce.Application.Interfaces.Reviews
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(Guid userId, string userName, CreateReviewDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(Guid productId, CancellationToken cancellationToken = default);
        Task DeleteReviewAsync(Guid userId, Guid reviewId, CancellationToken cancellationToken = default);
    }
}
