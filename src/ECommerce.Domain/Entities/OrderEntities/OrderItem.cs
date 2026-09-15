using ECommerce.Domain.Entities.CatalogEntities;

namespace ECommerce.Domain.Entities.OrderEntities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}