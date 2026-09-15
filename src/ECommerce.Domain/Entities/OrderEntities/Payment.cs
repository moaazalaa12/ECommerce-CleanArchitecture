using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities.OrderEntities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public Order Order { get; set; } = null!;
}