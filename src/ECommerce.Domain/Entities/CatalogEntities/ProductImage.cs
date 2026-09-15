namespace ECommerce.Domain.Entities.CatalogEntities;

public class ProductImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; } = false;
    public Guid ProductId { get; set; }

    // Navigation Property
    public Product Product { get; set; } = null!;
}