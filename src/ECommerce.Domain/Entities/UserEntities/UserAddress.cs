using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities.UserEntities;

public class UserAddress : BaseEntity
{
    // The link to the user
    public Guid UserId { get; set; } = Guid.Empty;

    // The actual address data (The Value Object)
    public Address AddressDetails { get; set; } = null!;

    // Business context specific to the user's address book
    public string AddressLabel { get; set; } = string.Empty; // e.g., "Home", "Work"
    public bool IsDefault { get; set; } = false;
}