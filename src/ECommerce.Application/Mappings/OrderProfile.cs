using AutoMapper;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Address, AddressDto>();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null));
        }
    }
}
