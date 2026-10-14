using AutoMapper;
using ECommerce.Application.DTOs.Notifications;
using ECommerce.Domain.Entities.UserEntities;

namespace ECommerce.Application.Mappings
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
        }
    }
}
