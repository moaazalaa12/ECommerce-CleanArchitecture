using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities.OrderEntities;

public class Order : BaseEntity
{
    public Guid UserId { get; set; } = Guid.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public Address? ShippingAddress { get; set; } = null;

    public decimal ShippingFee { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public decimal DiscountAmount { get; init; } = 0;

    public Guid? CouponId { get; set; }

    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}