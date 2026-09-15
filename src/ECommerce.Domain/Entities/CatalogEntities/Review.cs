using ECommerce.Domain.Entities.UserEntities;

namespace ECommerce.Domain.Entities.CatalogEntities;

public class Review : BaseEntity
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid ProductId { get; set; }
    public int Rating { get; set; }         // 1 to 5
    public string? Comment { get; set; }
    public bool IsVerifiedPurchase { get; set; } = false;

    // Navigation Properties
    public ApplicationUser User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}