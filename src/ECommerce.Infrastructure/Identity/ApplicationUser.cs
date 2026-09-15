using Microsoft.AspNetCore.Identity;

using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Entities.UserEntities;

namespace ECommerce.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<WishlistItem> Wishlist { get; set; } = new List<WishlistItem>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
