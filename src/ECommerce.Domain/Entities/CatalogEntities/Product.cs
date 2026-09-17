using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.UserEntities;

namespace ECommerce.Domain.Entities.CatalogEntities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }

    // Navigation Properties
    public Category Category { get; set; } = null!;
    public Brand? Brand { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductImage> Reviews { get; set; } = new List<ProductImage>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}