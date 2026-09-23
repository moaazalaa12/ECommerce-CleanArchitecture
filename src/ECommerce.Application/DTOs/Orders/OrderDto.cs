using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Orders
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public AddressDto? ShippingAddress { get; set; }
        public decimal ShippingFee { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? CouponCode { get; set; }
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
