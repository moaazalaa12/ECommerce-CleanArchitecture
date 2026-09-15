namespace ECommerce.Domain.Entities.UserEntities;

public class Address : BaseEntity
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;

    // Navigation Property
    public ApplicationUser User { get; set; } = null!;
}