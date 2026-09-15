using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.OrderEntities;

public class CouponUsage : BaseEntity
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid CouponId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;

    public Coupon Coupon { get; set; } = null!;
    public Order Order { get; set; } = null!;
}