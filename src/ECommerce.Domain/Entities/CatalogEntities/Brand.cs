using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.CatalogEntities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}