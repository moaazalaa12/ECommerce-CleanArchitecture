using Application.Exceptions;
using AutoMapper;
using ECommerce.Application.DTOs.Wishlist;
using ECommerce.Application.Interfaces.Wishlist;
using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Domain.Interfaces.UserInterfaces;

namespace ECommerce.Application.Services.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistItemRepository _wishlistRepo;
        private readonly IProductRepository _productRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishlistService(
            IWishlistItemRepository wishlistRepo,
            IProductRepository productRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _wishlistRepo = wishlistRepo;
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddToWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepo.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product not found.");

            if (await _wishlistRepo.ExistsAsync(userId, productId, cancellationToken))
                throw new BadRequestException("Product already in wishlist.");

            var item = new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            };

            await _wishlistRepo.AddAsync(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveFromWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            var item = await _wishlistRepo.GetByUserAndProductAsync(userId, productId, cancellationToken);
            if (item == null) return;

            _wishlistRepo.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<WishlistItemDto>> GetUserWishlistAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var items = await _wishlistRepo.GetUserWishlistWithProductAsync(userId, cancellationToken);
            return _mapper.Map<IReadOnlyList<WishlistItemDto>>(items);
        }

        public async Task<bool> IsInWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            return await _wishlistRepo.ExistsAsync(userId, productId, cancellationToken);
        }
    }
}

