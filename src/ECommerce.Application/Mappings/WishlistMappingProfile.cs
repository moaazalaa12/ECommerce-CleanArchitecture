using AutoMapper;
using ECommerce.Application.DTOs.Wishlist;
using ECommerce.Domain.Entities.UserEntities;

namespace ECommerce.Application.Mappings
{
    public class WishlistMappingProfile : Profile
    {
        public WishlistMappingProfile()
        {
            CreateMap<WishlistItem, WishlistItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(src => src.Product.DiscountPrice))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.Product.IsActive && src.Product.StockQuantity > 0))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.Images.FirstOrDefault() != null ? src.Product.Images.FirstOrDefault().Url : null));
        }
    }
}
