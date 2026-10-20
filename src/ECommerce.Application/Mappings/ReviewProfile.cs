using AutoMapper;
using ECommerce.Application.DTOs.Reviews;
using ECommerce.Domain.Entities.CatalogEntities;

namespace ECommerce.Application.Mappings
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<CreateReviewDto, Review>();

            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore());
        }
    }
}
