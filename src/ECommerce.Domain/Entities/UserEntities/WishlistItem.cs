using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.CatalogEntities;

namespace ECommerce.Domain.Entities.UserEntities;

public class WishlistItem : BaseEntity
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid ProductId { get; set; }

    // Navigation Properties
    public Product Product { get; set; } = null!;
}