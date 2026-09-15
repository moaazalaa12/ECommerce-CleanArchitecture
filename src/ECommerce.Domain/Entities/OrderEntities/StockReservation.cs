using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.CatalogEntities;

namespace ECommerce.Domain.Entities.OrderEntities;

public class StockReservation : BaseEntity
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public int? OrderId { get; set; }
    public bool IsReleased { get; set; } = false;

    public Product Product { get; set; } = null!;
    public Order? Order { get; set; }
}