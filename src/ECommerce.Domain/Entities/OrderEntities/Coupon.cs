using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities.OrderEntities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}