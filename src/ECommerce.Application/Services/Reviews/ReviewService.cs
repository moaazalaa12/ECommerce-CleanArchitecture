using Application.Exceptions;
using AutoMapper;
using ECommerce.Application.DTOs.Reviews;
using ECommerce.Application.Interfaces.Reviews;
using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;

namespace ECommerce.Application.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewDto> AddReviewAsync(Guid userId, string userName, CreateReviewDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product not found.");

            var existingReview = await _reviewRepo.GetUserReviewForProductAsync(userId, dto.ProductId, cancellationToken);
            if (existingReview != null)
                throw new BadRequestException("You have already reviewed this product.");

            var hasPurchased = await _orderRepo.HasUserPurchasedProductAsync(userId, dto.ProductId, cancellationToken);

            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                IsVerifiedPurchase = hasPurchased
            };

            await _reviewRepo.AddAsync(review);

            product.ReviewsCount += 1;
            product.AverageRating = ((product.AverageRating * (product.ReviewsCount - 1)) + dto.Rating) / product.ReviewsCount;
            _productRepo.Update(product);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = _mapper.Map<ReviewDto>(review);
            result.UserName = userName;
            return result;
        }

        public async Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var reviewsWithUserNames = await _reviewRepo.GetProductReviewsWithUserNameAsync(productId, cancellationToken);

            var result = new List<ReviewDto>();
            foreach (var item in reviewsWithUserNames)
            {
                var dto = _mapper.Map<ReviewDto>(item.Review);
                dto.UserName = item.UserName;
                result.Add(dto);
            }

            return result;
        }

        public async Task DeleteReviewAsync(Guid userId, Guid reviewId, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException("Review not found.");

            if (review.UserId != userId)
                throw new ForbiddenException("You can only delete your own review.");

            var product = await _productRepo.GetByIdAsync(review.ProductId, cancellationToken);
            if (product != null && product.ReviewsCount > 0)
            {
                if (product.ReviewsCount == 1)
                {
                    product.AverageRating = 0;
                    product.ReviewsCount = 0;
                }
                else
                {
                    product.AverageRating = ((product.AverageRating * product.ReviewsCount) - review.Rating) / (product.ReviewsCount - 1);
                    product.ReviewsCount -= 1;
                }
                _productRepo.Update(product);
            }

            _reviewRepo.Delete(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
