using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : ITransactionalCommand<Guid>
    {
        public Guid UserId { get; set; }
        public Guid? CouponId { get; set; }
        public AddressDto ShippingAddress { get; set; } = null!;
        public DeliveryMethod DeliveryMethod { get; set; }

        // This replaces the Cart temporarily <=========================================>
        public List<OrderItemRequestDto> Items { get; set; } = new();
    }
}
